using System.Globalization;

namespace DataIntegrationTool.Services
{
    public class CsvReaderOptions
    {
        public char Delimiter { get; set; } = ',';
        public bool HasHeaderRecord { get; set; } = true;
        public bool IgnoreBlankLines { get; set; } = true;
        public bool AllowComments { get; set; } = false;
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;
    }
}
