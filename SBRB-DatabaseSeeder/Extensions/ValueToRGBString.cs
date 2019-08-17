namespace SBRB_DatabaseSeeder.Extensions
{
    static class ValueToRGBString
    {
        // Character index for easy decimal -> hexideciaml conversion
        const string CHAR_INDEX = "0123456789ABCDEF";

        /// <summary>
        /// Convert a byte into a single RGB string (0 -> 00 | 255 -> FF)
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A single RGB color index</returns>
        public static string ToRgbString(this byte value)
        {
            int units = value % 16;
            int tens = value / 16;
            return string.Format("{0}{1}", CHAR_INDEX[tens], CHAR_INDEX[units]);
        }
    }
}
