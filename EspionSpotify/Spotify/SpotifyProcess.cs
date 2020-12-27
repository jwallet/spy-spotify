using EspionSpotify.AudioSessions;
using EspionSpotify.Models;
using EspionSpotify.Native;
using EspionSpotify.Native.Models;
using EspionSpotify.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify
{
    public class SpotifyProcess : ISpotifyProcess
    {
        private readonly int? _spotifyProcessId;
        private readonly IMainAudioSession _audioSession;
        private readonly IProcessManager _processManager;

        internal SpotifyProcess(IMainAudioSession audioSession) :
            this(audioSession, processManager: new ProcessManager())
        { }

        public SpotifyProcess(IMainAudioSession audioSession, IProcessManager processManager)
        {
            _processManager = processManager;
            _audioSession = audioSession;
            _spotifyProcessId = GetSpotifyProcesses(_processManager).FirstOrDefault(x => !string.IsNullOrEmpty(x.MainWindowTitle))?.Id;
        }

        public async Task<ISpotifyStatus> GetSpotifyStatus()
        {
            var isSpotifyAudioPlaying = await _audioSession.IsSpotifyCurrentlyPlaying();
            var processTitle = GetSpotifyTitle();
            var isWindowTitledSpotify = SpotifyStatus.WindowTitleIsSpotify(processTitle);

            if (string.IsNullOrWhiteSpace(processTitle))
            {
                return null;
            }

            var spotifyWindowInfo = new SpotifyWindowInfo
            {
                WindowTitle = processTitle,
                IsPlaying = isSpotifyAudioPlaying || !isWindowTitledSpotify
            };

            return new SpotifyStatus(spotifyWindowInfo);
        }

        private string GetSpotifyTitle()
        {
            if (!_spotifyProcessId.HasValue)
            {
                return null;
            }

            string mainWindowTitle = null;

            try
            {
                var process = _processManager.GetProcessById(_spotifyProcessId.Value);
                mainWindowTitle = process.MainWindowTitle;
            }
            catch (Exception) { }

            return mainWindowTitle;
        }

        internal static ICollection<IProcess> GetSpotifyProcesses(IProcessManager processManager)
        {
            var spotifyProcesses = new List<IProcess>();

            foreach (var process in processManager.GetProcesses())
            {
                if (SpotifyStatus.WindowTitleIsSpotify(process.ProcessName))
                {
                    spotifyProcesses.Add(process);
                }
            }

            return spotifyProcesses;
        }
    }
}
