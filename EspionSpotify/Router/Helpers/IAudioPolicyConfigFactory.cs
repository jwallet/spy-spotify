using System;
using EspionSpotify.Native;
using NAudio.CoreAudioApi;

namespace EspionSpotify.Router.Helpers
{
    /// <summary>
    /// Provides methods for controlling and interacting with audio endpoints for processes.
    /// </summary>
    public interface IAudioPolicyConfigFactory
    {
        /// <summary>
        /// Sets the persisted default audio endpoint for the specified process identifier.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="flow">The flow.</param>
        /// <param name="role">The role.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The result of setting the default audio endpoint.</returns>
        HRESULT SetPersistedDefaultAudioEndpoint(uint processId, DataFlow flow, Role role, IntPtr deviceId);

        /// <summary>
        /// Gets the persisted default audio endpoint for the specified process identifier.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="flow">The flow.</param>
        /// <param name="role">The role.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The result of getting the default audio endpoint.</returns>
        HRESULT GetPersistedDefaultAudioEndpoint(uint processId, DataFlow flow, Role role, out string deviceId);

        /// <summary>
        /// Clears all persisted application default endpoints.
        /// </summary>
        /// <returns>The result of clearing all persisted endpoints.</returns>
        HRESULT ClearAllPersistedApplicationDefaultEndpoints();
    }
}