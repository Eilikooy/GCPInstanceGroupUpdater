using Microsoft.Extensions.Logging;

namespace SharedLib
{
    public class UpdateManagedInstanceGroup : IUpdateManagedInstanceGroup
    {
        private readonly ILogger<UpdateManagedInstanceGroup> _logger;
        private readonly ICompute _compute;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IDbActions _db;
        private readonly string _suffix = $"-{DateTimeOffset.Now.ToString("yyyy-MM-dd")}-{new Random().Next(0, 10000)}";

        public UpdateManagedInstanceGroup(ILogger<UpdateManagedInstanceGroup> logger, ICompute compute, ICommandExecutor commandExecutor, IDbActions db)
        { 
            _logger = logger;
            _compute = compute;
            _commandExecutor = commandExecutor;
            _db = db;
        }
        public async Task ExecuteAsync(string friendlyName, string username, string sshKeyPath, bool autoContinue = false)
        {
            var settings = await _db.GetByFriendlyName(friendlyName);

            ushort tryCount = 0;
            if (_compute.GetVmStatus(settings.Project, settings.Zone, settings.TemplateInstanceName) == "TERMINATED")
            {
                _logger.LogInformation("Starting instance {0}...", settings.TemplateInstanceName);
                await _compute.StartVm(settings.Project, settings.Zone, settings.TemplateInstanceName);
            }
            while (_compute.GetVmStatus(settings.Project, settings.Zone, settings.TemplateInstanceName) != "RUNNING")
            {
                _logger.LogInformation("Waiting Running status for instance {0}...", settings.TemplateInstanceName);
                Thread.Sleep(TimeSpan.FromSeconds(30));
                if (tryCount >= 10)
                {
                    tryCount = 0;
                    _logger.LogWarning("Instance didn't start");
                    Environment.Exit(1);
                }
                tryCount++;
            }

            var instanceDetails = await _compute.GetVmDetails(settings.Project, settings.Zone, settings.TemplateInstanceName);
            if (instanceDetails.NetworkInterfaces.First().AccessConfigs.Any())
            {
                _commandExecutor.SshExecutePubKeyAuth(instanceDetails.NetworkInterfaces.First().AccessConfigs.First().NatIP, 
                    username, 
                    sshKeyPath, 
                    settings.SshCommand);
            }
            else
            {
                _commandExecutor.SshExecutePubKeyAuth(instanceDetails.NetworkInterfaces.First().NetworkIP, 
                    username, 
                    sshKeyPath,
                    settings.SshCommand);
            }
            if (!autoContinue)
            {
                Console.WriteLine("\nContinue? (y/n)");
                string? keyPress = Console.ReadLine();

                if (keyPress != null)
                {
                    switch (keyPress)
                    {
                        case "y":
                            break;
                        case "Y":
                            break;
                        default:
                            Environment.Exit(1);
                            break;
                    }
                }
            }
            await _compute.StopVm(settings.Project, settings.Zone, settings.TemplateInstanceName);

            while (_compute.GetVmStatus(settings.Project, settings.Zone, settings.TemplateInstanceName) != "TERMINATED")
            {
                _logger.LogInformation("Waiting for instance {0} to be stopped...", settings.TemplateInstanceName);
                Thread.Sleep(TimeSpan.FromSeconds(20));
                if (tryCount >= 30)
                {
                    _logger.LogWarning("Instance {0} didn't stop in time", settings.TemplateInstanceName);
                    Environment.Exit(1);
                }
                tryCount++;
            }

            await _compute.CreateImage(settings.Project, settings.Zone, settings.TemplateInstanceName, $"{settings.ImagePrefix}{_suffix}", settings.TemplateImageFamily);
            while (_compute.GetImageStatus(settings.Project, $"{settings.ImagePrefix}{_suffix}") != "READY")
            {
                _logger.LogInformation("Waiting for {0} Image to be created...", settings.ImagePrefix + _suffix);
                Thread.Sleep(TimeSpan.FromSeconds(20));
                if (tryCount >= 30)
                {
                    _logger.LogWarning("Waiting for image creation timed out.");
                    Environment.Exit(1);
                }
                tryCount++;
            }
            await _compute.CreateInstanceTemplate(settings.Project, settings.SourceTemplateName, $"{settings.SourceTemplateName}{_suffix}", $"{settings.ImagePrefix}{_suffix}");
            await _compute.UpdateManagedInstanceGroup(settings.Project, settings.Region, settings.InstanceGroupName, $"{settings.SourceTemplateName}{_suffix}");

            while (!await _compute.GetManagedInstanceGroupStatus(settings.Project, settings.Region, settings.InstanceGroupName))
            {
                _logger.LogInformation("Waiting for Instance Group {0} to be ready...", settings.InstanceGroupName);
                Thread.Sleep(TimeSpan.FromSeconds(30));
                if (tryCount >= 30)
                {
                    _logger.LogWarning("Instance Group {0} not ready", settings.InstanceGroupName);
                    Environment.Exit(1);
                }
                tryCount++;
            }
        }
    }
    public interface IUpdateManagedInstanceGroup
    {
        public Task ExecuteAsync(string friendlyName, string username, string sshKeyPath, bool autoContinue = false);
    }
}
