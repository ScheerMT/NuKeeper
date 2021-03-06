using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using NuKeeper.Configuration;
using NuKeeper.Engine;
using NuKeeper.Inspection.Logging;

namespace NuKeeper.Commands
{
    [Command(Description = "Performs version checks and generates pull requests for all repositories the provided token can access.")]
    internal class GlobalCommand : GitHubNuKeeperCommand
    {
        private readonly GithubEngine _engine;

        public GlobalCommand(GithubEngine engine, IConfigureLogLevel logger) : base(logger)
        {
            _engine = engine;
        }

        protected override void PopulateSettings(SettingsContainer settings)
        {
            base.PopulateSettings(settings);
            settings.ModalSettings.Mode = RunMode.Global;
        }

        protected override ValidationResult ValidateSettings(SettingsContainer settings)
        {
            if (settings.UserSettings.PackageIncludes == null)
            {
                return ValidationResult.Failure("Global mode must have an include regex");
            }

            var apiHost = settings.GithubAuthSettings.ApiBase.Host;
            if (apiHost.EndsWith("github.com"))
            {
                return ValidationResult.Failure("Global mode must not use public github");
            }

            return base.ValidateSettings(settings);
        }

        protected override async Task<int> Run(SettingsContainer settings)
        {
            await _engine.Run(settings);
            return 0;
        }

    }
}
