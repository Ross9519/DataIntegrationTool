using System.Text;
using DataIntegrationTool.Application.DataValidation;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;
using PhoneNumbers;

namespace DataIntegrationTool.Application.DTOs
{
    public partial class PhoneDto : ICleanable, IValidatable
    {
        private string? _country;

        public string? Value { get; set; }
        public string? Country
        {
            get => _country;
            set => _country = ConvertCountryInIso2(value);
        }
        public string? CountryCode { get; private set; }
        public string? NationalNumber { get; private set; }

        public void Clean()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
                CountryCode = null;
                NationalNumber = null;
                return;
            }

            // Normalizza la stringa per rimuovere caratteri invisibili o diacritici strani
            var normalized = Value.Normalize(NormalizationForm.FormKC);

            // Rimuove spazi, parentesi, trattini, etc.
            var raw = RegexVault.RemoveCharInPhoneRegex().Replace(normalized, "");

            // Sostituisci prefisso 00 con +
            if (raw.StartsWith("00"))
            {
                raw = $"+{raw[2..]}";
            }

            var phoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                string? defaultRegion = null;

                // Se il numero non inizia con +, proviamo a dedurre la regione
                if (!raw.StartsWith('+'))
                {
                    // Se Country è valorizzata, prova ad usarla (es. "IT", "FR", "US", ecc)
                    if (!string.IsNullOrWhiteSpace(Country))
                    {
                        // Qui puoi fare una mappa o validare il codice paese
                        // Esempio semplice, usa countryUpper come codice regione
                        defaultRegion = ConvertCountryInIso2(Country);
                    }
                }

                // Parsing con o senza default region
                var parsed = phoneUtil.Parse(raw, defaultRegion);

                // Formatta il numero in formato E164 (standard internazionale)
                var formatted = phoneUtil.Format(parsed, PhoneNumberFormat.E164);

                Value = formatted;
                CountryCode = $"+{parsed.CountryCode}";
                NationalNumber = parsed.NationalNumber.ToString();
            }
            catch (NumberParseException)
            {
                // Se non è possibile parsare, resetta tutto
                Value = null;
                CountryCode = null;
                NationalNumber = null;
            }
        }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(Value))
            {
                result.AddError(nameof(PhoneDto), "Phone number is required.");
                return result;
            }

            var phoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                // Se inizia con +, forza parse come numero internazionale
                var regionCode = Value.StartsWith('+') ? "ZZ" : Country ?? "ZZ";
                var parsed = phoneUtil.Parse(Value, regionCode);

                // Valida il numero
                if (!phoneUtil.IsValidNumber(parsed))
                {
                    if (phoneUtil.IsPossibleNumber(parsed))
                    {
                        result.AddWarning(nameof(PhoneDto), "Phone number has a valid structure but may not be a real assigned number.");
                    }
                    else
                    {
                        result.AddError(nameof(PhoneDto), "Phone number has an invalid format.");
                    }
                }

                // Ottieni regione effettiva dal numero parsato
                var detectedRegion = phoneUtil.GetRegionCodeForNumber(parsed);

                // Verifica che il numero corrisponda alla country salvata
                if (!string.IsNullOrWhiteSpace(Country) &&
                    !string.Equals(detectedRegion, Country, StringComparison.OrdinalIgnoreCase))
                    {
                        result.AddWarning(nameof(PhoneDto), $"Phone prefix does not match selected country ({Country}). Detected: {detectedRegion}.");
                    }
            }
            catch (NumberParseException ex)
            {
                result.AddError(nameof(PhoneDto), $"Invalid phone number format: {ex.Message}");
            }

            return result;
        }

        public override string ToString() => Value ?? string.Empty;
        
        private static string? ConvertCountryInIso2(string? raw)
        {
            if (raw != null && Maps.CountryToISO2Map.TryGetValue(raw, out var country))
            {
                return country;
            }

            return null;
        }
    }
}
