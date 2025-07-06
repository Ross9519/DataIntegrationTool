using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Application.DTOs
{
    public class CountryDto : ICleanable
    {
        public string? Value { get; set; }

        public void Clean()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
                return;
            }

            var raw = Value.Trim().ToUpperInvariant();

            // Mappa di normalizzazione: ISO codes e nomi
            var normalized = NormalizeCountry(raw);

            Value = normalized;
        }

        private static string? NormalizeCountry(string raw)
        {
            if (Maps.CountryMap.TryGetValue(raw, out var country))
            {
                return country;
            }

            return null;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
