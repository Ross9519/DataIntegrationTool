namespace DataIntegrationTool.Models
{
    public class CustomerRaw
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
    }
}
