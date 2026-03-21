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
        public const int CommandIdR = 0x0100;
        public const int CommandIdP = 0x0101;
        public const int CommandIdS = 0x0102;
        public const int CommandIdF = 0x0103;

        public static readonly Guid CommandSet = new Guid("125f8fca-8136-4a7d-be65-39638663d1ee");

        private readonly AsyncPackage package;
        private static int _isGenerating = 0;
        private static CancellationTokenSource _dotAnimationCts;

        private FakeTypingCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            ThreadHelper.ThrowIfNotOnUIThread();

            commandService.AddCommand(new MenuCommand(ExecuteR, new CommandID(CommandSet, CommandIdR)));
            commandService.AddCommand(new MenuCommand(ExecuteP, new CommandID(CommandSet, CommandIdP)));
            commandService.AddCommand(new MenuCommand(ExecuteS, new CommandID(CommandSet, CommandIdS)));
            commandService.AddCommand(new MenuCommand(ExecuteF, new CommandID(CommandSet, CommandIdF)));
        }

        public static FakeTypingCommand Instance { get; private set; }
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new FakeTypingCommand(package, commandService);
        }

        private IWpfTextView GetActiveWpfTextView()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            System.IServiceProvider sp = (System.IServiceProvider)this.ServiceProvider;

            var textManager = sp.GetService(typeof(SVsTextManager)) as IVsTextManager;
            if (textManager == null) return null;
            textManager.GetActiveView(1, null, out IVsTextView activeView);
            if (activeView == null) return null;

            var componentModel = sp.GetService(typeof(SComponentModel)) as IComponentModel;
            if (componentModel == null) return null;

            var editorFactory = componentModel.DefaultExportProvider.GetExportedValue<IVsEditorAdaptersFactoryService>();
            return editorFactory.GetWpfTextView(activeView);
        }

        private IVsStatusbar GetStatusBar()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            System.IServiceProvider sp = (System.IServiceProvider)this.ServiceProvider;
            return sp.GetService(typeof(SVsStatusbar)) as IVsStatusbar;
        }

        private async Task RunDotAnimationAsync(IVsStatusbar statusBar, CancellationToken token)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            statusBar?.SetText("Ratatouille");
            try
            {
                await Task.Delay(1000, token);
                string dots = "";
                while (!token.IsCancellationRequested)
                {
                    dots += ".";
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    statusBar?.SetText(dots);
                    await Task.Delay(700, token);
                }
            }
            catch (TaskCanceledException) { }
        }

        // ── R: Generate ───────────────────────────────────────────────────────────

        private void ExecuteR(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Interlocked.CompareExchange(ref _isGenerating, 1, 0) == 1)
            {
                GetStatusBar()?.SetText("Ratatouille: Please wait, AI is already generating code...");
                return;
            }

            bool backgroundTaskStarted = false;

            try
            {
                IWpfTextView wpfTextView = GetActiveWpfTextView();
                if (wpfTextView == null) return;

                string selectedText = wpfTextView.Selection.StreamSelectionSpan.GetText();

                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        "Please select a text prompt in the editor before activating the AI.",
                        "Ratatouille",
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return;
                }

                if (!wpfTextView.Properties.TryGetProperty(typeof(FakeTypingFilter), out FakeTypingFilter filter))
                {
                    VsShellUtilities.ShowMessageBox(
                        this.package,
                        "Error: The typing filter is not attached to this document!",
                        "Ratatouille",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return;
                }

                var statusBar = GetStatusBar();
                var options = (GeneralOptions)this.package.GetDialogPage(typeof(GeneralOptions));
                string apiKey = options.ApiKey;
                string model = options.Model;
                double temp = options.Temperature;
                string sysPrompt = options.SystemPrompt;

                _dotAnimationCts = new CancellationTokenSource();
                var animToken = _dotAnimationCts.Token;
                ThreadHelper.JoinableTaskFactory.RunAsync(() => RunDotAnimationAsync(statusBar, animToken));

                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    try
                    {
                        string aiResponse = await Task.Run(async () =>
                        {
                            var aiService = new AiService();
                            return await aiService.GenerateCodeAsync(selectedText, apiKey, model, temp, sysPrompt);
                        });

                        _dotAnimationCts?.Cancel();

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        wpfTextView.TextBuffer.Delete(wpfTextView.Selection.StreamSelectionSpan.SnapshotSpan);

                        filter.StopFakeTyping();
                        filter.TextToType = aiResponse;
                        filter.IsFakeTypingActive = true;

                        statusBar?.SetText("!");
                        await Task.Delay(1000);
                        statusBar?.SetText("");
                    }
                    catch (Exception ex)
                    {
                        _dotAnimationCts?.Cancel();
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        statusBar?.SetText(ex.Message);
                    }
                    finally
                    {
                        Interlocked.Exchange(ref _isGenerating, 0);
                    }
                });

                backgroundTaskStarted = true;
            }
            finally
            {
                if (!backgroundTaskStarted)
                    Interlocked.Exchange(ref _isGenerating, 0);
            }
        }

        // ── P: Pause / Resume ─────────────────────────────────────────────────────

        private void ExecuteP(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IWpfTextView wpfTextView = GetActiveWpfTextView();
            if (wpfTextView == null) return;
            if (!wpfTextView.Properties.TryGetProperty(typeof(FakeTypingFilter), out FakeTypingFilter filter)) return;
            if (!filter.IsFakeTypingActive) return;

            var statusBar = GetStatusBar();

            if (filter.IsPaused)
            {
                filter.Resume();
                statusBar?.SetText("");
            }
            else
            {
                filter.Pause();
                statusBar?.SetText("-");
            }
        }

        // ── S: Stop ───────────────────────────────────────────────────────────────

        private void ExecuteS(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IWpfTextView wpfTextView = GetActiveWpfTextView();
            if (wpfTextView == null) return;
            if (!wpfTextView.Properties.TryGetProperty(typeof(FakeTypingFilter), out FakeTypingFilter filter)) return;

            filter.StopFakeTyping();

            var statusBar = GetStatusBar();
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                statusBar?.SetText(".");
                await Task.Delay(1000);
                statusBar?.SetText("");
            });
        }

        // ── F: Finish instantly ───────────────────────────────────────────────────

        private void ExecuteF(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IWpfTextView wpfTextView = GetActiveWpfTextView();
            if (wpfTextView == null) return;
            if (!wpfTextView.Properties.TryGetProperty(typeof(FakeTypingFilter), out FakeTypingFilter filter)) return;

            filter.FinishInstantly();

            var statusBar = GetStatusBar();
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                statusBar?.SetText(".");
                await Task.Delay(1000);
                statusBar?.SetText("");
            });
        }
    }
}