using System.Reflection;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace DataIntegrationTool.Shared.Utils
{
    public class InternalDtoConverter<T> : DefaultTypeConverter where T : class, new()
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            // Crea una nuova istanza di T e imposta la proprietà 'Value' a text
            var instance = new T();
            var prop = typeof(T).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(instance, text.Trim());
                return instance;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }

        public override string ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value == null)
                return string.Empty;

            var prop = typeof(T).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                var val = prop.GetValue(value) as string;
                return val ?? string.Empty;
            }

            return base.ConvertToString(value, row, memberMapData) ?? string.Empty;
        }
    }
}
