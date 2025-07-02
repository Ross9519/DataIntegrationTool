using System.Globalization;

namespace DataIntegrationTool.Config
{
    public class CsvReaderOptionsConfig
    {
        public char Delimiter { get; set; } = ',';
        public bool HasHeaderRecord { get; set; } = true;
        public bool IgnoreBlankLines { get; set; } = true;
        public bool AllowComments { get; set; } = false;
        public string Comment { get; set; } = "#";
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
    }
}
