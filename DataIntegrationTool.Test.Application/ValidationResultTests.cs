using DataIntegrationTool.Application.DataValidation;

namespace DataIntegrationTool.Tests.Application
{
    public class ValidationResultTests
    {
        [Fact]
        public void IsValid_ReturnsTrue_WhenNoErrors()
        {
            var result = new ValidationResult();
            Assert.True(result.IsValid);
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenHasErrors()
        {
            var result = new ValidationResult();
            result.AddError("Field", "Error!");
            Assert.False(result.IsValid);
        }

        [Fact]
        public void HasWarnings_ReturnsTrue_WhenWarningsExist()
        {
            var result = new ValidationResult();
            result.AddWarning("Field", "Be careful");
            Assert.True(result.HasWarnings);
        }

        [Fact]
        public void AddError_DoesNotOverwriteExisting()
        {
            var result = new ValidationResult();
            result.AddError("Field", "First");
            result.AddError("Field", "Second");
            Assert.Equal("First", result.Errors["Field"]);
        }

        [Fact]
        public void AddWarning_DoesNotOverwriteExisting()
        {
            var result = new ValidationResult();
            result.AddWarning("Field", "First");
            result.AddWarning("Field", "Second");
            Assert.Equal("First", result.Warnings["Field"]);
        }

        [Fact]
        public void Add_NestedValidationResult_PrefixesKeys()
        {
            var nested = new ValidationResult();
            nested.AddError("Name", "Required");
            nested.AddWarning("Age", "Unusual");

            var parent = new ValidationResult();
            parent.Add("Child", nested);

            Assert.Contains("Child.Name", parent.Errors.Keys);
            Assert.Contains("Child.Age", parent.Warnings.Keys);
        }

        [Fact]
        public void ToString_FormatsErrorsAndWarnings()
        {
            var result = new ValidationResult();
            result.AddError("Name", "Required");
            result.AddWarning("Age", "Optional");

            var text = result.ToString();
            Assert.Contains("Errors:", text);
            Assert.Contains("Warnings:", text);
            Assert.Contains("Name", text);
            Assert.Contains("Age", text);
        }
    }
}
