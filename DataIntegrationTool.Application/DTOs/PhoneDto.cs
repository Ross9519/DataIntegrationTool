using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Application.DTOs
{
    public partial class PhoneDto : ICleanable
    {
        public string? Value { get; set; }

        public string? CountryCode { get; private set; }
        public string? NationalNumber { get; private set; }

        public void Clean()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
                return;
            }

            // Rimuove spazi, parentesi, trattini, ecc.
            var raw = RegexVault.RemoveCharInPhoneRegex().Replace(Value, "");

            // Normalizza il prefisso internazionale
            if (raw.StartsWith("00"))
            {
                raw = $"+{raw[2..]}";
            }

            if (!raw.StartsWith('+'))
            {
                // Se non ha prefisso, assume Italia per default
                raw = $"+39{raw}";
            }

            // Ora separa prefisso e numero nazionale
            var match = RegexVault.PrefixFromNumberSeparationRegex().Match(raw);

            if (!match.Success)
            {
                Value = null;
                return;
            }

            CountryCode = "+" + match.Groups["prefix"].Value;
            NationalNumber = match.Groups["number"].Value;

            // Ricompone in formato standard
            Value = $"{CountryCode}{NationalNumber}";
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
