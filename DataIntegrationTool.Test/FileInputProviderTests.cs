using System.Text;
using CsvHelper.Configuration.Attributes;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Infrastructure.InputProviders;
using Moq;
using FluentAssertions;
using DataIntegrationTool.Infrastructure.Exceptions;
using static DataIntegrationTool.Shared.Utils.CsvEnums;

namespace DataIntegrationTool.Tests.Infrastucture
{
    public class FileInputProviderTests
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
            var fakeData = new List<DummyDto>
            {
                new() { Value = "A" },
                new() { Value = "B" }
            };

            var mockFileReader = new Mock<IFileReader>();
            mockFileReader
                .Setup(r => r.OpenRead(It.IsAny<string>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("some,data")));
            mockFileReader
                .Setup(r => r.Exists(It.IsAny<string>()))
                .Returns(true);

            var mockCsvService = new Mock<ICsvReaderService>();
            mockCsvService
                .Setup(x => x.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    It.IsAny<CsvReaderOptionsConfig>(),
                    It.IsAny<string>()))
                .ReturnsAsync(fakeData);

            var provider = new FileInputProvider(mockCsvService.Object, mockFileReader.Object)
                .WithConfig(new InputSourceConfig
                {
                    FilePath = "fake.csv",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act
            var result = await provider.CreateObjectFromInputAsync<DummyDto>();

            // Assert
            result.Should().BeEquivalentTo(fakeData);
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_FileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var mockCsvService = new Mock<ICsvReaderService>();

            var mockFileReader = new Mock<IFileReader>();
            mockFileReader
                .Setup(r => r.OpenRead(It.IsAny<string>()))
                .Throws<FileNotFoundException>();

            var provider = new FileInputProvider(mockCsvService.Object, mockFileReader.Object)
                .WithConfig(new InputSourceConfig
                {
                    FilePath = "nonexistent.csv",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(provider.CreateObjectFromInputAsync<DummyDto>);
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_CsvServiceThrows_ShouldPropagateException()
        {
            var fakeStream = new MemoryStream(Encoding.UTF8.GetBytes("invalid,data"));

            var mockFileReader = new Mock<IFileReader>();
            mockFileReader.Setup(r => r.OpenRead(It.IsAny<string>())).Returns(fakeStream);
            mockFileReader.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);

            // Arrange
            var mockCsvService = new Mock<ICsvReaderService>();
            mockCsvService
                .Setup(x => x.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    It.IsAny<CsvReaderOptionsConfig>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new InvalidDataException("CSV malformed"));

            var provider = new FileInputProvider(mockCsvService.Object, mockFileReader.Object)
                .WithConfig(new InputSourceConfig
                {
                    FilePath = "fake.csv",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = Encoding.UTF8.WebName
                });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(provider.CreateObjectFromInputAsync<DummyDto>);
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenFilePathIsNull()
        {
            // Arrange
            var mockCsvService = new Mock<ICsvReaderService>(); 
            var mockFileReader = new Mock<IFileReader>();
            mockFileReader.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            var provider = new FileInputProvider(mockCsvService.Object, mockFileReader.Object)
                .WithConfig(new InputSourceConfig
                {
                    FilePath = null!,
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
            var mockCsvService = new Mock<ICsvReaderService>();
            mockCsvService
                .Setup(x => x.ReadCsvAsync<DummyDto>(It.IsAny<Stream>(), It.IsAny<CsvReaderOptionsConfig>(), "invalid-encoding"))
                .ThrowsAsync(new CsvReadException(
                            CsvErrorType.EncodingError,
                            "CSV error: EncodingError",
                            new ArgumentException("Encoding 'invalid-encoding' is not supported.")));

            var mockFileReader = new Mock<IFileReader>();

            var fakeStream = new MemoryStream(Encoding.UTF8.GetBytes("header1,header2\nvalue1,value2"));
            mockFileReader.Setup(r => r.OpenRead(It.IsAny<string>())).Returns(fakeStream);
            mockFileReader.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);

            var provider = new FileInputProvider(mockCsvService.Object, mockFileReader.Object)
                .WithConfig(new InputSourceConfig
                {
                    FilePath = "fake.csv",
                    Options = new CsvReaderOptionsConfig(),
                    Encoding = "invalid-encoding"
                });

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<CsvReadException>()
                     .Where(ex => ex.ErrorType == CsvErrorType.EncodingError
                                 && ex.Message.Contains("EncodingError"));
        }

        [Fact]
        public void WithConfig_ShouldSetConfigCorrectly()
        {
            // Arrange
            var config = new InputSourceConfig
            {
                FilePath = "test.csv",
                Encoding = Encoding.UTF8.WebName,
                Options = new CsvReaderOptionsConfig()
            };

            var mockCsvService = new Mock<ICsvReaderService>();
            var mockFileReader = new Mock<IFileReader>();

            var provider = new FileInputProvider(mockCsvService.Object, mockFileReader.Object);

            // Act
            var result = provider.WithConfig(config);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<FileInputProvider>();
        }
    }
}
