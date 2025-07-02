using CsvHelper.Configuration.Attributes;
using DataIntegrationTool.Utils;

namespace DataIntegrationTool.Models
{
    public class CustomerRaw
    {
        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public required string FirstName { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public required string LastName { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public required string Email { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public string? Company { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public string? JobTitle { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public string? Phone { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public string? Country { get; set; }
    }
}
