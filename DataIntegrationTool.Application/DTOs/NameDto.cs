using DataIntegrationTool.Application.DataValidation;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Application.DTOs
{
    public partial class NameDto : ICleanable, IValidatable
    {
        public string? Value { get; set; }

        public void Clean()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
                return;
            }

            Value = char.ToUpper(Value[0]) + Value[1..].ToLower();
        }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(Value))
            {
                result.AddError(nameof(NameDto), "Value cannot be null or empty.");
            }
            else if (Value.Length < 2)
            {
                result.AddError(nameof(NameDto), "Value must be at least 2 characters.");
            }
            else if (!RegexVault.InvalidCharNameRegex().IsMatch(Value))
            {
                result.AddError(nameof(NameDto), "Value contains invalid characters.");
            }

            return result;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
