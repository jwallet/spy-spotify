using EspionSpotify.AudioSessions;
using EspionSpotify.Models;
using EspionSpotify.Spotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify
{
    public class SpotifyProcess: ISpotifyProcess
    {
        private readonly int? _spotifyProcessId;
        private readonly IMainAudioSession _audioSession;

        public SpotifyProcess(IMainAudioSession audioSession)
        {
            _audioSession = audioSession;
            _spotifyProcessId = GetSpotifyProcesses().FirstOrDefault(x => !string.IsNullOrEmpty(x.MainWindowTitle))?.Id;
        }

        public async Task<ISpotifyStatus> GetSpotifyStatus()
        {
            var isSpotifyPlaying = await _audioSession.IsSpotifyCurrentlyPlaying();
            var processTitle = GetSpotifyTitle();

            if (string.IsNullOrWhiteSpace(processTitle))
            {
                return null;
            }

            var spotifyWindowInfo = new SpotifyWindowInfo
            {
                WindowTitle = processTitle,
                IsPlaying = isSpotifyPlaying
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
                var process = Process.GetProcessById(_spotifyProcessId.Value);
                mainWindowTitle = process.MainWindowTitle;
            }
            catch (Exception) { }

            return mainWindowTitle;
        }

        internal static ICollection<Process> GetSpotifyProcesses()
        {
            var spotifyProcesses = new List<Process>();

            foreach (var process in Process.GetProcesses())
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
