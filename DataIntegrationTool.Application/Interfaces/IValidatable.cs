using DataIntegrationTool.Application.DataValidation;

namespace DataIntegrationTool.Application.Interfaces
{
    public interface IValidatable
    {
        ValidationResult Validate();
    }
}
