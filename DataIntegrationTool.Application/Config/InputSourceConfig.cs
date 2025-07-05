using System.Text.Json.Serialization;
using static DataIntegrationTool.Shared.Utils.CsvEnums;

namespace DataIntegrationTool.Application.Config
{
    public class InputSourceConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public InputType Type { get; set; }

        // Usato se Type == File
        public string? FilePath { get; set; }

        // Usato se Type == Http
        public string? Url { get; set; }

        // Usato se Type == Database
        public string? ConnectionString { get; set; }
        public string? Query { get; set; }

        // Usato se si vuole fornire direttamente il CSV come stringa (alternativa a FilePath/Http/Database)
        public string? CsvStringContent { get; set; }

        // Opzionale, per configurare la lettura CSV
        public string Encoding { get; set; } = "utf-8";
        public CsvReaderOptionsConfig Options { get; set; } = new();
    }
}
