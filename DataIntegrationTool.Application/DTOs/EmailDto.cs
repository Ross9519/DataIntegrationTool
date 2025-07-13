using DataIntegrationTool.Application.DataValidation;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Application.DTOs
{
    public partial class EmailDto : ICleanable, IValidatable
    {
        public string? Value { get; set; }

        public void Clean()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
                return;
            }

            var email = Value.Trim().ToLowerInvariant();

            // Rimuovi spazi interni
            email = RegexVault.RemoveInternalSpacesRegex().Replace(email, "");

            // Mantieni solo il primo @
            var parts = email.Split('@');
            if (parts.Length > 2)
            {
                email = parts[0] + "@" + string.Join("", parts.Skip(1));
            }

            // Rimuovi punti doppi nella parte locale
            var atIndex = email.IndexOf('@');
            if (atIndex > 0)
            {
                var local = email[..atIndex];
                var domain = email[(atIndex + 1)..];

                local = RegexVault.RemoveDoubleDotsRegex().Replace(local, ".");
                email = $"{local}@{domain}";
            }

            Value = email;
        }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(Value))
            {
                result.AddError(nameof(EmailDto), "Email is required.");
                return result;
            }

            if (!RegexVault.EmailValidationRegex().IsMatch(Value))
            {
                result.AddError(nameof(EmailDto), "Invalid email format.");
            }

            return result;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
