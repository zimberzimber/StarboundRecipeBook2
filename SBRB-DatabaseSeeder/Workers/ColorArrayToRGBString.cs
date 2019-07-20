using System;
using System.Text;

namespace SBRB_DatabaseSeeder.Workers
{
    static class ColorArrayToRGBString
    {
        const int MAX_COLORS = 3;
        const string CHAR_INDEX = "0123456789ABCDEF";

        public static string ToRGBString(this int[] nums)
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
