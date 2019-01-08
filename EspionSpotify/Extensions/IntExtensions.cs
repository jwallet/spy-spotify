namespace EspionSpotify.Extensions
{
    public static class IntExtensions
    {
        public static int? ToNullableInt(this string value)
        {
            if (int.TryParse(value, out int i)) return i;
            return null;
        }
    }
}
