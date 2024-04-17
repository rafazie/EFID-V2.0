namespace EFID_V2.Classes
{
    public static class AvoidNull
    {
        public static string AvoidNullString(this string value)
        {
            if (value == null)
                return "";
            return value;
        }

        public static int AvoidNullInt(this int value)
        {
            if (value == null)
                return 0;
            return value;
        }
    }
}
