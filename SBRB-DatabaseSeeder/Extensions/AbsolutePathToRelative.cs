namespace SBRB.Seeder.Extensions
{
    public static class AbsolutePathToRelative
    {
        /// <summary>
        /// Trim an unwanted path from the received absolute path.
        /// </summary>
        /// <param name="absolute">Subject path</param>
        /// <param name="unwantedPath">Part to trim</param>
        /// <returns>A trimmed path string</returns>
        public static string TrimPath(this string absolute, string unwantedPath)
            => absolute.Replace(unwantedPath, "");
    }
}
