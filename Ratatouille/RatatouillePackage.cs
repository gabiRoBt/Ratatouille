using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace Ratatouille
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(RatatouillePackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(GeneralOptions), "Ratatouille AI", "General Settings", 0, 0, true)]
    public sealed class RatatouillePackage : AsyncPackage
    {
        public const string PackageGuidString = "cadea9e4-5f8b-4d17-ac60-90e43c1afa4d";

        // BUG FIX #4: Instance = this era lipsă complet! Câmpul static nu era niciodată
        // populat, deci orice cod care folosea RatatouillePackage.Instance primea null.
        public static RatatouillePackage Instance { get; private set; }

        public string GetApiKey()
        {
            var options = (GeneralOptions)this.GetDialogPage(typeof(GeneralOptions));
            return options.ApiKey;
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Setăm instanța statică acum că suntem pe UI thread
            Instance = this;

            await FakeTypingCommand.InitializeAsync(this);
        }
    }
}