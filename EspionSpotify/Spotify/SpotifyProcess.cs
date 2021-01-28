using EspionSpotify.AudioSessions;
using EspionSpotify.API;
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
            var (processTitle, isSpotifyAudioPlaying) = await GetSpotifyTitle();
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

        private async Task<(string, bool)> GetSpotifyTitle()
        {
            string mainWindowTitle = null;
            var isSpotifyAudioPlaying = false;
            
            if (_spotifyProcessId.HasValue)
            {
                try
                {
                    if (ExternalAPI.Instance.GetTypeAPI == Enums.ExternalAPIType.Spotify && ExternalAPI.Instance.IsAuthenticated)
                    {
                        var (title, isPlaying) = await ExternalAPI.Instance.GetCurrentPlayback();
                        mainWindowTitle = title;
                        isSpotifyAudioPlaying = isPlaying;
                    }
                    // always fallback if it was not properly set by spotify api
                    if (mainWindowTitle == null)
                    {
                        isSpotifyAudioPlaying = await _audioSession.IsSpotifyCurrentlyPlaying();
                        var process = _processManager.GetProcessById(_spotifyProcessId.Value);
                        mainWindowTitle = process?.MainWindowTitle ?? "";
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return (mainWindowTitle, isSpotifyAudioPlaying);
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
