using System.Text.Json.Serialization;
using static DataIntegrationTool.Utils.CsvEnums;

namespace DataIntegrationTool.Config
{
    public class OutputSourceConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OutputType Type { get; set; }

        // Usato se Type == File
        public string? DestinationPath { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OutputFormat Format { get; set; } = OutputFormat.Json;

        // Usato se Type == Database
        public string? OutputConnectionString { get; set; }
        public string? Table { get; set; }

        // Usato se Type == Api
        public string? Endpoint { get; set; }
        public string? AuthToken { get; set; }
    }
}