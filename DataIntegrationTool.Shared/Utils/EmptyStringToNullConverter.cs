using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace DataIntegrationTool.Shared.Utils
{
    public class EmptyStringToNullConverter : StringConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            return string.IsNullOrWhiteSpace(text) ? null : base.ConvertFromString(text, row, memberMapData);
        }
    }
}
