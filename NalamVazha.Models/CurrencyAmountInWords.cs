using System;
using System.Collections.Generic;

namespace NalamVazha.Models
{
    public static class CurrencyAmountInWords
    {
        private static readonly string[] Ones =
        {
            "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
            "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
            "Seventeen", "Eighteen", "Nineteen"
        };

        private static readonly string[] Tens =
        {
            "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        private static readonly Dictionary<string, (string Major, string Minor)> CurrencyNames =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["INR"] = ("Rupees", "Paise"),
                ["USD"] = ("Dollars", "Cents"),
                ["EUR"] = ("Euros", "Cents"),
                ["GBP"] = ("Pounds", "Pence"),
                ["AED"] = ("Dirhams", "Fils")
            };

        public static string Convert(decimal amount, string currencyCode)
        {
            var code = string.IsNullOrWhiteSpace(currencyCode) ? "INR" : currencyCode.Trim().ToUpperInvariant();
            (string Major, string Minor) names = CurrencyNames.TryGetValue(code, out var configured)
                ? configured
                : (code, "Minor Units");

            var rounded = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
            var absolute = Math.Abs(rounded);
            var major = decimal.ToInt64(decimal.Truncate(absolute));
            var minor = decimal.ToInt32((absolute - major) * 100m);

            var prefix = rounded < 0 ? "Refund " : "";
            var words = prefix + names.Major + " " + ConvertIndianNumber(major);
            if (minor > 0)
                words += " and " + ConvertIndianNumber(minor) + " " + names.Minor;

            return words + " Only";
        }

        private static string ConvertIndianNumber(long number)
        {
            if (number == 0) return Ones[0];
            if (number < 0) return "Minus " + ConvertIndianNumber(Math.Abs(number));

            var parts = new List<string>();
            AppendScale(parts, ref number, 10_000_000_000_000L, "Neel");
            AppendScale(parts, ref number, 100_000_000_000L, "Kharab");
            AppendScale(parts, ref number, 1_000_000_000L, "Arab");
            AppendScale(parts, ref number, 10_000_000L, "Crore");
            AppendScale(parts, ref number, 100_000L, "Lakh");
            AppendScale(parts, ref number, 1_000L, "Thousand");

            if (number >= 100)
            {
                parts.Add(Ones[number / 100] + " Hundred");
                number %= 100;
            }

            if (number > 0)
                parts.Add(number < 20 ? Ones[number] : Tens[number / 10] + (number % 10 > 0 ? " " + Ones[number % 10] : ""));

            return string.Join(" ", parts);
        }

        private static void AppendScale(List<string> parts, ref long number, long divisor, string scale)
        {
            if (number < divisor) return;
            parts.Add(ConvertIndianNumber(number / divisor) + " " + scale);
            number %= divisor;
        }
    }
}
