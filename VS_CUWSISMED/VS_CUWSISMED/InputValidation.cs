using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace VS_CUWSISMED
{
    internal static class InputValidation
    {
        private static readonly Regex NameRegex = new Regex(@"^[\p{L}\s-]+$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PostalCodeRegex = new Regex(@"^\d{2}-\d{3}$", RegexOptions.Compiled);

        public static bool IsName(string value)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length > 0 && NameRegex.IsMatch(value);
        }

        public static bool IsOptionalName(string value)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length == 0 || IsName(value);
        }

        public static bool IsPeselPrefix(string value)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length <= 11 && value.All(char.IsDigit);
        }

        public static bool IsFullPesel(string value)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length == 11 && value.All(char.IsDigit);
        }

        public static bool IsPhone(string value)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length <= 9 && value.All(char.IsDigit);
        }

        public static bool IsOptionalEmail(string value)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length == 0 || EmailRegex.IsMatch(value);
        }

        public static bool IsPostalCode(string value)
        {
            value = (value ?? string.Empty).Trim();
            return PostalCodeRegex.IsMatch(value);
        }

        public static bool TryParseBirthDate(string value, out DateTime? birthDate)
        {
            birthDate = null;
            value = (value ?? string.Empty).Trim();
            if (value.Length == 0)
            {
                return true;
            }

            DateTime parsed;
            if (DateTime.TryParseExact(
                value,
                new[] { "dd.MM.yyyy", "dd-MM-yyyy" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsed))
            {
                birthDate = parsed.Date;
                return true;
            }

            return false;
        }
    }
}
