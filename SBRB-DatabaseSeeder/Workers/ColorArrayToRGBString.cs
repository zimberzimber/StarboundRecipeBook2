using System;
using System.Text;

namespace SBRB.Seeder.Workers
{
    static class ColorArrayToRGBString
    {
        const int MAX_COLORS = 3;
        const string CHAR_INDEX = "0123456789ABCDEF";

        public static string ToRgBString(this double[] nums)
        {
            byte[] bytes = new byte[nums.Length];

            for (int i = 0; i < nums.Length; i++)
                bytes[i] = (byte)Math.Max(0, Math.Min(255, Math.Floor(nums[i] + 0.5)));

            return bytes.ToRGBString();
        }

        public static string ToRgBString(this int[] nums)
        {
            byte[] bytes = new byte[nums.Length];

            for (int i = 0; i < nums.Length; i++)
                bytes[i] = (byte)Math.Max(0, Math.Min(255, nums[i]));

            return bytes.ToRGBString();
        }

        public static string ToRGBString(this byte[] nums)
        {
            int iterations = Math.Min(nums.Length, MAX_COLORS);
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < iterations; i++)
            {
                int units = nums[i] % CHAR_INDEX.Length;
                int tens = nums[i] / 16;
                str.Append(CHAR_INDEX[tens]);
                str.Append(CHAR_INDEX[units]);
            }

            return str.ToString();
        }
    }
}
