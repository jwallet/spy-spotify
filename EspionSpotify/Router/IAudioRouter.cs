namespace EspionSpotify.Router
{
    /// <summary>
    /// Provides a service for controlling and interacting with the audio device of an application.
    /// </summary>
    public interface IAudioRouter
    {
        bool IsRouted { get; }

        /// <summary>
        /// Gets the default audio device for the specified process.
        /// </summary>
        /// <param name="processId">The process id.</param>
        /// <returns>The audio device; otherwise <c>null</c>.</returns>
        string GetDefaultEndPoint(int processId);

        /// <summary>
        /// Sets the default audio device of an application.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="processId">The process to update.</param>
        void SetDefaultEndPoint(string deviceId, int processId);

        /// <summary>
        /// Resets the default audio devices previously set.
        /// </summary>
        void ResetDefaultEndpoints();
    }
}