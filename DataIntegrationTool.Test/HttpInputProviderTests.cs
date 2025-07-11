using System.Net;
using System.Text;
using CsvHelper.Configuration.Attributes;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Infrastructure.InputProviders;
using FluentAssertions;
using Moq.Protected;
using Moq;

namespace DataIntegrationTool.Tests.Infrastucture
{
    public class HttpInputProviderTests
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
            var csvContent = "Value\nA\nB";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var expectedData = new List<DummyDto>
            {
                new() { Value = "A" },
                new() { Value = "B" }
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csvContent)))
                });

            var mockCsvService = new Mock<ICsvReaderService>();
            mockCsvService
                .Setup(x => x.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    It.IsAny<CsvReaderOptionsConfig>(),
                    It.IsAny<string>()))
                .ReturnsAsync(expectedData);

            var client = new HttpClient(handler.Object);

            var provider = new HttpInputProvider(mockCsvService.Object, client)
                .WithConfig(new InputSourceConfig
                {
                    Url = "http://fake.url/data.csv",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            var result = await provider.CreateObjectFromInputAsync<DummyDto>();

            // Assert
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenUrlIsNull()
        {
            // Arrange
            var provider = new HttpInputProvider(Mock.Of<ICsvReaderService>())
                .WithConfig(new InputSourceConfig
                {
                    Url = null!,
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenEncodingIsInvalid()
        {
            // Arrange
            var provider = new HttpInputProvider(Mock.Of<ICsvReaderService>())
                .WithConfig(new InputSourceConfig
                {
                    Url = "http://fake.url/data.csv",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = "invalid-encoding"
                });

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenHttpRequestFails()
        {
            // Arrange
            var invalidUrl = "http://nonexistent.url/data.csv";
            var provider = new HttpInputProvider(Mock.Of<ICsvReaderService>())
                .WithConfig(new InputSourceConfig
                {
                    Url = invalidUrl,
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldPropagateException_FromCsvService()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("irrelevant content", Encoding.UTF8, "text/csv")
                });

            var mockCsvService = new Mock<ICsvReaderService>();
            mockCsvService
                .Setup(s => s.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    It.IsAny<CsvReaderOptionsConfig>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new InvalidDataException("Malformed CSV"));

            var client = new HttpClient(handler.Object);

            var provider = new HttpInputProvider(mockCsvService.Object, client)
                .WithConfig(new InputSourceConfig
                {
                    Url = "http://fake.url/data.csv",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<InvalidDataException>();
        }

        [Fact]
        public void WithConfig_ShouldSetConfigCorrectly()
        {
            // Arrange
            var config = new InputSourceConfig
            {
                Url = "http://fake.url",
                Encoding = Encoding.UTF8.WebName,
                Options = new CsvReaderOptionsConfig()
            };

            var provider = new HttpInputProvider(Mock.Of<ICsvReaderService>());

            // Act
            var result = provider.WithConfig(config);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<HttpInputProvider>();
        }
    }
}
