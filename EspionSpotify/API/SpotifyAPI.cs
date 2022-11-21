using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EspionSpotify.Enums;
using EspionSpotify.Extensions;
using EspionSpotify.Models;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace EspionSpotify.API
{
    public sealed class SpotifyAPI : ISpotifyAPI, IExternalAPI, IDisposable
    {
        public const string SPOTIFY_API_DEFAULT_REDIRECT_URL = "http://localhost:4002";
        public const string SPOTIFY_API_DASHBOARD_URL = "https://developer.spotify.com/dashboard";
        private readonly AuthorizationCodeAuth _auth;
        private readonly LastFMAPI _lastFmApi;
        private SpotifyWebAPI _api;
        private AuthorizationCodeAuth _authorizationCodeAuth;
        private bool _connectionDialogOpened;
        private bool _disposed;
        private string _refreshToken;
        private Token _token;

        public SpotifyAPI()
        {
        }

        public SpotifyAPI(string clientId, string secretId, string redirectUrl = SPOTIFY_API_DEFAULT_REDIRECT_URL)
        {
            _lastFmApi = new LastFMAPI();

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(secretId))
            {
                _auth = new AuthorizationCodeAuth(clientId, secretId, redirectUrl, redirectUrl,
                    Scope.Streaming | Scope.PlaylistReadCollaborative | Scope.UserReadCurrentlyPlaying |
                    Scope.UserReadRecentlyPlayed | Scope.UserReadPlaybackState);
                _auth.AuthReceived += AuthOnAuthReceived;
                _auth.Start();
            }
        }


        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public bool IsAuthenticated => _token != null;

        public ExternalAPIType GetTypeAPI => ExternalAPIType.Spotify;

        public async Task<bool> UpdateTrack(Track track)
        {
            return await UpdateTrack(track, false);
        }

        public async Task Authenticate()
        {
            await GetSpotifyWebAPI();
        }

        public void Reset()
        {
            _connectionDialogOpened = false;
        }

        public void MapSpotifyTrackToTrack(Track track, FullTrack spotifyTrack)
        {
            var performers = GetAlbumArtistFromSimpleArtistList(spotifyTrack.Artists);
            var (titleParts, separatorType) = SpotifyStatus.GetTitleTags(spotifyTrack.Name, 2);

            track.SetArtistFromApi(performers.FirstOrDefault());
            track.SetTitleFromApi(SpotifyStatus.GetTitleTag(titleParts, 1));
            track.SetTitleExtendedFromApi(SpotifyStatus.GetTitleTag(titleParts, 2), separatorType);

            track.AlbumPosition = spotifyTrack.TrackNumber;
            track.Performers = performers;
            track.Disc = spotifyTrack.DiscNumber;
        }

        public void MapSpotifyAlbumToTrack(Track track, FullAlbum spotifyAlbum)
        {
            track.AlbumArtists = GetAlbumArtistFromSimpleArtistList(spotifyAlbum.Artists);
            track.Album = spotifyAlbum.Name;
            track.Genres = spotifyAlbum.Genres.ToArray();

            if (DateTime.TryParse(spotifyAlbum.ReleaseDate ?? "", out var date)) track.Year = date.Year;

            if (spotifyAlbum.Images?.Count > 0)
            {
                var sorted = spotifyAlbum.Images.OrderByDescending(i => i.Width).ToList();

                if (sorted.Count > 0) track.ArtExtraLargeUrl = sorted[0].Url;
                if (sorted.Count > 1) track.ArtLargeUrl = sorted[1].Url;
                if (sorted.Count > 2) track.ArtMediumUrl = sorted[2].Url;
                if (sorted.Count > 3) track.ArtSmallUrl = sorted[3].Url;
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
                        switch (playback.CurrentlyPlayingType)
                        {
                            case TrackType.Ad:
                                title = Constants.ADVERTISEMENT;
                                break;
                            case TrackType.Track when playback.Item != null:
                                title = string.Join(" - ", playback.Item.Artists.Select(x => x.Name).First(),
                                    playback.Item.Name);
                                break;
                        }
                }
            }

            return (title, playing);
        }

        #region Spotify Track updater

        private async Task<bool> UpdateTrack(Track track, bool retryDone = false)
        {
            await GetSpotifyWebAPI();

            if (_api == null) return false;

            await Task.Delay(100);

            var playback = await _api.GetPlaybackWithoutExceptionAsync();
            var hasNoPlayback = playback == null || playback.Item == null;

            if (!retryDone && hasNoPlayback)
            {
                await Task.Delay(1000);
                var res = await UpdateTrack(track, true);
                if (track.MetaDataUpdated != true)
                {
                    // open spotify authentication page if user is disconnected
                    // user might be connected with a different account that the one that granted rights
                    OpenAuthenticationDialog(true);           
                }
                return res;
            }

            if (hasNoPlayback || playback.HasError())
            {
                _api.Dispose();

                // fallback in case getting the playback did not work
                ExternalAPI.Instance = _lastFmApi;
                Settings.Default.app_selected_external_api_id = (int) ExternalAPIType.LastFM;
                Settings.Default.Save();

                _ = Task.Run(() =>
                {
                    FrmEspionSpotify.Instance.UpdateExternalAPIToggle(ExternalAPIType.LastFM);
                    FrmEspionSpotify.Instance.ShowFailedToUseSpotifyAPIMessage();
                });

                return await _lastFmApi.UpdateTrack(track);
            }

            // prevent changing track metadata with invalid ones (unknown track)
            if (!IsPlaybackTrackDetectedTrack(track, playback.Item))
            {
                if (retryDone) return false;
                
                await Task.Delay(1000);
                return await UpdateTrack(track, retryDone: true);
            }

            MapSpotifyTrackToTrack(track, playback.Item);

            if (playback.Item.Album?.Id == null) return false;

            var album = await _api.GetAlbumWithoutExceptionAsync(playback.Item.Album.Id);

            if (album.HasError()) return false;

            MapSpotifyAlbumToTrack(track, album);

            return true;
        }

        #endregion Spotify Track updater

        private string[] GetAlbumArtistFromSimpleArtistList(List<SimpleArtist> artists)
        {
            return (artists ?? new List<SimpleArtist>()).Select(a => a.Name).ToArray();
        }

        private bool IsPlaybackTrackDetectedTrack(Track track, FullTrack spotifyTrack)
        {
            var (titleParts, separatorType) = SpotifyStatus.GetTitleTags(spotifyTrack.Name, 2);
            return titleParts.FirstOrDefault() == track.Title;
        }

        private async void AuthOnAuthReceived(object sender, AuthorizationCode payload)
        {
            _authorizationCodeAuth = (AuthorizationCodeAuth) sender;

            _authorizationCodeAuth.Stop();

            try
            {
                _token = await _authorizationCodeAuth.ExchangeCode(payload.Code);
                _refreshToken = _token.RefreshToken;
                _connectionDialogOpened = false;
            }
            catch
            {
                // ignored
            }
        }

        private void OpenAuthenticationDialog(bool refresh = false)
        {
            if (_connectionDialogOpened) return;

            if (refresh)
            {
                _auth.Stop();
                _token = null;
                _auth.Start();
            }

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
                try
                {
                    if (_api != null) _api.Dispose();
                    _api = null;
                    _token = await _authorizationCodeAuth.RefreshToken(_token.RefreshToken ?? _refreshToken);
                }
                catch
                {
                    // ignored
                }

            if (_api == null)
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

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
                if (_api != null)
                    _api.Dispose();

            _disposed = true;
        }
    }
}