using System.Text;
using System.Text.RegularExpressions;

namespace StarboundRecipeBook2.Services
{
    public interface ITextColorResolver
    {
        string ResolveColor(string raw);
    }

    public class TextColorResolver : ITextColorResolver
    {
        const string REGEX_FORMAT_CATCHER = @"\^(.*?);";
        const string FORMATTED_START = "<p style=\"color:{0};\">";
        const string FORMATTED_END = "{0}</p>";

        public string ResolveColor(string raw)
        {
            if (!Regex.IsMatch(raw, REGEX_FORMAT_CATCHER))
                return raw;

            Regex rx = new Regex(REGEX_FORMAT_CATCHER);
            string[] splits = rx.Split(raw);

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < splits.Length; i++)
            {
                // Every 2nd is the color tag
                if (i % 2 == 1)
                {
                    if (splits[i].ToLower().Equals("white"))
                        result.Append(string.Format(FORMATTED_START, "reset"));
                    else
                        result.Append(string.Format(FORMATTED_START, splits[i]));
                }
                else
                {
                    if (i > 0)
                        result.Append(string.Format(FORMATTED_END, splits[i]));
                    else
                        result.Append(splits[i]);
                }
            }

            return result.ToString();
        }
    }
}
