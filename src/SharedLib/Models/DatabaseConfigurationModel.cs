using System.Text.Json.Serialization;

namespace SharedLib.Models
{
    public class DatabaseConfiguration
    {
        [JsonPropertyName("database")]
        public string Database { get; set; } = string.Empty;
        [JsonPropertyName("settings")]
        public Sql Settings { get; set; } = new Sql();
    }

    public class Sql
    {
        [JsonPropertyName("host")]
        public string Host { get; set; } = string.Empty;

        [JsonPropertyName("port")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ushort? Port { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("databasename")]
        public string DatabaseName { get; set; } = string.Empty;
    }
}
