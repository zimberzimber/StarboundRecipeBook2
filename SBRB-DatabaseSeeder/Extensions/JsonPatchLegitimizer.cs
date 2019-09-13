namespace SBRB.Seeder.Extensions
{
    static class JsonPatchLegitimizer
    {
        // The container making the JSON patch string legit.
        // Double brackets to have string.Format add them abrackets instead of parsing them as special characters for the formatting process.
        const string JSON_LEGITIMIZING_ENCAPSULATION = "{{\"contents\":{0}}}";

        /// <summary>
        /// Legitimizes a Starbound JSON patch string, so it can be read by the deserializer as proper json.
        /// </summary>
        /// <param name="baseJson">The patch JSON string</param>
        /// <returns>The base JSON string contained within an object under the 'contents' key.</returns>
        public static string LegitizimeJsonPatch(this string baseJson)
        { 
            return string.Format(JSON_LEGITIMIZING_ENCAPSULATION, baseJson).Replace("[]", "null");
        }
    }
}
