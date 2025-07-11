using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using static DataIntegrationTool.Shared.Utils.CsvEnums;
using static DataIntegrationTool.Shared.Utils.Constants;
using System.Text;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Infrastructure.Exceptions;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Shared.Utils;
using DataIntegrationTool.Application.DTOs;

namespace DataIntegrationTool.Infrastructure.Services
{
    public class CsvReaderService : ICsvReaderService
    {
        public async Task<IEnumerable<T>> ReadCsvAsync<T>(Stream csvStream, CsvReaderOptionsConfig? options, string encoding) where T : class
        {
            try
            {
                var (preparedStream, realEncoding) = StreamUtils.PrepareStreamAndEncoding(csvStream, encoding);

                options ??= new CsvReaderOptionsConfig();

                var (headers, prependedStream) = ReadHeaderPreserveStream(preparedStream, encoding);

                ValidateHeaders(headers, options);

                var csvConfig = GetCsvConfigFromCsvOpt(options);

                var reader = new StreamReader(prependedStream, realEncoding, detectEncodingFromByteOrderMarks: false, leaveOpen: true);

                using var csv = new CsvReader(reader, csvConfig);

                AddConverterForInternalDto(csv);

                return await csv.GetRecordsAsync<T>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw MapCsvException(ex);
            }
        }

        private static void AddConverterForInternalDto(CsvReader csv)
        {
            csv.Context.TypeConverterCache.AddConverter<PhoneDto>(new InternalDtoConverter<PhoneDto>());
            csv.Context.TypeConverterCache.AddConverter<EmailDto>(new InternalDtoConverter<EmailDto>());
            csv.Context.TypeConverterCache.AddConverter<NameDto>(new InternalDtoConverter<NameDto>());
            csv.Context.TypeConverterCache.AddConverter<CountryDto>(new InternalDtoConverter<CountryDto>());
        }

        private static void ValidateHeaders(string firstLine, CsvReaderOptionsConfig options)
        {
            if (string.IsNullOrWhiteSpace(firstLine))
                throw new CsvReadException(CsvErrorType.MissingAllHeaders, $"{ERRORMESSAGE}: {CsvErrorType.MissingAllHeaders}", null);

            var headers = firstLine.Split(options.Delimiter);
            var duplicates = headers.GroupBy(h => h.Trim())
                                    .Where(g => g.Count() > 1)
                                    .Select(g => g.Key);

            if (duplicates.Any())
            {
                var message = $"{ERRORMESSAGE}: {CsvErrorType.DuplicateHeader}\rDuplicati: {string.Join(options.Delimiter, duplicates)}";
                throw new CsvReadException(CsvErrorType.DuplicateHeader, message, null);
            }
        }

        private static CsvReadException MapCsvException(Exception ex)
        {
            var errorType = ex switch
            {
                CsvHelper.MissingFieldException => CsvErrorType.MissingField,
                TypeConverterException => CsvErrorType.TypeConversion,
                HeaderValidationException => CsvErrorType.MissingHeader,
                CsvReadException => ((CsvReadException)ex).ErrorType,
                ArgumentException argEx when argEx.Message.Contains("encoding", StringComparison.OrdinalIgnoreCase) => CsvErrorType.EncodingError,
                _ => CsvErrorType.Generic
            };

            var message = $"{ERRORMESSAGE}: {errorType}";
            return new CsvReadException(errorType, message, ex);
        }

        private static CsvConfiguration GetCsvConfigFromCsvOpt(CsvReaderOptionsConfig options)
        {
            return new CsvConfiguration(options.Culture)
            {
                Delimiter = options.Delimiter.ToString(),
                HasHeaderRecord = options.HasHeaderRecord,
                IgnoreBlankLines = options.IgnoreBlankLines,
                AllowComments = options.AllowComments
            };
        }

        private static (string header, Stream restStream) ReadHeaderPreserveStream(Stream input, string encodingName)
        {
            var encoding = Encoding.GetEncoding(encodingName);
            var decoder = encoding.GetDecoder();

            var charBuffer = new char[1];
            var byteBuffer = new byte[1];

            var headerText = new StringBuilder();

            bool foundNewline = false;
            var readBuffer = new List<byte>();

            while (!foundNewline)
            {
                int read = input.Read(byteBuffer, 0, 1);
                if (read == 0)
                    break;

                readBuffer.Add(byteBuffer[0]);

                int charsDecoded = decoder.GetChars(byteBuffer, 0, 1, charBuffer, 0);
                if (charsDecoded > 0)
                {
                    char c = charBuffer[0];
                    headerText.Append(c);
                    if (c == '\n')
                        foundNewline = true;
                }
            }

            if (headerText.Length == 0)
                throw new CsvReadException(CsvErrorType.MissingAllHeaders, $"{ERRORMESSAGE}: {CsvErrorType.MissingAllHeaders}", null);

            // Header come stringa
            var headerString = headerText.ToString().TrimEnd('\r', '\n');

            // Prepara uno stream che ri-espone i byte letti + il resto
            var preambleStream = new MemoryStream([.. readBuffer]);
            var combinedStream = new ConcatenatedStream(preambleStream, input, leaveOpen: true);

            return (headerString, combinedStream);
        }
    }
}
