namespace DataIntegrationTool.CustomExceptions
{
    public class CsvReadException(CsvErrorType errorType, string message, Exception? innerException = null) : Exception(message, innerException)
    {
        public CsvErrorType ErrorType { get; } = errorType;
    }

    public enum CsvErrorType
    {
        FileNotFound,
        MissingField,
        TypeConversion,
        HeaderValidation,
        Generic
    }
}
