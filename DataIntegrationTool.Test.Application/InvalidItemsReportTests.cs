using DataIntegrationTool.Application.DataValidation;

namespace DataIntegrationTool.Tests.Application
{
    public class InvalidItemsReportTests
    {
        [Fact]
        public void Report_WithAllValidItems_HasEmptyInvalidItems()
        {
            var report = new InvalidItemsReport<string>
            {
                ValidItems = ["A", "B", "C"],
                InvalidItems = []
            };

            Assert.Equal(3, report.ValidItems.Count());
            Assert.Empty(report.InvalidItems);
        }

        [Fact]
        public void Report_WithInvalidItems_ContainsItemAndResult()
        {
            var result = new ValidationResult();
            result.AddError("Field", "Invalid");

            var report = new InvalidItemsReport<string>
            {
                ValidItems = ["A"],
                InvalidItems = [("B", result)]
            };

            Assert.Single(report.ValidItems);
            Assert.Single(report.InvalidItems);

            var invalid = report.InvalidItems.First();
            Assert.Equal("B", invalid.Item);
            Assert.False(invalid.Result.IsValid);
        }

        [Fact]
        public void Report_Empty_IsValidStructure()
        {
            var report = new InvalidItemsReport<int>();

            Assert.Empty(report.ValidItems);
            Assert.Empty(report.InvalidItems);
        }
    }
}
