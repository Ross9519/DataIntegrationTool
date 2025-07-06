using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Application.DTOs
{
    public partial class EmailDto : ICleanable
    {
        public string? Value { get; set; }

        public void Clean()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
                return;
            }

            // 1. Trim e minuscolo
            var email = Value.Trim().ToLowerInvariant();

            // 2. Rimuovi spazi interni (caso raro ma possibile)
            email = RegexVault.RemoveInternalSpacesRegex().Replace(email, "");

            // 3. Rimuovi caratteri non validi per email (mantieni @ e .)
            email = RegexVault.RemoveNotEmailFriendlyCharRegex().Replace(email, "");

            // 4. Se ci sono più @, mantieni solo la prima (semplice pulizia)
            var atCount = email.Split('@').Length - 1;
            if (atCount > 1)
            {
                var parts = email.Split('@');
                email = parts[0] + "@" + string.Join("", parts, 1, parts.Length - 1);
            }

            // 5. Rimuovi punti doppi nella parte locale (prima della @)
            var atIndex = email.IndexOf('@');
            if (atIndex > 0)
            {
                var local = email[..atIndex];
                var domain = email[(atIndex + 1)..];

                local = RegexVault.RemoveDoubleDotsRegex().Replace(local, ".");
                domain = RegexVault.RemoveDoubleDotsRegex().Replace(local, ".");

                email = local + "@" + domain;
            }

            // 6. Semplice controllo formato (esclude valori completamente non email)
            if (!RegexVault.EmailSimpleValidationRegex().IsMatch(email))
            {
                Value = null;
                return;
            }

            Value = email;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
