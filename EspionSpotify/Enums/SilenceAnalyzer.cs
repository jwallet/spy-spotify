namespace EspionSpotify.Enums
{
    public enum SilenceAnalyzer
    {  
        None,
        TrimEnd,
        TrimStart,
        SkipStart, // Skip first X milliseconds to prevent audio overlap from previous track
    }
}