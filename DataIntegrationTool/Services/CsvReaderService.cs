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
        private const char CSVSEPARATOR = ',';
        private static readonly char[] firstLineSeparator = ['\r', '\n'];

        public async Task<IEnumerable<T>> ConvertFromCsvToTAsync<T>(string csvContent) where T : class
        {
            ValidateHeaders(csvContent);

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
                    HeaderValidationException => CsvErrorType.MissingHeader,
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

        private static void ValidateHeaders(string csvContent)
        {
            var firstLine = csvContent.Split(firstLineSeparator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() 
                ?? throw new CsvReadException(CsvErrorType.MissingAllHeaders, $"{ERRORMESSAGE}: {CsvErrorType.MissingAllHeaders}", null);
            
            var headers = firstLine.Split(CSVSEPARATOR);
            var duplicates = headers.GroupBy(h => h.Trim())
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key);

            if (duplicates.Any())
            {
                var message = $"{ERRORMESSAGE}: {CsvErrorType.DuplicateHeader}\rDuplicati: {string.Join(", ", duplicates)}";
                throw new CsvReadException(CsvErrorType.DuplicateHeader, message, null);
            }
        }
    }
}
