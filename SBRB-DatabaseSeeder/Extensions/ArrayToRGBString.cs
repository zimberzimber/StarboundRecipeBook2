using SBRB_DatabaseSeeder.Extensions;
using System;
using System.Text;

namespace SBRB.Seeder.Extensions
{
    public static class ArrayToRGBString
    {
        const int MAX_COLORS = 3;

        /// <summary>
        /// Convert a double array into an RGB string. Only 6 first values are read.
        /// Values are rounded, and clamped to 0-255.
        /// </summary>
        /// <param name="nums">Double array to convert.</param>
        /// <returns>A string describing an RGB color.</returns>
        public static string ToRgBString(this double[] nums)
        {
            // Get the amount of iterations that should happen.
            int iterations = Math.Min(nums.Length, MAX_COLORS);

            // Create the subject byte array
            byte[] bytes = new byte[iterations];

            // Convert numbers into byte appropriate values.
            for (int i = 0; i < iterations; i++)
                bytes[i] = (byte)Math.Max(0, Math.Min(255, Math.Floor(nums[i] + 0.5)));

            // Return the result of the original method.
            return bytes.ToRGBString();
        }

        /// <summary>
        /// Convert an int array into an RGB string. Only 6 first values are read.
        /// Values are clamped to 0-255.
        /// </summary>
        /// <param name="nums">Int array to convert.</param>
        /// <returns>A string describing an RGB color.</returns>
        public static string ToRgBString(this int[] nums)
        {
            // Get the amount of iterations that should happen.
            int iterations = Math.Min(nums.Length, MAX_COLORS);

            // Create the subject byte array
            byte[] bytes = new byte[iterations];

            // Convert numbers into byte appropriate values.
            for (int i = 0; i < iterations; i++)
                bytes[i] = (byte)Math.Max(0, Math.Min(255, nums[i]));

            // Return the result of the original method.
            return bytes.ToRGBString();
        }

        /// <summary>
        /// Convert a byte array into an RGB string. Only 6 first values are read.
        /// </summary>
        /// <param name="nums">Byte array to convert.</param>
        /// <returns>A string describing an RGB color.</returns>
        public static string ToRGBString(this byte[] nums)
        {
            // Get the amount of iterations that should happen.
            int iterations = Math.Min(nums.Length, MAX_COLORS);

            // Create a string builder to which the results are added and stored.
            StringBuilder str = new StringBuilder();

            // Convert the received byte into an RGB color index using a separate extension
            for (int i = 0; i < iterations; i++)
                str.Append(nums[i].ToRgbString());

            // Fill empty indexes with no value for that color if iterations are lower than MAX_COLORS
            for (int i = iterations; i < MAX_COLORS; i++)
                str.Append("00");

            // Return the built string.
            return str.ToString();
        }
    }
}
