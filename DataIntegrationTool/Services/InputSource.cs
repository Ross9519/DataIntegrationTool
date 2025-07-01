namespace DataIntegrationTool.Services
{
    public class InputSource
    {
        public string Type { get; set; } = "File"; // oppure "Stream" o "String"
        public string FilePath { get; set; } = "";
        public string? Content { get; set; }
    }
}
