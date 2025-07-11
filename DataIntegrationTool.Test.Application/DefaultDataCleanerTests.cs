using DataIntegrationTool.Application.DataCleaning;
using DataIntegrationTool.Application.Interfaces;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Tests.Application
{
    public class CleanableDto : ICleanable
    {
        public bool WasCleaned { get; private set; } = false;
        public void Clean() => WasCleaned = true;
    }

    public class NestedDto
    {
        public string? Text { get; set; }
    }

    public class TestDto
    {
        public string? Name { get; set; }
        public CleanableDto? Cleanable { get; set; }
        public List<NestedDto>? NestedList { get; set; }
        public NestedDto? NestedObject { get; set; }
    }

    public class DefaultDataCleanerTests
    {
        private readonly DefaultDataCleaner<TestDto> _cleaner = new();

        [Fact]
        public void Clean_SingleNull_ThrowsArgumentNullException()
        {
            TestDto? cleanableDto = null;
            Assert.Throws<ArgumentNullException>(() => _cleaner.Clean(cleanableDto!));
        }

        [Fact]
        public void Clean_String_TrimsAndCleans()
        {
            var dto = new TestDto { Name = "  hello   world  " };
            _cleaner.Clean(dto);
            Assert.Equal("hello world", dto.Name);
        }

        [Fact]
        public void Clean_ICleanable_CallsClean()
        {
            var cleanable = new CleanableDto();
            var dto = new TestDto { Cleanable = cleanable };
            _cleaner.Clean(dto);
            Assert.True(cleanable.WasCleaned);
        }

        [Fact]
        public void Clean_NullProperties_DoesNotThrow()
        {
            var dto = new TestDto
            {
                Name = null,
                Cleanable = null,
                NestedList = null,
                NestedObject = null
            };

            _cleaner.Clean(dto);
        }

        [Fact]
        public void Clean_Enumerable_CleansAllItems()
        {
            var nested1 = new NestedDto { Text = "  foo  " };
            var nested2 = new NestedDto { Text = " bar\t " };

            var dto = new TestDto
            {
                NestedList = [nested1, nested2]
            };

            _cleaner.Clean(dto);

            Assert.Equal("foo", nested1.Text);
            Assert.Equal("bar", nested2.Text);
        }

        [Fact]
        public void Clean_NestedObject_CleansNestedProperties()
        {
            var nested = new NestedDto { Text = "  baz " };
            var dto = new TestDto { NestedObject = nested };
            _cleaner.Clean(dto);
            Assert.Equal("baz", nested.Text);
        }

        [Fact]
        public void Clean_MultipleItems_RemovesDuplicates()
        {
            var dto1 = new TestDto { Name = "John" };
            var dto2 = new TestDto { Name = "John" }; // duplicate by Name property
            var dto3 = new TestDto { Name = "Jane" };

            var list = new List<TestDto> { dto1, dto2, dto3 };

            var distinct = DataCleaningHelper.RemoveDuplicates(list).ToList();

            Assert.Contains(dto1, distinct);
            Assert.Contains(dto3, distinct);
            Assert.DoesNotContain(dto2, distinct);
            Assert.Equal(2, distinct.Count);
        }
    }
}
