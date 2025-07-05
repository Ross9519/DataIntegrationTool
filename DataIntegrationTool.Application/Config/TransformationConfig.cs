using System.Text.Json.Serialization;
using static DataIntegrationTool.Shared.Utils.CsvEnums;

namespace DataIntegrationTool.Application.Config
{
    public class TransformationConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransformationType Type { get; set; }

        // Specifico per ValidateRequiredFields
        public List<string>? Fields { get; set; }

        // Specifico per MapFields
        public Dictionary<string, string>? FieldMappings { get; set; }

        // Specifico per CustomScript
        public string? ScriptLanguage { get; set; }
        public string? ScriptContent { get; set; }
    }
}