using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EspionSpotify.AudioSessions;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Native;
using EspionSpotify.Native.Models;
using Process = System.Diagnostics.Process;

namespace EspionSpotify.Spotify
{
    public class SpotifyProcess : ISpotifyProcess
    {
        private readonly IMainAudioSession _audioSession;
        private readonly IProcessManager _processManager;
        private int? _spotifyProcessId;

        internal SpotifyProcess(IMainAudioSession audioSession) :
            this(audioSession, new ProcessManager())
        {
        }

        public SpotifyProcess(IMainAudioSession audioSession, IProcessManager processManager)
        {
            _processManager = processManager;
            _audioSession = audioSession;
            _spotifyProcessId = GetMainSpotifyProcess(_processManager)?.Id;
        }

        public async Task<ISpotifyStatus> GetSpotifyStatus()
        {
            var (processTitle, isSpotifyAudioPlaying) = await GetSpotifyTitle();
            var isWindowTitledSpotify = processTitle.IsNullOrSpotifyIdleState();

            if (string.IsNullOrWhiteSpace(processTitle)) return null;

            var spotifyWindowInfo = new SpotifyWindowInfo
            {
                WindowTitle = processTitle,
                IsPlaying = isSpotifyAudioPlaying || !isWindowTitledSpotify
            };

            return new SpotifyStatus(spotifyWindowInfo);
        }

        private async Task<(string, bool)> GetSpotifyTitle()
        {
            string mainWindowTitle = null;
            var isSpotifyAudioPlaying = false;

            if (_spotifyProcessId.HasValue)
                try
                {
                    isSpotifyAudioPlaying = await _audioSession.IsSpotifyCurrentlyPlaying();
                    var process = _processManager.GetProcessById(_spotifyProcessId.Value);
                    mainWindowTitle = process?.MainWindowTitle ?? "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            else
                _spotifyProcessId = GetMainSpotifyProcess(_processManager)?.Id;

            return (mainWindowTitle, isSpotifyAudioPlaying);
        }

        internal static ICollection<IProcess> GetSpotifyProcesses(IProcessManager processManager)
        {
            return processManager.GetProcesses().Where(x => x.ProcessName.IsSpotifyIdleState()).ToList();
        }

        private static IProcess GetMainSpotifyProcess(IProcessManager processManager)
        {
            return processManager.GetProcesses()
                .FirstOrDefault(x => x.ProcessName.IsSpotifyIdleState() && !string.IsNullOrEmpty(x.MainWindowTitle));
        }

        public static IntPtr? GetMainSpotifyHandler(IProcessManager processManager)
        {
            return GetMainSpotifyProcess(processManager)?.MainWindowHandle;
        }
    }
}