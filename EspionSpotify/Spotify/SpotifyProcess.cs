using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EspionSpotify.AudioSessions;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Native;
using EspionSpotify.Native.Models;

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
            _spotifyProcessId = GetSpotifyProcesses(_processManager)
                .FirstOrDefault(x => !string.IsNullOrEmpty(x.MainWindowTitle))?.Id;
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
                _spotifyProcessId = GetSpotifyProcesses(_processManager)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x.MainWindowTitle))?.Id;

            return (mainWindowTitle, isSpotifyAudioPlaying);
        }

        internal static ICollection<IProcess> GetSpotifyProcesses(IProcessManager processManager)
        {
            var spotifyProcesses = new List<IProcess>();

            foreach (var process in processManager.GetProcesses())
                if (process.ProcessName.IsSpotifyIdleState())
                    spotifyProcesses.Add(process);

            return spotifyProcesses;
        }
    }
}