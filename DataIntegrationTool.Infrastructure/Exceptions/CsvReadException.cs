using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Infrastructure.Exceptions
{
    public class CsvReadException(CsvEnums.CsvErrorType errorType, string message, Exception? innerException = null) : Exception(message, innerException)
    {
        public CsvEnums.CsvErrorType ErrorType { get; } = errorType;
    }
}
