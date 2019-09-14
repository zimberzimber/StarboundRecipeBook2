using System.Text;
using System.Text.RegularExpressions;

namespace SBRB_DatabaseSeeder.Extensions
{
    static class StringDeformatter
    {
        const string REGEX_FORMAT_CATCHER = @"\^(.*?);";

        public static string RemoveFormatting(this string original)
        {
            if (!Regex.IsMatch(original, REGEX_FORMAT_CATCHER))
                return original;

            Regex rx = new Regex(REGEX_FORMAT_CATCHER);
            string[] splits = rx.Split(original);

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < splits.Length; i++)
            {
                // Every 2nd is the formatting tag, which we should ignore
                if (i % 2 == 0)
                    result.Append(splits[i]);
            }

            return result.ToString();
        }
    }
}
