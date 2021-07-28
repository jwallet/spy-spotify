using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify.API
{
    public class SpotifyAPI : ISpotifyAPI, IExternalAPI, IDisposable
    {
        private bool _disposed = false;
        private readonly string _clientId;
        private readonly string _secretId;
        private Token _token;
        private AuthorizationCodeAuth _authorizationCodeAuth;
        private readonly LastFMAPI _lastFmApi;
        private readonly AuthorizationCodeAuth _auth;
        private string _refreshToken;
        private SpotifyWebAPI _api;
        private bool _connectionDialogOpened = false;

        public const string SPOTIFY_API_DEFAULT_REDIRECT_URL = "http://localhost:4002";
        public const string SPOTIFY_API_DASHBOARD_URL = "https://developer.spotify.com/dashboard";

        public bool IsAuthenticated { get => _token != null; }

        public ExternalAPIType GetTypeAPI { get => ExternalAPIType.Spotify; }

        public SpotifyAPI() { }

        public SpotifyAPI(string clientId, string secretId, string redirectUrl = SPOTIFY_API_DEFAULT_REDIRECT_URL)
        {
            _clientId = clientId;
            _secretId = secretId;
            _lastFmApi = new LastFMAPI();

            if (!string.IsNullOrEmpty(_clientId) && !string.IsNullOrEmpty(_secretId))
            {
                _auth = new AuthorizationCodeAuth(_clientId, _secretId, redirectUrl, redirectUrl,
                    Scope.Streaming | Scope.PlaylistReadCollaborative | Scope.UserReadCurrentlyPlaying | Scope.UserReadRecentlyPlayed | Scope.UserReadPlaybackState);
                _auth.AuthReceived += AuthOnAuthReceived;
                _auth.Start();
            }
        }

        [Obsolete("It triggers too many web requests, ~ 60k per day")]
        public async Task<(string, bool)> GetCurrentPlayback()
        {
            var playing = false;
            string title = null;

            await GetSpotifyWebAPI();
            
            if (_api != null)
            {
                var playback = await _api.GetPlaybackWithoutExceptionAsync();
                if (playback != null && !playback.HasError())
                {
                    playing = playback.IsPlaying;

                    if (playing)
                    {
                        switch (playback.CurrentlyPlayingType)
                        {
                            case TrackType.Ad:
                                title = Constants.ADVERTISEMENT;
                                break;
                            case TrackType.Track when playback.Item != null:
                                title = string.Join(" - ", new[] { playback.Item.Artists.Select(x => x.Name).First(), playback.Item.Name });
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (title, playing);
        }

        public async Task UpdateTrack(Track track) => await UpdateTrack(track, retry: false);

        public void MapSpotifyTrackToTrack(Track track, FullTrack spotifyTrack)
        {
            var performers = GetAlbumArtistFromSimpleArtistList(spotifyTrack.Artists);
            var (titleParts, separatorType) = SpotifyStatus.GetTitleTags(spotifyTrack.Name, 2);

            track.SetArtistFromAPI(performers.FirstOrDefault());
            track.SetTitleFromAPI(SpotifyStatus.GetTitleTag(titleParts, 1));
            track.SetTitleExtendedFromAPI(SpotifyStatus.GetTitleTag(titleParts, 2), separatorType);

            track.AlbumPosition = spotifyTrack.TrackNumber;
            track.Performers = performers;
            track.Disc = spotifyTrack.DiscNumber;
        }

        public void MapSpotifyAlbumToTrack(Track track, FullAlbum spotifyAlbum)
        {
            track.AlbumArtists = GetAlbumArtistFromSimpleArtistList(spotifyAlbum.Artists);
            track.Album = spotifyAlbum.Name;
            track.Genres = spotifyAlbum.Genres.ToArray();

            if (DateTime.TryParse(spotifyAlbum.ReleaseDate ?? "", out var date))
            {
                track.Year = date.Year;
            }

            if (spotifyAlbum.Images?.Count > 0)
            {
                var sorted = spotifyAlbum.Images.OrderByDescending(i => i.Width).ToList();

                if (sorted.Count > 0) track.ArtExtraLargeUrl = sorted[0].Url;
                if (sorted.Count > 1) track.ArtLargeUrl = sorted[1].Url;
                if (sorted.Count > 2) track.ArtMediumUrl = sorted[2].Url;
                if (sorted.Count > 3) track.ArtSmallUrl = sorted[3].Url;
            }
        }

        #region Spotify Track updater
        private async Task UpdateTrack(Track track, bool retry = false)
        {
            await GetSpotifyWebAPI();

            if (_api == null) return;

            var playback = await _api.GetPlaybackWithoutExceptionAsync();
            var hasNoPlayback = playback == null || playback.Item == null;

            if (!retry && hasNoPlayback)
            {
                await Task.Delay(3000);
                await UpdateTrack(track, retry: true);
                if (track.MetaDataUpdated)
                {
                    retry = false;
                }
                else
                {
                    // open spotify authentication page if user is disconnected
                    // user might be connected with a different account that the one that granted rights
                    OpenAuthenticationDialog();
                }
                return;
            }

            if (hasNoPlayback || playback.HasError())
            {
                _api.Dispose();

                // fallback in case getting the playback did not work
                ExternalAPI.Instance = _lastFmApi;
                Settings.Default.app_selected_external_api_id = (int)Enums.ExternalAPIType.LastFM;
                Settings.Default.Save();

                _ = Task.Run(() =>
                {
                    FrmEspionSpotify.Instance.UpdateExternalAPIToggle(Enums.ExternalAPIType.LastFM);
                    FrmEspionSpotify.Instance.ShowFailedToUseSpotifyAPIMessage();
                });

                await _lastFmApi.UpdateTrack(track);

                return;
            }

            MapSpotifyTrackToTrack(track, playback.Item);

            if (playback.Item.Album?.Id == null) return;
            
            var album = await _api.GetAlbumWithoutExceptionAsync(playback.Item.Album.Id);

            if (album.HasError()) return;
                
            MapSpotifyAlbumToTrack(track, album);

            track.MetaDataUpdated = true;
        }
        #endregion Spotify Track updater

        private string[] GetAlbumArtistFromSimpleArtistList(List<SimpleArtist> artists) => (artists ?? new List<SimpleArtist>()).Select(a => a.Name).ToArray();

        private async void AuthOnAuthReceived(object sender, AuthorizationCode payload)
        {
            _authorizationCodeAuth = (AuthorizationCodeAuth)sender;

            _authorizationCodeAuth.Stop();

            try
            {
                _token = await _authorizationCodeAuth.ExchangeCode(payload.Code);
                _refreshToken = _token.RefreshToken;
                _connectionDialogOpened = false;
            }
            catch { }
        }

        private void OpenAuthenticationDialog()
        {
            if (_connectionDialogOpened) return;
            _auth.ShowDialog = true;
            _auth.OpenBrowser();
            _connectionDialogOpened = true;
        }

        private async Task GetSpotifyWebAPI()
        {
            if (_token == null)
            {
                OpenAuthenticationDialog();
                return;
            }

            if (_token.IsExpired())
            {
                try
                {
                    _api.Dispose();
                    _api = null;
                    _token = await _authorizationCodeAuth.RefreshToken(_token.RefreshToken ?? _refreshToken);
                }
                catch { }
            }

            if (_api == null)
            {
                try
                {
                    _api = new SpotifyWebAPI
                    {
                        AccessToken = _token.AccessToken,
                        TokenType = _token.TokenType
                    };
                }
                catch (Exception)
                {
                    _api = null;
                    _authorizationCodeAuth.Stop();
                }
            }
        }

        public async Task Authenticate() => await GetSpotifyWebAPI();

        public void Reset()
        {
            _connectionDialogOpened = false;
        }


        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_api != null) _api.Dispose();
            }

            _disposed = true;
        }
    }
}