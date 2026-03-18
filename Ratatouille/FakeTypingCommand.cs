using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;

namespace Ratatouille
{
    internal sealed class FakeTypingCommand
    {
        public const int CommandId = 0x0100;

        // BUG FIX #1: GUID-ul corect este guidRatatouillePackageCmdSet din .vsct,
        // NU guidImages! Înainte era setat greșit la GUID-ul pentru imagini:
        // {34afecfa-6c06-4a36-8490-685e38196dd5} — acela e guidImages!
        // Valoarea corectă e guidRatatouillePackageCmdSet: {125f8fca-8136-4a7d-be65-39638663d1ee}
        public static readonly Guid CommandSet = new Guid("125f8fca-8136-4a7d-be65-39638663d1ee");

        private readonly AsyncPackage package;

        // Concurrency control: 0 = disponibil, 1 = se generează
        private static int _isGenerating = 0;

        private FakeTypingCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            ThreadHelper.ThrowIfNotOnUIThread();

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static FakeTypingCommand Instance { get; private set; }
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new FakeTypingCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            System.IServiceProvider sysServiceProvider = (System.IServiceProvider)this.ServiceProvider;

            if (Interlocked.CompareExchange(ref _isGenerating, 1, 0) == 1)
            {
                var tempStatusBar = sysServiceProvider.GetService(typeof(SVsStatusbar)) as IVsStatusbar;
                tempStatusBar?.SetText("Ratatouille: Așteaptă, AI-ul generează deja cod...");
                return;
            }

            bool backgroundTaskStarted = false;

            try
            {
                var textManager = sysServiceProvider.GetService(typeof(SVsTextManager)) as IVsTextManager;
                if (textManager == null) return;

                textManager.GetActiveView(1, null, out IVsTextView activeView);
                if (activeView == null) return;

                var componentModel = sysServiceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
                if (componentModel == null) return;

                var editorFactory = componentModel.DefaultExportProvider.GetExportedValue<IVsEditorAdaptersFactoryService>();
                IWpfTextView wpfTextView = editorFactory.GetWpfTextView(activeView);
                if (wpfTextView == null) return;

                string selectedText = wpfTextView.Selection.StreamSelectionSpan.GetText();

                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        "Selectează un prompt text în editor înainte de a activa AI-ul.",
                        "Ratatouille AI",
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return;
                }

                if (wpfTextView.Properties.TryGetProperty(typeof(FakeTypingFilter), out FakeTypingFilter filter))
                {
                    var statusBar = sysServiceProvider.GetService(typeof(SVsStatusbar)) as IVsStatusbar;
                    statusBar?.SetText("Ratatouille: AI-ul generează cod...");

                    var options = (GeneralOptions)this.package.GetDialogPage(typeof(GeneralOptions));
                    string currentApiKey = options.ApiKey;

                    ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                    {
                        try
                        {
                            // BUG FIX #2: Folosim Task.Run pentru a ieși corect de pe UI thread,
                            // în loc de "await TaskScheduler.Default" care e ambiguu și poate
                            // cauza blocaje sau comportament nedefinit în VS SDK.
                            string aiResponse = await Task.Run(async () =>
                            {
                                var aiService = new AiService();
                                return await aiService.GenerateCodeAsync(selectedText, currentApiKey);
                            });

                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            // Ștergem textul selectat (promptul)
                            wpfTextView.TextBuffer.Delete(wpfTextView.Selection.StreamSelectionSpan.SnapshotSpan);

                            // BUG FIX #3: Resetăm poziția înainte de a seta textul nou,
                            // altfel dacă utilizatorul mai generează o dată fără să fi tastat
                            // tot textul anterior, _currentPosition rămâne la o valoare veche.
                            filter.StopFakeTyping();
                            filter.TextToType = aiResponse;
                            filter.IsFakeTypingActive = true;

                            statusBar?.SetText("Ratatouille: Codul e gata! Apasă orice tastă pentru a-l \"tasta\".");
                        }
                        catch (Exception ex)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            statusBar?.SetText($"Ratatouille: Eroare — {ex.Message}");
                        }
                        finally
                        {
                            Interlocked.Exchange(ref _isGenerating, 0);
                        }
                    });

                    backgroundTaskStarted = true;
                }
                else
                {
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        "Eroare: Filtrul de tastare nu este atașat acestui document!",
                        "Ratatouille Error",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
            finally
            {
                if (!backgroundTaskStarted)
                {
                    Interlocked.Exchange(ref _isGenerating, 0);
                }
            }
        }
    }
}