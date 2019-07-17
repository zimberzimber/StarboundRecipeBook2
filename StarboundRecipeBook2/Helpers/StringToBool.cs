namespace StarboundRecipeBook2.Helpers
{
    public static class StringToBool
    {
        public static bool? ToBool(this string subject)
        {
            switch (subject?.ToLower())
            {
                case "true": return true;
                case "false": return false;
                default: return null;
            }
        }
    }
}
