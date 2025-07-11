using System.Data.Common;
using System.Text;
using CsvHelper.Configuration.Attributes;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Factories;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Infrastructure.InputProviders;
using FluentAssertions;
using Moq;

namespace DataIntegrationTool.Tests.Infrastucture
{
    public class DatabaseInputProviderTests
    {
        public class DummyDto
        {
            [Name("Value")]
            public string Value { get; set; } = string.Empty;
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldReturnParsedObjects()
        {
            // Arrange
            var expected = new List<DummyDto>
            {
                new() { Value = "A" },
                new() { Value = "B" }
            };

            var mockReader = new Mock<DbDataReader>();

            var mockDbAccessor = new Mock<IDatabaseAccessor>();
            mockDbAccessor
                .Setup(d => d.ExecuteReaderAsync(It.IsAny<string>(), default))
                .ReturnsAsync(mockReader.Object);

            var mockCsvService = new Mock<ICsvReaderService>();
            mockCsvService
                .Setup(s => s.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    It.IsAny<CsvReaderOptionsConfig>(),
                    It.IsAny<string>()))
                .ReturnsAsync(expected);

            var provider = new DatabaseInputProvider(mockCsvService.Object, mockDbAccessor.Object)
                .WithConfig(new InputSourceConfig
                {
                    ConnectionString = "fake-conn",
                    Query = "SELECT * FROM Table",
                    Options = new CsvReaderOptionsConfig { Delimiter = ',' },
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            var result = await provider.CreateObjectFromInputAsync<DummyDto>();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenConnectionStringIsNull()
        {
            var mockDb = new Mock<IDatabaseAccessor>();

            var provider = new DatabaseInputProvider(Mock.Of<ICsvReaderService>(), mockDb.Object)
                .WithConfig(new InputSourceConfig
                {
                    ConnectionString = null!,
                    Query = "SELECT *",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("ConnectionString");
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenQueryIsNull()
        {
            var mockDb = new Mock<IDatabaseAccessor>();

            var provider = new DatabaseInputProvider(Mock.Of<ICsvReaderService>(), mockDb.Object)
                .WithConfig(new InputSourceConfig
                {
                    ConnectionString = "some-conn",
                    Query = null!,
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("Query");
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenEncodingIsInvalid()
        {
            var mockDb = new Mock<IDatabaseAccessor>();

            // Setup: non serve che venga chiamato, ma evitiamo NullReference
            mockDb.Setup(d => d.ExecuteReaderAsync(It.IsAny<string>(), default))
                  .ReturnsAsync(Mock.Of<DbDataReader>());

            var provider = new DatabaseInputProvider(Mock.Of<ICsvReaderService>(), mockDb.Object)
                .WithConfig(new InputSourceConfig
                {
                    ConnectionString = "some-conn",
                    Query = "SELECT *",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = "invalid-encoding"
                });

            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*'invalid-encoding'*");
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldPropagate_WhenDbFails()
        {
            // Arrange
            var dbAccessor = new Mock<IDatabaseAccessor>();
            dbAccessor.Setup(d => d.ExecuteReaderAsync(It.IsAny<string>(), default))
                      .ThrowsAsync(new InvalidOperationException("DB failure"));

            var provider = new DatabaseInputProvider(Mock.Of<ICsvReaderService>(), dbAccessor.Object)
                .WithConfig(new InputSourceConfig
                {
                    ConnectionString = "bad-conn",
                    Query = "SELECT *",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*DB failure*");
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldPropagate_WhenCsvParsingFails()
        {
            // Arrange
            var mockCsv = new Mock<ICsvReaderService>();
            mockCsv
                .Setup(s => s.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    It.IsAny<CsvReaderOptionsConfig>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new FormatException("CSV error"));

            var mockDbAccessor = new Mock<IDatabaseAccessor>();
            mockDbAccessor
                .Setup(d => d.ExecuteReaderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new FormatException("CSV error"));

            var provider = new DatabaseInputProvider(mockCsv.Object, mockDbAccessor.Object)
                .WithConfig(new InputSourceConfig
                {
                    ConnectionString = "some-connection-string",
                    Query = "SELECT * FROM table",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<FormatException>()
                .WithMessage("*CSV error*");

            // Cleanup: reset factory per evitare side effect su altri test
            SqlConnectionFactory.ResetFactory();
        }

        [Fact]
        public void WithConfig_ShouldAssignConfiguration()
        {
            var config = new InputSourceConfig
            {
                ConnectionString = "x",
                Query = "y",
                Options = new CsvReaderOptionsConfig(),
                Encoding = Encoding.UTF8.WebName
            };

            var provider = new DatabaseInputProvider(Mock.Of<ICsvReaderService>(), Mock.Of<IDatabaseAccessor>());
            var result = provider.WithConfig(config);

            result.Should().BeSameAs(provider);
        }
    }
}
