using System.Text.Json.Serialization;

namespace SharedLib.Models
{
    public class Configuration
    {
        [JsonPropertyName("databases")]
        public List<DatabaseConfiguration> Databases { get; set; } = new List<DatabaseConfiguration>();
    }
}
