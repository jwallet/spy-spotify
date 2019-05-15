namespace EspionSpotify.MediaTags
{
    public static class ExternalAPI
    {
        public static IExternalAPI Instance { get; set; } = new LastFMAPI();
    }
}
