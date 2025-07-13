using System.Text;

namespace DataIntegrationTool.Application.DataValidation
{
    public class ValidationResult
    {
        public Dictionary<string, string> Errors { get; } = [];
        public Dictionary<string, string> Warnings { get; } = [];

        public bool IsValid => Errors.Count == 0;
        public bool HasWarnings => Warnings.Count > 0;

        public void AddError(string fieldName, string message)
        {
            if (!Errors.ContainsKey(fieldName))
                Errors[fieldName] = message;
        }

        public void AddWarning(string fieldName, string message)
        {
            if (!Warnings.ContainsKey(fieldName))
                Warnings[fieldName] = message;
        }

        public void Add(string fieldName, ValidationResult nested)
        {
            foreach (var kv in nested.Errors)
            {
                AddError($"{fieldName}.{kv.Key}", kv.Value);
            }

            foreach (var kv in nested.Warnings)
            {
                AddWarning($"{fieldName}.{kv.Key}", kv.Value);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Errors.Count > 0)
            {
                sb.AppendLine("Errors:");
                foreach (var e in Errors)
                    sb.AppendLine($" - {e.Key}: {e.Value}");
            }

            if (Warnings.Count > 0)
            {
                sb.AppendLine("Warnings:");
                foreach (var w in Warnings)
                    sb.AppendLine($" - {w.Key}: {w.Value}");
            }

            return sb.ToString();
        }
    }
}
