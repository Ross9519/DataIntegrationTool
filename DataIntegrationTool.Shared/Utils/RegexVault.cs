using System.Text.RegularExpressions;

namespace DataIntegrationTool.Shared.Utils
{
    public partial class RegexVault
    {
        [GeneratedRegex(@"\s{2,}")]
        public static partial Regex ExtraSpacesRegex();

        [GeneratedRegex(@"^[a-z0-9._\-+]+@[a-z0-9.\-]+\.[a-z]{2,}$")]
        public static partial Regex EmailSimpleValidationRegex();

        [GeneratedRegex(@"[^\d\+]")]
        public static partial Regex RemoveCharInPhoneRegex();

        [GeneratedRegex(@"^\+(?<prefix>\d{1,4})(?<number>\d{6,15})$")]
        public static partial Regex PrefixFromNumberSeparationRegex();

        [GeneratedRegex(@"\.{2,}")]
        public static partial Regex RemoveDoubleDotsRegex();

        [GeneratedRegex(@"\s+")]
        public static partial Regex RemoveInternalSpacesRegex();

        [GeneratedRegex(@"[^a-z0-9@._\-+]")]
        public static partial Regex RemoveNotEmailFriendlyCharRegex();

        [GeneratedRegex(@"^[\p{L} \-']+$")]
        public static partial Regex InvalidCharNameRegex();

        [GeneratedRegex(@"^\+\d{6,15}$")]
        public static partial Regex PhoneValidationRegex();

        [GeneratedRegex(
            @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" +
            @"([-a-zA-Z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)+)(?<!\.)" +
            @"@[a-zA-Z0-9][\w\.-]*\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled)]
        public static partial Regex EmailValidationRegex();
    }


}
