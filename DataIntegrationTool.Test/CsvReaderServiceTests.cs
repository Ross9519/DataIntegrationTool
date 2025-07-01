using DataIntegrationTool.Services;
using DataIntegrationTool.Models;
using DataIntegrationTool.CustomExceptions;
using DataIntegrationTool.Test.TestData;
namespace DataIntegrationTool.Test
{
    public class CsvReaderServiceTests
    {
        private const string FILENOTFOUNDEXCEPTION = "FileNotFound";
        private const string MISSINGFIELDEXCEPTION = "MissingField";
        private const string HEADERVALIDATIONEXCEPTION = "HeaderValidation";
        private const string MISSINGHEADEREXCEPTION = "MissingHeader";
        private const string MISSINGALLHEADERSEXCEPTION = "MissingAllHeaders";
        private const string DUPLICATEHEADEREXCEPTION = "DuplicateHeader";

        [Fact]
        public async Task HandleContentAsync_ReturnsCorrectNumberOfRecords()
        {
            // Arrange
            var expectedCount = 2;

            // Act
            var result = (await new CsvReaderService().HandleContentAsync<CustomerRaw>(MockCsv.CustomersTest)).ToList();

            // Assert
            Assert.Equal(expectedCount, result.Count);
            var customer = result[0];
            Assert.Equal("Anna", customer.FirstName);
            Assert.Equal("Test", customer.LastName);
            Assert.Equal("anna.test@example.com", customer.Email);
        }

        [Fact]
        public async Task HandleContentAsync_ReturnsEmptyList()
        {
            // Act
            var result = (await new CsvReaderService().HandleContentAsync<CustomerRaw>(MockCsv.CustomerEmpty)).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task HandleFilePathAsync_WrongFilePath_ThrowsException()
        {
            // Arrange
            var testCsv = "TestData/not_existent_file.csv";

            // Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                await new CsvReaderService().HandleFilePathAsync<CustomerRaw>(testCsv);
            });

            Assert.Equal(CsvErrorType.FileNotFound, ex.ErrorType);
            Assert.Contains(FILENOTFOUNDEXCEPTION, ex.Message);
        }

        [Fact]
        public async Task HandleContentAsync_OptionalFieldsMissing_ParsesSuccessfully()
        {
            //Assert
            var result = (await new CsvReaderService().HandleContentAsync<CustomerRaw>(MockCsv.CustomersOptionalFieldsMissing)).ToList();

            // Assert
            Assert.Single(result);
            var customer = result[0];
            Assert.Equal("Laura", customer.FirstName);
            Assert.Equal("Verdi", customer.LastName);
            Assert.Equal("laura.verdi@example.com", customer.Email);
            Assert.Null(customer.Company);
            Assert.Null(customer.JobTitle);
            Assert.Null(customer.Phone);
            Assert.Null(customer.Country);
        }

        [Fact]
        public async Task HandleContentAsync_MissingHeader_ThrowsException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                var result = await new CsvReaderService().HandleContentAsync<CustomerRaw>(MockCsv.CustomersMissingHeaders);
            });

            Assert.Equal(CsvErrorType.MissingHeader, ex.ErrorType);
            Assert.Contains(MISSINGHEADEREXCEPTION, ex.Message);
        }

        [Fact]
        public async Task HandleContentAsync_MissingAllHeaders_ThrowsException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                var result = await new CsvReaderService().HandleContentAsync<CustomerRaw>(MockCsv.CustomerMissingAllHeaders);
            });

            Assert.Equal(CsvErrorType.MissingAllHeaders, ex.ErrorType);
            Assert.Contains(MISSINGALLHEADERSEXCEPTION, ex.Message);
        }

        [Fact]
        public async Task HandleContentAsync_DuplicateHeader_ThrowsExceptionWithCorrectErrorType()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                await new CsvReaderService().HandleContentAsync<CustomerRaw>(MockCsv.CustomerDuplicateHeader);
            });

            Assert.Equal(CsvErrorType.DuplicateHeader, ex.ErrorType);
            Assert.Contains(DUPLICATEHEADEREXCEPTION, ex.Message);
        }

        [Fact]
        public async Task HandleContentAsync_MalformedRows_ThrowsException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                var result = await new CsvReaderService().HandleContentAsync<CustomerRaw>(MockCsv.CustomersMalformedRows);
            });

            Assert.Equal(CsvErrorType.MissingField, ex.ErrorType);
            Assert.Contains(MISSINGFIELDEXCEPTION, ex.Message);
        }
    }
}