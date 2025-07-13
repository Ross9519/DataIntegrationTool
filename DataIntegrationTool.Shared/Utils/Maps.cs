namespace DataIntegrationTool.Shared.Utils
{
    public static class Maps
    {
        public static readonly Dictionary<string, string> CountryMap = new()
        {
            // Italy
            { "IT", "Italy" },
            { "ITA", "Italy" },
            { "ITALY", "Italy" },
            { "ITALIA", "Italy" },
            { "🇮🇹", "Italy" },

            // France
            { "FR", "France" },
            { "FRA", "France" },
            { "FRANCE", "France" },
            { "🇫🇷", "France" },

            // Germany
            { "DE", "Germany" },
            { "DEU", "Germany" },
            { "GER", "Germany" },
            { "GERMANY", "Germany" },
            { "🇩🇪", "Germany" },

            // Spain
            { "ES", "Spain" },
            { "ESP", "Spain" },
            { "SPAIN", "Spain" },
            { "🇪🇸", "Spain" },

            // United States
            { "US", "United States" },
            { "USA", "United States" },
            { "UNITED STATES", "United States" },
            { "UNITED STATES OF AMERICA", "United States" },
            { "🇺🇸", "United States" },

            // United Kingdom
            { "UK", "United Kingdom" },
            { "GB", "United Kingdom" },
            { "GBR", "United Kingdom" },
            { "GREAT BRITAIN", "United Kingdom" },
            { "ENGLAND", "United Kingdom" },
            { "🇬🇧", "United Kingdom" },

            // Canada
            { "CA", "Canada" },
            { "CAN", "Canada" },
            { "CANADA", "Canada" },
            { "🇨🇦", "Canada" },

            // Australia
            { "AU", "Australia" },
            { "AUS", "Australia" },
            { "AUSTRALIA", "Australia" },
            { "🇦🇺", "Australia" },

            // Russia
            { "RU", "Russia" },
            { "RUS", "Russia" },
            { "RUSSIA", "Russia" },
            { "🇷🇺", "Russia" },

            // China
            { "CN", "China" },
            { "CHN", "China" },
            { "CHINA", "China" },
            { "🇨🇳", "China" },

            // Japan
            { "JP", "Japan" },
            { "JPN", "Japan" },
            { "JAPAN", "Japan" },
            { "🇯🇵", "Japan" },

            // India
            { "IN", "India" },
            { "IND", "India" },
            { "INDIA", "India" },
            { "🇮🇳", "India" },

            // Brazil
            { "BR", "Brazil" },
            { "BRA", "Brazil" },
            { "BRAZIL", "Brazil" },
            { "🇧🇷", "Brazil" },

            // Mexico
            { "MX", "Mexico" },
            { "MEX", "Mexico" },
            { "MEXICO", "Mexico" },
            { "🇲🇽", "Mexico" },

            // South Africa
            { "ZA", "South Africa" },
            { "ZAF", "South Africa" },
            { "SOUTH AFRICA", "South Africa" },
            { "RSA", "South Africa" },
            { "🇿🇦", "South Africa" },

            // Sweden
            { "SE", "Sweden" },
            { "SWE", "Sweden" },
            { "SWEDEN", "Sweden" },
            { "🇸🇪", "Sweden" },

            // Norway
            { "NO", "Norway" },
            { "NOR", "Norway" },
            { "NORWAY", "Norway" },
            { "🇳🇴", "Norway" },

            // Denmark
            { "DK", "Denmark" },
            { "DNK", "Denmark" },
            { "DENMARK", "Denmark" },
            { "🇩🇰", "Denmark" },

            // Netherlands
            { "NL", "Netherlands" },
            { "NLD", "Netherlands" },
            { "HOLLAND", "Netherlands" },
            { "NETHERLANDS", "Netherlands" },
            { "🇳🇱", "Netherlands" },

            // Belgium
            { "BE", "Belgium" },
            { "BEL", "Belgium" },
            { "BELGIUM", "Belgium" },
            { "🇧🇪", "Belgium" },

            // Switzerland
            { "CH", "Switzerland" },
            { "CHE", "Switzerland" },
            { "SWITZERLAND", "Switzerland" },
            { "🇨🇭", "Switzerland" },

            // Austria
            { "AT", "Austria" },
            { "AUT", "Austria" },
            { "AUSTRIA", "Austria" },
            { "🇦🇹", "Austria" },

            // Poland
            { "PL", "Poland" },
            { "POL", "Poland" },
            { "POLAND", "Poland" },
            { "🇵🇱", "Poland" },

            // Portugal
            { "PT", "Portugal" },
            { "PRT", "Portugal" },
            { "PORTUGAL", "Portugal" },
            { "🇵🇹", "Portugal" },

            // Greece
            { "GR", "Greece" },
            { "GRC", "Greece" },
            { "GREECE", "Greece" },
            { "🇬🇷", "Greece" },

            // Ireland
            { "IE", "Ireland" },
            { "IRL", "Ireland" },
            { "IRELAND", "Ireland" },
            { "🇮🇪", "Ireland" },

            // Turkey
            { "TR", "Turkey" },
            { "TUR", "Turkey" },
            { "TURKEY", "Turkey" },
            { "🇹🇷", "Turkey" },

            // New Zealand
            { "NZ", "New Zealand" },
            { "NZL", "New Zealand" },
            { "NEW ZEALAND", "New Zealand" },
            { "🇳🇿", "New Zealand" },

            // South Korea
            { "KR", "South Korea" },
            { "KOR", "South Korea" },
            { "SOUTH KOREA", "South Korea" },
            { "🇰🇷", "South Korea" },

            // North Korea
            { "KP", "North Korea" },
            { "PRK", "North Korea" },
            { "NORTH KOREA", "North Korea" },
            { "🇰🇵", "North Korea" },

            // Argentina
            { "AR", "Argentina" },
            { "ARG", "Argentina" },
            { "ARGENTINA", "Argentina" },
            { "🇦🇷", "Argentina" },

            // Chile
            { "CL", "Chile" },
            { "CHL", "Chile" },
            { "CHILE", "Chile" },
            { "🇨🇱", "Chile" },

            // Colombia
            { "CO", "Colombia" },
            { "COL", "Colombia" },
            { "COLOMBIA", "Colombia" },
            { "🇨🇴", "Colombia" },

            // Egypt
            { "EG", "Egypt" },
            { "EGY", "Egypt" },
            { "EGYPT", "Egypt" },
            { "🇪🇬", "Egypt" },

            // Saudi Arabia
            { "SA", "Saudi Arabia" },
            { "SAU", "Saudi Arabia" },
            { "SAUDI ARABIA", "Saudi Arabia" },
            { "🇸🇦", "Saudi Arabia" },

            // United Arab Emirates
            { "AE", "United Arab Emirates" },
            { "ARE", "United Arab Emirates" },
            { "UNITED ARAB EMIRATES", "United Arab Emirates" },
            { "UAE", "United Arab Emirates" },
            { "🇦🇪", "United Arab Emirates" },

            // Israel
            { "IL", "Israel" },
            { "ISR", "Israel" },
            { "ISRAEL", "Israel" },
            { "🇮🇱", "Israel" },
        };

        public static readonly Dictionary<string, string> CountryToISO2Map = new()
        {
            { "Italy", "IT" },
            { "France", "FR" },
            { "Germany", "DE" },
            { "Spain", "ES" },
            { "United States", "US" },
            { "United Kingdom", "UK" },
            { "Canada", "CA" },
            { "Australia", "AU" },
            { "Russia", "RU" },
            { "China", "CN" },
            { "Japan", "JP" },
            { "India", "IN" },
            { "Brazil", "BR" },
            { "Mexico", "MX" },
            { "South Africa", "ZA" },
            { "Sweden", "SE" },
            { "Norway", "NO" },
            { "Denmark", "DK" },
            { "Netherlands", "NL" },
            { "Belgium", "BE" },
            { "Switzerland", "CH" },
            { "Austria", "AT" },
            { "Poland", "PL" },
            { "Portugal", "PT" },
            { "Greece", "GR" },
            { "Ireland", "IE" },
            { "Turkey", "TR" },
            { "New Zealand", "NZ" },
            { "South Korea", "KR" },
            { "North Korea", "KP" },
            { "Argentina", "AR" },
            { "Chile", "CL" },
            { "Colombia", "CO" },
            { "Egypt", "EG" },
            { "Saudi Arabia", "SA" },
            { "United Arab Emirates", "AE" },
            { "Israel", "IL" }
        };
    }
}
