using EspionSpotify.Enums;
using EspionSpotify.Models;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify.API
{
    public class SpotifyAPI : ISpotifyAPI, IExternalAPI, IDisposable
    {
        private bool _disposed = false;
        private readonly string _clientId;
        private readonly string _secretId;
        private AuthorizationCodeTokenResponse _token;
        private SpotifyClientConfig _config;
        private readonly LastFMAPI _lastFmApi;
        private readonly EmbedIOAuthServer _server;
        private string _accessToken;
        private string _refreshToken;
        private SpotifyClient _spotify;
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
                _config = SpotifyClientConfig.CreateDefault();
                var uri = new Uri(redirectUrl);
                _server = new EmbedIOAuthServer(uri, uri.Port);
                _server.AuthorizationCodeReceived += AuthOnAuthReceived;
                _server.ErrorReceived += OnErrorReceived;
                Task.Run(StartServer);
            }
        }

        public async Task<(string, bool)> GetCurrentPlayback()
        {
            var playing = false;
            string title = null;

            await GetSpotifyWebAPI();
            
            if (_spotify != null)
            {
                var playback = await _spotify.GetPlaybackAsync();
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

            var playback = await _api.GetPlaybackAsync();
            var hasNoPlayback = playback == null || playback.Item == null;

            if (!retry && hasNoPlayback)
            {
                await Task.Delay(3000);
                await UpdateTrack(track, retry: true);
                return;
            }

            if (hasNoPlayback || playback.HasError())
            {
                _api.Dispose();

                // open spotify authentication page if user is disconnected
                // user might be connected with a different account that the one that granted rights
                OpenAuthenticationDialog();

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
            
            var album = await _api.GetAlbumAsync(playback.Item.Album.Id);

            if (album.HasError()) return;
                
            MapSpotifyAlbumToTrack(track, album);

            track.MetaDataUpdated = true;
        }
        #endregion Spotify Track updater

        private string[] GetAlbumArtistFromSimpleArtistList(List<SimpleArtist> artists) => (artists ?? new List<SimpleArtist>()).Select(a => a.Name).ToArray();

        private async Task StartServer()
        {
            await _server.Start();
            var loginRequest = new LoginRequest(_server.BaseUri, _clientId, LoginRequest.ResponseType.Code)
            {
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
            };
            BrowserUtil.Open(loginRequest.ToUri());
        }

        private async Task AuthOnAuthReceived(object sender, AuthorizationCodeResponse response)
        {
            await _server.Stop();
            var token = await new OAuthClient(_config).RequestToken(new AuthorizationCodeTokenRequest(
              _clientId, _secretId, response.Code, _server.BaseUri
            ));

            _refreshToken = token.RefreshToken;

            var config = SpotifyClientConfig
              .CreateDefault()
              .WithAuthenticator(new AuthorizationCodeAuthenticator(_clientId, _secretId, token));

            _spotify = new SpotifyClient(config);
        }

        private async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"// Aborting Spotify API authorization, error received: {error}");
            await _server.Stop();
        }

        private void OpenAuthenticationDialog()
        {
            if (_connectionDialogOpened) return;
            var request = new LoginRequest(_server.BaseUri, _clientId, LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { Scopes.UserReadEmail }
            };
            BrowserUtil.Open(request.ToUri());
            _connectionDialogOpened = true;
        }

        private async Task GetSpotifyWebAPI()
        {
            if (_token == null)
            {
                OpenAuthenticationDialog();
                return;
            }

            if (_token.IsExpired)
            {
                try
                {
                    var token = await new OAuthClient().RequestToken(
                      new AuthorizationCodeRefreshRequest(_clientId, _secretId, _refreshToken)
                    );
                    _token.ExpiresIn = token.ExpiresIn;
                    _token.CreatedAt = token.CreatedAt;
                    _token.AccessToken = token.AccessToken;
                    _spotify = new SpotifyClient(_token.AccessToken);
                }
                catch { }
            }

            if (_spotify == null)
            {
                try
                {
                    _spotify = new SpotifyClient(_token.AccessToken);
                }
                catch (Exception)
                {
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
                if (_server != null) _server.Dispose();
            }

            _disposed = true;
        }
    }
}