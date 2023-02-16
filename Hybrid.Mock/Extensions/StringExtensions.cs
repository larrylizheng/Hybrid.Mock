using System.Text.RegularExpressions;

namespace Hybrid.Mock.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string text)
        {
            return string.Join(" ", text
                .Split()
                .Select(i => char.ToUpper(i[0]) + i.Substring(1).ToLower()));
        }

        public static string GetTabDisplayName(this string serviceName)
        {
            const string delimiterPattern = @"[\s\-\._]";
            var regex = new Regex(delimiterPattern);

            if (regex.IsMatch(serviceName))
            {
                var stringResult = Regex.Replace(serviceName, delimiterPattern, " ");
                return stringResult.ToCamelCase();
            }
            else
            {
                return AddSpaceToCamelCaseString(serviceName);
            }
        }

        public static string AddSpaceToCamelCaseString(this string camelCaseString)
        {
            return Regex.Replace(camelCaseString, "(?<=[a-z])([A-Z])", " $1");
        }
    }
}