using System.Text.RegularExpressions;

namespace DataIntegrationTool.Shared.Utils
{
    public partial class RegexVault
    {
        [GeneratedRegex(@"\s{2,}")]
        public static partial Regex ExtraSpacesRegex();

        [GeneratedRegex(@"^[a-z0-9._\-+]+@[a-z0-9.\-]+\.[a-z]{2,}$")]
        public static partial Regex EmailSimpleValidationRegex();

        [GeneratedRegex(@"[\s\-\(\)]")]
        public static partial Regex RemoveCharInPhoneRegex();

        [GeneratedRegex(@"^\+(?<prefix>\d{1,4})(?<number>\d{6,15})$")]
        public static partial Regex PrefixFromNumberSeparationRegex();

        [GeneratedRegex(@"\.{2,}")]
        public static partial Regex RemoveDoubleDotsRegex();

        [GeneratedRegex(@"\s+")]
        public static partial Regex RemoveInternalSpacesRegex();

        [GeneratedRegex(@"[^a-z0-9@._\-+]")]
        public static partial Regex RemoveNotEmailFriendlyCharRegex();
    }


}
