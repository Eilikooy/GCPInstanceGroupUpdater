using Google.Apis.Compute.v1;
using Google.Apis.Compute.v1.Data;

namespace SharedLib
{
    public class Compute : ICompute
    {
        private readonly IAuthentication _authentication;
        private readonly ComputeService _computeService;
        public Compute(IAuthentication authentication)
        {
            _authentication = authentication;
            _computeService = new ComputeService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = _authentication.CredentialsFromApplication().Result,
                ApplicationName = "GCPInstanceGroupUpdater"
            });
        }

        public async Task StartVm(string project, string zone, string instanceName)
        {
            await _computeService.Instances.Start(project, zone, instanceName).ExecuteAsync();
        }

        public async Task<Instance> GetVmDetails(string project, string zone, string instanceName)
        {
            return await _computeService.Instances.Get(project, zone, instanceName).ExecuteAsync();
        }
        public string GetVmStatus(string project, string zone, string instanceName)
        {
            return _computeService.Instances.Get(project, zone, instanceName).Execute().Status;
        }

        public async Task StopVm(string project, string zone, string instanceName)
        {
            await _computeService.Instances.Stop(project, zone, instanceName).ExecuteAsync();
        }
        public async Task CreateImage(string project, string zone, string sourceImage, string destImageName, string imageFamily)
        {
            var response = await _computeService.Images.Insert(new Image { 
                SourceDisk = $"projects/{project}/zones/{zone}/disks/{sourceImage}",
                Family = imageFamily,
                Name = destImageName,
                 
            }, project).ExecuteAsync();
        }
        public string GetImageStatus(string project, string imageName)
        {
            return _computeService.Images.Get(project, imageName).Execute().Status;
        }
        public async Task CreateInstanceTemplate(string project, string baseInstanceTemplateName, string newInstanceTemplateName, string imageName)
        {
            var responseFromBaseTemplate = await _computeService.InstanceTemplates.Get(project, baseInstanceTemplateName).ExecuteAsync();
            var instanceProperty = responseFromBaseTemplate.Properties;
            instanceProperty.Disks.First().InitializeParams.SourceImage = $"projects/{project}/global/images/{imageName}";

            var response = await _computeService.InstanceTemplates.Insert(new InstanceTemplate
            {
                Name = newInstanceTemplateName,
                Properties = instanceProperty,
                Description = $"Base template: {baseInstanceTemplateName} Image: {imageName}"
            },
            project).ExecuteAsync();
        }
        public void GetInstanceTemplateStatus(string project, string instanceTemplate)
        {
            var result = _computeService.InstanceTemplates.Get(project, instanceTemplate).Execute();
        }
        public async Task UpdateManagedInstanceGroup(string project, string region, string instanceGroup, string instanceTemplateName)
        {
            var mig = await _computeService.RegionInstanceGroupManagers.Get(project, region, instanceGroup).ExecuteAsync();

            await _computeService.RegionInstanceGroupManagers.Patch(new InstanceGroupManager
            {
                UpdatePolicy = new InstanceGroupManagerUpdatePolicy
                {
                    Type = "PROACTIVE",
                    MinimalAction = "REPLACE",
                    MaxSurge = new FixedOrPercent { Fixed__ = 3 },
                    MaxUnavailable = new FixedOrPercent { Fixed__ = 0 },
                }, 
                InstanceTemplate = $"projects/{project}/global/instanceTemplates/{instanceTemplateName}",
            }, 
            project, region, instanceGroup).ExecuteAsync();
        }
        public async Task<bool> GetManagedInstanceGroupStatus(string project, string region, string instanceGroup)
        {
            var response = await _computeService.RegionInstanceGroupManagers.Get(project, region, instanceGroup).ExecuteAsync();
            if (response.Status.IsStable == null)
            {
                return false;
            }
            else
            {
                return (bool)response.Status.IsStable;
            }
        }
    }
    public interface ICompute
    {
        public Task StartVm(string project, string zone, string instanceName);
        public Task<Instance> GetVmDetails(string project, string zone, string instanceName);
        public string GetVmStatus(string project, string zone, string instanceName);
        public Task StopVm(string project, string zone, string instanceName);
        public Task CreateImage(string project, string zone, string sourceImage, string destImageName, string imageFamily);
        public string GetImageStatus(string project, string imageName);
        public Task CreateInstanceTemplate(string project, string baseInstanceTemplateName, string newInstanceTemplateName, string imageName);
        public void GetInstanceTemplateStatus(string project, string instanceTemplate);
        public Task UpdateManagedInstanceGroup(string project, string region, string instanceGroup, string instanceTemplateName);
        public Task<bool> GetManagedInstanceGroupStatus(string project, string region, string instanceGroup);
    }
}