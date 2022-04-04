namespace EspionSpotify.API
{
    public static class ExternalAPI
    {
        public static IExternalAPI Instance { get; set; } = new LastFMAPI();
    }
}