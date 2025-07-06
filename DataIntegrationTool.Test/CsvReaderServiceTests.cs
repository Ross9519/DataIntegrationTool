using DataIntegrationTool.Test.TestData;
using static DataIntegrationTool.Shared.Utils.CsvEnums;
using static DataIntegrationTool.Shared.Utils.Constants;
using System.Text;
using DataIntegrationTool.Infrastructure.Exceptions;
using DataIntegrationTool.Infrastructure.Services;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Models;
namespace DataIntegrationTool.Test
{
    public class CsvReaderServiceTests
    {
        private static readonly CsvReaderService reader = new();
        private static readonly CsvReaderOptionsConfig options = new();

        [Fact]
        public async Task ReadCsvAsync_ReturnsCorrectNumberOfRecords()
        {
            // Arrange
            var expectedCount = 2;

            // Act
            var result = (await reader.ReadCsvAsync<CustomerRaw>(ToStream(MockCsv.CustomersTest), options, "utf-8")).ToList();

            // Assert
            Assert.Equal(expectedCount, result.Count);
            var customer = result[0];
            Assert.Equal("Anna", customer.FirstName.Value);
            Assert.Equal("Test", customer.LastName.Value);
            Assert.Equal("anna.test@example.com", customer.Email.Value);
        }

        [Fact]
        public async Task ReadCsvAsync_ReturnsEmptyList()
        {
            // Act
            var result = (await reader.ReadCsvAsync<CustomerRaw>(ToStream(MockCsv.CustomerEmpty), options, "utf-8")).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ReadCsvAsync_OptionalFieldsMissing_ParsesSuccessfully()
        {
            //Assert
            var result = (await reader.ReadCsvAsync<CustomerRaw>(ToStream(MockCsv.CustomersOptionalFieldsMissing), options, "utf-8" )).ToList();

            // Assert
            Assert.Single(result);
            var customer = result[0];
            Assert.Equal("Laura", customer.FirstName.Value);
            Assert.Equal("Verdi", customer.LastName.Value);
            Assert.Equal("laura.verdi@example.com", customer.Email.Value);
            Assert.Null(customer.Company);
            Assert.Null(customer.JobTitle);
            Assert.Null(customer.Phone);
            Assert.Null(customer.Country);
        }

        [Fact]
        public async Task ReadCsvAsync_MissingHeader_ThrowsException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                var result = await reader.ReadCsvAsync<CustomerRaw>(ToStream(MockCsv.CustomersMissingHeaders), options, "utf-8");
            });

            Assert.Equal(CsvErrorType.MissingHeader, ex.ErrorType);
            Assert.Contains(MISSINGHEADEREXCEPTION, ex.Message);
        }

        [Fact]
        public async Task ReadCsvAsync_MissingAllHeaders_ThrowsException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                var result = await reader.ReadCsvAsync<CustomerRaw>(ToStream(MockCsv.CustomerMissingAllHeaders), options, "utf-8");
            });

            Assert.Equal(CsvErrorType.MissingAllHeaders, ex.ErrorType);
            Assert.Contains(MISSINGALLHEADERSEXCEPTION, ex.Message);
        }

        [Fact]
        public async Task ReadCsvAsync_DuplicateHeader_ThrowsExceptionWithCorrectErrorType()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                await reader.ReadCsvAsync<CustomerRaw>(ToStream(MockCsv.CustomerDuplicateHeader), options, "utf-8");
            });

            Assert.Equal(CsvErrorType.DuplicateHeader, ex.ErrorType);
            Assert.Contains(DUPLICATEHEADEREXCEPTION, ex.Message);
        }

        [Fact]
        public async Task ReadCsvAsync_MalformedRows_ThrowsException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<CsvReadException>(async () =>
            {
                var result = await reader.ReadCsvAsync<CustomerRaw>(ToStream(MockCsv.CustomersMalformedRows), options, "utf-8");
            });

            Assert.Equal(CsvErrorType.MissingField, ex.ErrorType);
            Assert.Contains(MISSINGFIELDEXCEPTION, ex.Message);
        }

        private static Stream ToStream(string content, string encoding = "utf-8")
        {
            return new MemoryStream(Encoding.GetEncoding(encoding).GetBytes(content));
        }
    }
}