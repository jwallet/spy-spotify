using System;
using System.Threading.Tasks;
using EspionSpotify.API;
using EspionSpotify.AudioSessions;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Native;
using EspionSpotify.Native.Models;
using EspionSpotify.Spotify;
using Moq;
using Xunit;

namespace EspionSpotify.Tests
{
    public class SpotifyProcessTests
    {
        private readonly Mock<IMainAudioSession> _mainAudioSessionMock;
        private readonly Mock<IProcessManager> _processManagerMock;

        public SpotifyProcessTests()
        {
            var apiMock = new Mock<IExternalAPI>();
            apiMock.Setup(x => x.UpdateTrack(It.IsAny<Track>()));
            ExternalAPI.Instance = apiMock.Object;

            var processes = new IProcess[]
            {
                new Process {Id = 1, MainWindowTitle = Constants.SPYTIFY, ProcessName = Constants.SPYTIFY, MainWindowHandle = new IntPtr(0x1012)},
                new Process {Id = 2, MainWindowTitle = "Facebook", ProcessName = "Firefox"},
                new Process {Id = 3, MainWindowTitle = "", ProcessName = Constants.SPOTIFY}
            };

            _mainAudioSessionMock = new Mock<IMainAudioSession>();
            _mainAudioSessionMock.Setup(x => x.IsSpotifyCurrentlyPlaying()).ReturnsAsync(false);

            _processManagerMock = new Mock<IProcessManager>();
            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(new Mock<IProcess>().Object);
        }

        [Fact]
        internal void GetSpotifyStatus_ReturnInstance()
        {
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);
            Assert.IsType<SpotifyProcess>(spotifyProcess);
        }

        [Fact]
        internal async Task GetSpotifyStatus_WithNoSpotifyProcess_ReturnsNull()
        {
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.Null(spotifyStatus);
        }

        [Fact]
        internal async Task GetSpotifyStatus_WithWrongSpotifyProcess_ReturnsNull()
        {
            var spotify = new Process {Id = 4, MainWindowTitle = " ", ProcessName = Constants.SPOTIFY, MainWindowHandle = new IntPtr(0x1010)};
            IProcess[] processes = {spotify};

            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(spotify);
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.Null(spotifyStatus);
        }

        [Fact]
        internal async Task GetSpotifyStatus_WithIdleSpotifyProcess_ReturnsNotPlaying()
        {
            const string windowTitle = Constants.SPOTIFY;
            var spotify = new Process {Id = 4, MainWindowTitle = windowTitle, ProcessName = Constants.SPOTIFY, MainWindowHandle = new IntPtr(0x1010)};
            IProcess[] processes = {spotify};

            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(spotify);
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.IsType<SpotifyStatus>(spotifyStatus);
            var track = Assert.IsType<Track>(spotifyStatus.CurrentTrack);

            Assert.Equal(windowTitle, track.Artist);
            Assert.Equal(windowTitle, track.ToString());
            Assert.Empty(track.ToTitleString());
            Assert.False(track.Playing);
            Assert.False(track.IsNormalPlaying);
            Assert.False(track.Ad);
        }

        [Theory]
        [InlineData(Constants.SPOTIFY)]
        [InlineData(Constants.SPOTIFYFREE)]
        [InlineData(Constants.SPOTIFYPREMIUM)]
        internal async Task GetSpotifyStatus_WithSpotifyProcessPlayingInIdle_ReturnsAdPlaying(string title)
        {
            var windowTitle = title.Capitalize();
            var spotify = new Process {Id = 4, MainWindowTitle = windowTitle, ProcessName = Constants.SPOTIFY, MainWindowHandle = new IntPtr(0x1010)};
            IProcess[] processes = {spotify};

            _mainAudioSessionMock.Setup(x => x.IsSpotifyCurrentlyPlaying()).ReturnsAsync(true);
            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(spotify);
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.IsType<SpotifyStatus>(spotifyStatus);
            var track = Assert.IsType<Track>(spotifyStatus.CurrentTrack);

            Assert.Equal(windowTitle, track.ToString());
            Assert.True(track.Playing);
            Assert.True(track.Ad);
            Assert.False(track.IsUnknownPlaying);
        }

        [Fact]
        internal async Task GetSpotifyStatus_WithSpotifyProcessNoSoundOnAds_ReturnsAdPlaying()
        {
            const string windowTitle = Constants.ADVERTISEMENT;
            var spotify = new Process {Id = 4, MainWindowTitle = windowTitle, ProcessName = Constants.SPOTIFY, MainWindowHandle = new IntPtr(0x1010)};
            IProcess[] processes = {spotify};

            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(spotify);
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.IsType<SpotifyStatus>(spotifyStatus);
            var track = Assert.IsType<Track>(spotifyStatus.CurrentTrack);

            Assert.Equal(windowTitle, track.ToString());
            Assert.True(track.Playing);
            Assert.True(track.Ad);
            Assert.False(track.IsUnknownPlaying);
        }

