using DataIntegrationTool.Application.DataValidation;
using DataIntegrationTool.Application.Interfaces;

namespace DataIntegrationTool.Tests.Application
{
    public class ValidatableDto : IValidatable
    {
        public bool ShouldBeValid { get; set; } = true;

        public ValidationResult Validate()
        {
            var result = new ValidationResult();
            if (!ShouldBeValid)
            {
                result.AddError(nameof(ShouldBeValid), "Invalid");
            }
            return result;
        }
    }

    public class NestedValidatable
    {
        public string? Text { get; set; }
    }

    public class TestDtoVal
    {
        public string? Name { get; set; }
        public ValidatableDto? Validatable { get; set; }
        public List<NestedValidatable?>? NestedList { get; set; }
        public NestedValidatable? NestedObject { get; set; }
    }

    public class DefaultDataValidatorTests
    {
        private readonly DefaultDataValidator<TestDtoVal> _validator = new();

        [Fact]
        public void Validate_NullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _validator.Validate((IEnumerable<TestDtoVal>)null!));
        }

        [Fact]
        public void Validate_SingleValidObject_ReturnsValid()
        {
            var dto = new TestDtoVal { Name = "Hello" };
            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_SingleInvalidValidatableObject_ReturnsInvalid()
        {
            var dto = new TestDtoVal
            {
                Validatable = new ValidatableDto { ShouldBeValid = false }
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains("Validatable.ShouldBeValid", result.Errors.Keys);
        }

        [Fact]
        public void Validate_NestedList_ValidatesEachItem()
        {
            var list = new List<NestedValidatable>
        {
            new() { Text = "foo" },
            new() { Text = "bar" }
        };

            var dto = new TestDtoVal { NestedList = list };
            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_NestedObject_ValidatesNested()
        {
            var dto = new TestDtoVal
            {
                NestedObject = new NestedValidatable { Text = "nested" }
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_MultipleItems_MixedValidAndInvalid()
        {
            var valid = new TestDtoVal { Name = "ok" };
            var invalid = new TestDtoVal { Validatable = new ValidatableDto { ShouldBeValid = false } };

            var list = new List<TestDtoVal> { valid, invalid };

            var report = _validator.Validate(list);

            Assert.Single(report.ValidItems);
            Assert.Single(report.InvalidItems);

            var (Item, Result) = report.InvalidItems.First();
            Assert.Same(invalid, Item);
            Assert.False(Result.IsValid);
        }

        [Fact]
        public void Validate_ClassWithNullProperties_DoesNotThrow()
        {
            var dto = new TestDtoVal();
            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_ObjectWithNonSystemClass_IsRecursivelyValidated()
        {
            var dto = new TestDtoVal
            {
                NestedObject = new NestedValidatable { Text = "test" }
            };

            var result = _validator.Validate(dto);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_ListWithNullItem_IgnoresNull()
        {
            var dto = new TestDtoVal
            {
                NestedList = [null, new NestedValidatable { Text = "ok" }!]
            };

            var result = _validator.Validate(dto);
            Assert.True(result.IsValid);
        }
    }
}
