using CsvHelper.Configuration.Attributes;
using DataIntegrationTool.Application.DTOs;
using DataIntegrationTool.Shared.Utils;

namespace DataIntegrationTool.Application.Models
{
    public class CustomerRaw
    {
        public required NameDto FirstName { get; set; }

        public required NameDto LastName { get; set; }

        public required EmailDto Email { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public string? Company { get; set; }

        [TypeConverter(typeof(EmptyStringToNullConverter))]
        public string? JobTitle { get; set; }

        public PhoneDto? Phone { get; set; }

        public CountryDto? Country { get; set; }
    }
}
