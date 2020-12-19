using EspionSpotify.Enums;
using EspionSpotify.Models;
using EspionSpotify.Properties;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EspionSpotify.MediaTags
{
    public class SpotifyAPI : ISpotifyAPI, IExternalAPI
    {
        private readonly string _clientId;
        private readonly string _secretId;
        private Token _token;
        private DateTimeOffset _nextTokenRenewal;
        private AuthorizationCodeAuth _authorizationCodeAuth;
        private readonly LastFMAPI _lastFmApi;
        private readonly AuthorizationCodeAuth _auth;
        private bool _connectionDialogOpened = false;

        public bool IsAuthenticated { get => _token != null; }

        public SpotifyAPI() { }

        public SpotifyAPI(string clientId, string secretId, string redirectUrl = "http://localhost:4002")
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

        private void OpenAuthenticationDialog()
        {
            _auth.ShowDialog = true;
            _auth.OpenBrowser();
            _connectionDialogOpened = true;
        }

        public async Task<bool> UpdateTrack(Track track, string forceQueryTitle = null)
        {
            var api = await GetSpotifyWebAPI();

            if (api == null)
            {
                OpenAuthenticationDialog();
                return false;
            }

            PlaybackContext playback = null;
            
            try
            {
                playback = await api.GetPlaybackAsync();
            }
            catch (Exception ex)
            {
                var x = ex;
            }

            if (playback == null || playback.Item == null)
            {
                return false;
            }

            if (playback.HasError())
            {
                api.Dispose();

                // open spotify authentication page if user is disconnected
                // user might be connected with a different account that the one that granted rights
                if (!_connectionDialogOpened)
                {
                    OpenAuthenticationDialog();
                }

                // fallback in case getting the playback did not work
                Settings.Default.MediaTagsAPI = (int)MediaTagsAPI.LastFM;
                Settings.Default.Save();

                _ = Task.Run(() =>
                  {
                      FrmEspionSpotify.Instance.UpdateMediaTagsAPIToggle(MediaTagsAPI.LastFM);
                      FrmEspionSpotify.Instance.ShowFailedToUseSpotifyAPIMessage();
                  });

                var lastFmApiResult = await _lastFmApi.UpdateTrack(track);

                return lastFmApiResult;
            }

            MapSpotifyTrackToTrack(track, playback.Item);

            if (playback.Item.Album?.Id == null)
            {
                api.Dispose();
                return false;
            }

            try
            {
                var album = await api.GetAlbumAsync(playback.Item.Album.Id);

                if (album.HasError())
                {
                    api.Dispose();
                    return false;
                }

                MapSpotifyAlbumToTrack(track, album);

                api.Dispose();
            }
            catch (Exception ex)
            {
                var x = ex;
            }


            return true;
        }

        public void MapSpotifyTrackToTrack(Track track, FullTrack spotifyTrack)
        {
            var performers = GetAlbumArtistFromSimpleArtistList(spotifyTrack.Artists);
            track.Artist = performers.FirstOrDefault();
            track.Title = spotifyTrack.Name;
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

        private string[] GetAlbumArtistFromSimpleArtistList(List<SimpleArtist> artists) => (artists ?? new List<SimpleArtist>()).Select(a => a.Name).ToArray();

        private void RenewNextTokenRenewal()
        {
            // remember when to renew the 60 minutes token (10 minutes upfront)
            _nextTokenRenewal = DateTimeOffset.UtcNow.AddSeconds(_token.ExpiresIn).AddMinutes(-10);
        }

        private async void AuthOnAuthReceived(object sender, AuthorizationCode payload)
        {
            _authorizationCodeAuth = (AuthorizationCodeAuth)sender;
            try
            {
                _authorizationCodeAuth.Stop();

                _token = await _authorizationCodeAuth.ExchangeCode(payload.Code);
            }
            catch (Exception ex)
            {
                var x = ex;
            }

            RenewNextTokenRenewal();
        }

        private async Task<SpotifyWebAPI> GetSpotifyWebAPI()
        {
            if (_token == null) return null;

            if (DateTimeOffset.UtcNow >= _nextTokenRenewal || _token.IsExpired())
            {
                try
                {
                    _token = await _authorizationCodeAuth.RefreshToken(_token.RefreshToken);
                }
                catch (Exception ex)
                {
                    var x = ex;
                }
                RenewNextTokenRenewal();
            }

            if (_token.HasError())
            {
                FrmEspionSpotify.Instance.WriteIntoConsole(I18nKeys.LogUnknownException, $"{_token.Error} {_token.ErrorDescription}");
                return null;
            }

            SpotifyWebAPI api = null;
            try
            {
                api = new SpotifyWebAPI
                {
                    AccessToken = _token.AccessToken,
                    TokenType = _token.TokenType
                };
            }
            catch (Exception ex)
            {
                var x = ex;
            }

            return api;
        }
    }
}