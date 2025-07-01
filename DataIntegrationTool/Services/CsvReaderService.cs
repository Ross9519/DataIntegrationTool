using System.Globalization;
using CsvHelper;
using CsvHelper.TypeConversion;
using DataIntegrationTool.CustomExceptions;
using DataIntegrationTool.Services.Interfaces;

namespace DataIntegrationTool.Services
{
    public class CsvReaderService : ICsvReaderService
    {
        private const string ERRORMESSAGE = "Errore durante la lettura del file CSV";

        public async Task<IEnumerable<T>> ConvertFromCsvToTAsync<T>(string csvContent) where T : class
        {
            try
            {
                var reader = new StringReader(csvContent);

                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add(string.Empty);

                return await csv.GetRecordsAsync<T>().ToListAsync();
            }
            catch(Exception ex)
            {
                var errorType = ex switch
                {
                    FileNotFoundException => CsvErrorType.FileNotFound,
                    CsvHelper.MissingFieldException => CsvErrorType.MissingField,
                    TypeConverterException => CsvErrorType.TypeConversion,
                    HeaderValidationException => CsvErrorType.HeaderValidation,
                    _ => CsvErrorType.Generic
                };

                var message = $"{ERRORMESSAGE}: {errorType}";
                throw new CsvReadException(errorType, message, ex);
            }
        }

        public async Task<IEnumerable<T>> ReadFromFileAsync<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
            {
                var errorType = CsvErrorType.FileNotFound;
                var message = $"{ERRORMESSAGE}: {errorType}";
                throw new CsvReadException(errorType, message, new FileNotFoundException());
            }

            var csvContent = await File.ReadAllTextAsync(filePath);
            return await ConvertFromCsvToTAsync<T>(csvContent);
        }
    }
}
