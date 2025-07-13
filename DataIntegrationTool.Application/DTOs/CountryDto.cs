using DataIntegrationTool.Application.DataValidation;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Application.DTOs
{
    public class CountryDto : ICleanable, IValidatable
    {
        private string? _value;

        public string? Value { 
            get => _value; 
            set {
                var raw = value?.Trim()?.ToUpperInvariant();

                // Mappa di normalizzazione: ISO codes e nomi
                _value = NormalizeCountry(raw);
            }
        }

        public void Clean()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
                return;
            }
        }

        private static string? NormalizeCountry(string? raw)
        {
            if (raw != null && Maps.CountryMap.TryGetValue(raw, out var country))
            {
                return country;
            }

            return null;
        }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(Value))
            {
                result.AddError(nameof(CountryDto), "Country is required.");
            }
            else if (!Maps.CountryMap.ContainsValue(Value))
            {
                result.AddError(nameof(CountryDto), $"Invalid country value: '{Value}'.");
            }

            return result;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
