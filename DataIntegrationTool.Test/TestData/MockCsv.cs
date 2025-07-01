namespace DataIntegrationTool.Test.TestData
{
    internal static class MockCsv
    {
        public static string CustomersTest = @"FirstName,LastName,Email,Company,JobTitle,Phone,Country
Anna,Test,anna.test@example.com,TestCorp,Tester,+000111222,Italy
Marco,Demo,marco.demo@example.com,Demo Inc,Developer,+333444555,Spain";
        public const string CustomerEmpty = @"FirstName,LastName,Email,Company,JobTitle,Phone,Country";
        public static string CustomerMissingAllHeaders = @"";
        public static string CustomersOptionalFieldsMissing = @"FirstName,LastName,Email,Company,JobTitle,Phone,Country
Laura,Verdi,laura.verdi@example.com,,,,";
        public static string CustomersMissingHeaders = @"FirstName,LastName,Email
Anna,Test,anna.test@example.com
Marco,Demo,marco.demo@example.com";
        public static string CustomerDuplicateHeader = @"FirstName,LastName,Email,Company,JobTitle,Phone,Phone,Country
Anna,Test,anna.test@example.com,TestCorp,Tester,+000111222,+000111222
Marco,Demo,marco.demo@example.com,Demo Inc,Developer,+333444555,+333444555";
        public static string CustomersInvalidFormat = @"FirstName,LastName,Email,Company,JobTitle,Phone,Country
Anna,Bianchi,12345,CompanyX,CEO,333444555,Italy";
        public static string CustomersMalformedRows = @"FirstName,LastName,Email,Company,JobTitle,Phone,Country
Anna,Test,anna.test@example.com,TestCorp,Tester,+000111222,Italy
Marco,Demo";
    }
}
