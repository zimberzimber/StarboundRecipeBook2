﻿namespace SBRB_DatabaseSeeder.Workers
{
    public static class AbsolutePathToRelative
    {
        public static string ToReletivePath(this string absolute, string unwantedPath)
            => absolute.Replace(unwantedPath, "");
    }
}