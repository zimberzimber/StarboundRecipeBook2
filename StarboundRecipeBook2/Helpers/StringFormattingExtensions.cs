using System.Text;
using System.Text.RegularExpressions;

namespace StarboundRecipeBook2.Helpers
{
    public static class StringFormattingExtensions
    {
        const string REGEX_FORMAT_CATCHER = @"\^(.*?);";
        const string HTML_FORMATTED_COLOR_START = "<p style=\"color:{0};\">";
        const string HTML_FORMATTED_END = "{0}</p>";

        public static string ResolveColor(this string original)
        {
            if (!Regex.IsMatch(original, REGEX_FORMAT_CATCHER))
                return original;

            Regex rx = new Regex(REGEX_FORMAT_CATCHER);
            string[] splits = rx.Split(original);

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < splits.Length; i++)
            {
                // Every 2nd is the color tag
                if (i % 2 == 1)
                {
                    if (splits[i].ToLower().Equals("white"))
                        result.Append(string.Format(HTML_FORMATTED_COLOR_START, "reset"));
                    else
                        result.Append(string.Format(HTML_FORMATTED_COLOR_START, splits[i]));
                }
                else
                {
                    if (i > 0)
                        result.Append(string.Format(HTML_FORMATTED_END, splits[i]));
                    else
                        result.Append(splits[i]);
                }
            }

            return result.ToString();
        }

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
