using System.Text;
using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Infrastructure.InputProviders;
using FluentAssertions;
using Moq;

namespace DataIntegrationTool.Tests.Infrastucture
{
    public class StringInputProviderTests
    {
        public class DummyDto
        {
            public string? Value { get; set; }
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldParseCsvStringCorrectly()
        {
            // Arrange
            var csvContent = "Value\nTestValue";
            var encodingName = "utf-8";

            var config = new InputSourceConfig
            {
                CsvStringContent = csvContent,
                Encoding = encodingName,
                Options = new CsvReaderOptionsConfig() // se hai opzioni speciali, settale qui
            };

            var expectedBytes = Encoding.UTF8.GetBytes(csvContent);

            var mockService = new Mock<ICsvReaderService>();

            mockService.Setup(s => s.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    config.Options,
                    encodingName))
                .ReturnsAsync(
                [
                new DummyDto { Value = "TestValue" }
                ]);

            var provider = new StringInputProvider(mockService.Object).WithConfig(config);

            // Act
            var result = await provider.CreateObjectFromInputAsync<DummyDto>();

            // Assert
            result.Should().HaveCount(1);
            result.Should().ContainSingle(x => x.Value == "TestValue");
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldReturnEmptyList_WhenCsvHasOnlyHeaders()
        {
            // Arrange
            var csvContent = "Value\n";
            var encodingName = "utf-8";
            var config = new InputSourceConfig
            {
                CsvStringContent = csvContent,
                Encoding = encodingName,
                Options = new CsvReaderOptionsConfig()
            };

            var mockService = new Mock<ICsvReaderService>();
            mockService
                .Setup(s => s.ReadCsvAsync<DummyDto>(
                    It.IsAny<Stream>(),
                    config.Options,
                    encodingName))
                .ReturnsAsync([]);

            var provider = new StringInputProvider(mockService.Object).WithConfig(config);

            // Act
            var result = await provider.CreateObjectFromInputAsync<DummyDto>();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenCsvContentIsNull()
        {
            // Arrange
            var config = new InputSourceConfig
            {
                CsvStringContent = null!,
                Encoding = "utf-8",
                Options = new CsvReaderOptionsConfig()
            };

            var provider = new StringInputProvider(Mock.Of<ICsvReaderService>()).WithConfig(config);

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldThrow_WhenEncodingIsInvalid()
        {
            // Arrange
            var config = new InputSourceConfig
            {
                CsvStringContent = "Value\nSomething",
                Encoding = "invalid-encoding",
                Options = new CsvReaderOptionsConfig()
            };

            var provider = new StringInputProvider(Mock.Of<ICsvReaderService>()).WithConfig(config);

            // Act
            Func<Task> act = provider.CreateObjectFromInputAsync<DummyDto>;

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                     .Where(e => e.Message.Contains("'invalid-encoding'") && e.Message.Contains("Encoding.RegisterProvider"));

        }

        [Fact]
        public void WithConfig_ShouldSetConfigurationCorrectly()
        {
            // Arrange
            var config = new InputSourceConfig
            {
                CsvStringContent = "Value\nTest",
                Encoding = "utf-8",
                Options = new CsvReaderOptionsConfig()
            };

            var provider = new StringInputProvider(Mock.Of<ICsvReaderService>());

            // Act
            var result = provider.WithConfig(config);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StringInputProvider>();
        }

        [Fact]
        public async Task CreateObjectFromInputAsync_ShouldPropagateException_WhenCsvServiceFails()
        {
            // Arrange
            var mockService = new Mock<ICsvReaderService>();
            mockService.Setup(s => s.ReadCsvAsync<DummyDto>(
                It.IsAny<Stream>(),
                It.IsAny<CsvReaderOptionsConfig>(),
                It.IsAny<string>()))
                .ThrowsAsync(new InvalidDataException("Malformed CSV"));

            var provider = new StringInputProvider(mockService.Object).WithConfig(new InputSourceConfig
            {
                CsvStringContent = "Value\nBadData",
                Encoding = "utf-8",
                Options = new CsvReaderOptionsConfig()
            });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(provider.CreateObjectFromInputAsync<DummyDto>);
        }
    }
}
