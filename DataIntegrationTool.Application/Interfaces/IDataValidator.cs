using DataIntegrationTool.Application.DataValidation;

namespace DataIntegrationTool.Application.Interfaces
{
    public interface IDataValidator<T>
    {
        InvalidItemsReport<T> Validate(IEnumerable<T> raws);
        ValidationResult Validate(T raw);
    }
}