        [Theory]
        [InlineData("#123 Podcast")]
        [InlineData("Message of Government")]
        internal async Task GetSpotifyStatus_WithSpotifyProcessPlayingUnknown_ReturnsAdPlaying(string windowTitle)
        {
            var spotify = new Process {Id = 4, MainWindowTitle = windowTitle, ProcessName = Constants.SPOTIFY, MainWindowHandle = new IntPtr(0x1010)};
            IProcess[] processes = {spotify};

            _mainAudioSessionMock.Setup(x => x.IsSpotifyCurrentlyPlaying()).ReturnsAsync(true);
            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(spotify);
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.IsType<SpotifyStatus>(spotifyStatus);
            var track = Assert.IsType<Track>(spotifyStatus.CurrentTrack);

            Assert.Equal(windowTitle, track.ToString());
            Assert.True(track.Playing);
            Assert.True(track.Ad);
            Assert.False(track.IsNormalPlaying);
            Assert.True(track.IsUnknownPlaying);
        }

        [Theory]
        [InlineData("Artist - Title")]
        [InlineData("Artist - Title - Remastered")]
        [InlineData("Main Artist - Title (feat. Other Artist)")]
        [InlineData("Main Artist - Title (feat. Other Artist) - Live in Town")]
        [InlineData("#123 - Podcast")]
        internal async Task GetSpotifyStatus_WithSpotifyProcessPlayingNormalType_ReturnsTrackPlaying(string windowTitle)
        {
            var spotify = new Process {Id = 4, MainWindowTitle = windowTitle, ProcessName = Constants.SPOTIFY, MainWindowHandle = new IntPtr(0x1010) };
            IProcess[] processes = {spotify};

            _mainAudioSessionMock.Setup(x => x.IsSpotifyCurrentlyPlaying()).ReturnsAsync(true);
            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(spotify);
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.IsType<SpotifyStatus>(spotifyStatus);
            var track = Assert.IsType<Track>(spotifyStatus.CurrentTrack);

            Assert.Equal(windowTitle, track.ToString());
            Assert.NotEqual(windowTitle, track.ToTitleString());
            Assert.True(track.Playing);
            Assert.False(track.Ad);
            Assert.True(track.IsNormalPlaying);
            Assert.False(track.IsUnknownPlaying);
        }

        [Fact]
        internal async Task GetSpotifyStatus_WithSpotifyProcessNoAudioNormalType_ReturnsTrackPlaying()
        {
            const string windowTitle = "Artist - Title";
            var spotify = new Process {Id = 4, MainWindowTitle = windowTitle, ProcessName = Constants.SPOTIFY, MainWindowHandle = new IntPtr(0x1010)};
            IProcess[] processes = {spotify};

            _processManagerMock.Setup(x => x.GetProcesses()).Returns(processes);
            _processManagerMock.Setup(x => x.GetProcessById(It.IsAny<int>())).Returns(spotify);
            var spotifyProcess = new SpotifyProcess(_mainAudioSessionMock.Object, _processManagerMock.Object);

            var spotifyStatus = await spotifyProcess.GetSpotifyStatus();

            Assert.IsType<SpotifyStatus>(spotifyStatus);
            var track = Assert.IsType<Track>(spotifyStatus.CurrentTrack);

            Assert.Equal(windowTitle, track.ToString());
            Assert.NotEqual(windowTitle, track.ToTitleString());
            Assert.True(track.Playing);
            Assert.False(track.Ad);
            Assert.True(track.IsNormalPlaying);
            Assert.False(track.IsUnknownPlaying);
        }

        [Fact]
        internal void GetSpotifyMainWindowHandle_ReturnsHandle()
        {
            var mainWindowHandle = new IntPtr(0x1010);
            var spotify1 = new Process {Id = 4, MainWindowTitle = "Artist - Title", ProcessName = Constants.SPOTIFYFREE, MainWindowHandle = mainWindowHandle};
            var spotify2 = new Process {Id = 4, MainWindowTitle = "", ProcessName = Constants.SPOTIFY, MainWindowHandle = mainWindowHandle};
            var spytify = new Process {Id = 4, MainWindowTitle = Constants.SPYTIFY, ProcessName = Constants.SPYTIFY, MainWindowHandle = new IntPtr(0x1012)};

            _processManagerMock.Setup(x => x.GetProcesses()).Returns(new[] { spotify1, spotify2, spytify });
            var actual = SpotifyProcess.GetMainSpotifyHandler(_processManagerMock.Object);
            
            Assert.Equal(expected: mainWindowHandle, actual);
        }
    }
}