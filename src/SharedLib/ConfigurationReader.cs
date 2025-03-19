using SharedLib.Models;
using System.Text.Json;

namespace SharedLib
{
    public class ConfigurationReader : IConfigurationReader
    {
        public ConfigurationReader() { }

        public Configuration ReadConfiguration()
        {
            return JsonSerializer.Deserialize<Configuration>(File.ReadAllText("config.json")) ?? throw new NullReferenceException();
        }
        public async Task<Configuration> ReadConfigurationAsync()
        {
            try
            {
                var path = Environment.CurrentDirectory;
                return JsonSerializer.Deserialize<Configuration>(await File.ReadAllTextAsync("config.json")) ?? throw new NullReferenceException();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task WriteConfigurationAsync(Configuration configuration)
        {
            await File.WriteAllTextAsync("config.test.json", JsonSerializer.Serialize<Configuration>(configuration, new JsonSerializerOptions { WriteIndented = true}));
        }
    }
    public interface IConfigurationReader
    {
        public Configuration ReadConfiguration();
        public Task<Configuration> ReadConfigurationAsync();
        public Task WriteConfigurationAsync(Configuration configuration);
    }
}
