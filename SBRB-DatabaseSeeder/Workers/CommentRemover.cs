using System.Text.RegularExpressions;

namespace SBRB.Seeder.Workers
{
    static class CommentRemover
    {
        // Used to remove comments from the files as JSON de/serializers do not inherently support commenting
        // Single line comment regex:		[/]+[/](.*?)[\n]
        // Multi-line comment regex:		[/]+[*](.*?)[*]+[/]
        const string UNCOMMENT_REGEX_PATTERN = @"([/]+[/](.*?)[\n])|([/]+[*](.*?)[*]+[/])";

        /// <summary>Uncomment the subject JSON string</summary>
        /// <param name="json">Subject JSON string to uncomment</param>
        /// <returns>The uncommented JSON string</returns>
        public static string RemoveComments(this string json)
            => Regex.Replace(json, UNCOMMENT_REGEX_PATTERN, "", RegexOptions.Singleline);
    }
}
