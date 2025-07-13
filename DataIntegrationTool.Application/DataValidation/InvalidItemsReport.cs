namespace DataIntegrationTool.Application.DataValidation
{
    public class InvalidItemsReport<T>
    {
        public IEnumerable<T> ValidItems { get; init; } = [];
        public IEnumerable<(T Item, ValidationResult Result)> InvalidItems { get; init; } = [];
    }
}
