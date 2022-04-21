using System;
using System.Runtime.InteropServices;
using EspionSpotify.Native;
using NAudio.CoreAudioApi;

namespace EspionSpotify.Router
{
     /// <summary>
    /// Provides methods for controlling and interacting with audio endpoints for processes.
    /// </summary>
    [Guid("2a59116d-6c4f-45e0-a74f-707e3fef9258")]
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IAudioPolicyConfigFactoryDownlevel
    {
        int __incomplete__add_CtxVolumeChange();
        int __incomplete__remove_CtxVolumeChanged();
        int __incomplete__add_RingerVibrateStateChanged();
        int __incomplete__remove_RingerVibrateStateChange();
        int __incomplete__SetVolumeGroupGainForId();
        int __incomplete__GetVolumeGroupGainForId();
        int __incomplete__GetActiveVolumeGroupForEndpointId();
        int __incomplete__GetVolumeGroupsForEndpoint();
        int __incomplete__GetCurrentVolumeContext();
        int __incomplete__SetVolumeGroupMuteForId();
        int __incomplete__GetVolumeGroupMuteForId();
        int __incomplete__SetRingerVibrateState();
        int __incomplete__GetRingerVibrateState();
        int __incomplete__SetPreferredChatApplication();
        int __incomplete__ResetPreferredChatApplication();
        int __incomplete__GetPreferredChatApplication();
        int __incomplete__GetCurrentChatApplications();
        int __incomplete__add_ChatContextChanged();
        int __incomplete__remove_ChatContextChanged();

        /// <summary>
        /// Sets the persisted default audio endpoint for the specified process identifier.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="flow">The flow.</param>
        /// <param name="role">The role.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The result of setting the default audio endpoint.</returns>
        [PreserveSig]
        HRESULT SetPersistedDefaultAudioEndpoint(uint processId, DataFlow flow, Role role, IntPtr deviceId);

        /// <summary>
        /// Gets the persisted default audio endpoint for the specified process identifier.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="flow">The flow.</param>
        /// <param name="role">The role.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns>The result of getting the default audio endpoint.</returns>
        [PreserveSig]
        HRESULT GetPersistedDefaultAudioEndpoint(uint processId, DataFlow flow, Role role, [Out, MarshalAs(UnmanagedType.HString)] out string deviceId);

        /// <summary>
        /// Clears all persisted application default endpoints.
        /// </summary>
        /// <returns>The result of clearing all persisted endpoints.</returns>
        [PreserveSig]
        HRESULT ClearAllPersistedApplicationDefaultEndpoints();
    }
}
