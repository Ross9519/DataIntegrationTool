using CsvHelper;
using CsvHelper.Configuration;
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


        public async Task<IEnumerable<T>> HandleContentAsync<T>(string? csvContent, CsvReaderOptions? options = null) where T : class
        {
            csvContent ??= string.Empty;

            ValidateHeaders(csvContent);

            try
            {
                var reader = new StringReader(csvContent);

                using var csv = new CsvReader(reader, GetCsvConfigFromCsvOpt(options));

                return await csv.GetRecordsAsync<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw MapCsvException(ex);
            }
        }

        public async Task<IEnumerable<T>> HandleStreamAsync<T>(Stream csvStream, CsvReaderOptions? options = null) where T : class
        {
            ValidateHeaders(csvStream);

            try
            {
                var reader = new StreamReader(csvStream);

                using var csv = new CsvReader(reader, GetCsvConfigFromCsvOpt(options));

                return await csv.GetRecordsAsync<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw MapCsvException(ex);
            }
        }

        public async Task<IEnumerable<T>> HandleFilePathAsync<T>(string filePath, CsvReaderOptions? options = null) where T : class
        {
            if (!File.Exists(filePath))
            {
                var errorType = CsvErrorType.FileNotFound;
                var message = $"{ERRORMESSAGE}: {errorType}";
                throw new CsvReadException(errorType, message, new FileNotFoundException());
            }

            using var stream = File.OpenRead(filePath);
            return await HandleStreamAsync<T>(stream, options);
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

        private static void ValidateHeaders(Stream csvStream)
        {
            long originalPosition = csvStream.Position;
            
            using var reader = new StreamReader(csvStream, leaveOpen: true);

            var firstLine = reader.ReadLine()
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

            csvStream.Position = originalPosition;
        }

        private static CsvReadException MapCsvException(Exception ex)
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
            return new CsvReadException(errorType, message, ex);
        }

        private static CsvConfiguration GetCsvConfigFromCsvOpt(CsvReaderOptions? options)
        {
            options ??= new CsvReaderOptions();

            return new CsvConfiguration(options.Culture)
            {
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeaderRecord,
                IgnoreBlankLines = options.IgnoreBlankLines,
                AllowComments = options.AllowComments
            };
        }
    }
}
