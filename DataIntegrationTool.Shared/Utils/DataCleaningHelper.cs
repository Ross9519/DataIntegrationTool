using System.Reflection;
using System.Text;

namespace DataIntegrationTool.Shared.Utils
{
    public static partial class DataCleaningHelper
    {
        public static string? CleanString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string cleaned = input.Trim();
            cleaned = RemoveExtraSpaces(cleaned);
            cleaned = RemoveControlCharacters(cleaned);
            cleaned = NormalizeUnicode(cleaned);

            return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
        }

        private static string RemoveExtraSpaces(string input)
        {
            return RegexVault.ExtraSpacesRegex().Replace(input, " ");
        }

        private static string RemoveControlCharacters(string input)
        {
            return new string(input.Where(c => !char.IsControl(c)).ToArray());
        }

        private static string NormalizeUnicode(string input)
        {
            return input.Normalize(NormalizationForm.FormC);
        }

        public static IEnumerable<T> RemoveDuplicates<T>(IEnumerable<T> entities) where T : class
        {
            return entities
                .GroupBy(item => GetPropertyValuesKey(item))
                .Select(g => g.First());
        }

        private static string GetPropertyValuesKey<T>(T obj)
        {
            if (obj == null)
                return string.Empty;

            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .OrderBy(p => p.Name); // Ordine deterministico

            var values = props.Select(p =>
            {
                var value = p.GetValue(obj);
                return value != null ? value.ToString() : "null";
            });

            return string.Join("|", values); // chiave di confronto
        }
    }
}
