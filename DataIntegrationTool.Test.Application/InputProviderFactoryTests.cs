using DataIntegrationTool.Application.Config;
using DataIntegrationTool.Application.Factories;
using DataIntegrationTool.Application.Interfaces;
using Moq;
using static DataIntegrationTool.Shared.Utils.CsvEnums;

namespace DataIntegrationTool.Tests.Application
{
    public class InputProviderFactoryTests
    {
        [Fact]
        public void Create_WithNullConfig_ShouldThrowArgumentNullException()
        {
            // Arrange
            var factory = new InputProviderFactory(new Dictionary<InputType, InputProviderResolver>());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.Create(null!));
        }

        [Fact]
        public void Create_WithUnsupportedInputType_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var resolvers = new Dictionary<InputType, InputProviderResolver>();
            var factory = new InputProviderFactory(resolvers);

            var config = new InputSourceConfig
            {
                Type = (InputType)999, // tipo non registrato
                FilePath = "file.csv"
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => factory.Create(config));
        }

        [Fact]
        public void Create_WithSupportedInputType_ShouldReturnProvider()
        {
            // Arrange
            var expectedProvider = Mock.Of<IInputProvider>();
            var inputType = InputType.File; // o quello che usi
            var resolvers = new Dictionary<InputType, InputProviderResolver>
            {
                [inputType] = config =>
                {
                    Assert.NotNull(config);
                    return expectedProvider;
                }
            };
            var factory = new InputProviderFactory(resolvers);
            var config = new InputSourceConfig { Type = inputType };

            // Act
            var result = factory.Create(config);

            // Assert
            Assert.Same(expectedProvider, result);
        }
    }
}
