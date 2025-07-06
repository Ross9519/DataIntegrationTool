using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Application.DTOs
{
    public class NameDto : ICleanable
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

        public override string ToString() => Value ?? string.Empty;
    }
}
