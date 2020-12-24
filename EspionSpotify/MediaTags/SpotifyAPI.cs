using EspionSpotify.Enums;
using EspionSpotify.Models;
using EspionSpotify.Properties;
using EspionSpotify.Spotify;
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
        private AuthorizationCodeAuth _authorizationCodeAuth;
        private readonly LastFMAPI _lastFmApi;
        private readonly AuthorizationCodeAuth _auth;
        private string _refreshToken;
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

        public async Task UpdateTrack(Track track) => await UpdateTrack(track, retry: false);

        private async Task UpdateTrack(Track track, bool retry = false)
        {
            var api = await GetSpotifyWebAPI();

            if (api == null)
            {
                OpenAuthenticationDialog();
                return;
            }

            var playback = await api.GetPlaybackAsync();

            if (playback == null || playback.Item == null)
            {
                if (!retry)
                {
                    await Task.Delay(3000);
                    await UpdateTrack(track, retry: true);
                }
                return;
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

                await _lastFmApi.UpdateTrack(track);

                return;
            }

            MapSpotifyTrackToTrack(track, playback.Item);

            if (playback.Item.Album?.Id == null)
            {
                api.Dispose();
                return;
            }
            else
            {
                var album = await api.GetAlbumAsync(playback.Item.Album.Id);

                if (album.HasError())
                {
                    api.Dispose();
                    return;
                }
                else
                {
                    MapSpotifyAlbumToTrack(track, album);
                }
            }

            track.MetaDataUpdated = true;

            api.Dispose();
        }

        public void MapSpotifyTrackToTrack(Track track, FullTrack spotifyTrack)
        {
            var performers = GetAlbumArtistFromSimpleArtistList(spotifyTrack.Artists);
            var (titleParts, separatorType) = SpotifyStatus.GetTitleTags(spotifyTrack.Name, 2);

            track.SetArtistFromAPI(performers.FirstOrDefault());
            track.SetTitleFromAPI(SpotifyStatus.GetTitleTag(titleParts, 1));
            track.SetTitleExtendedFromAPI(SpotifyStatus.GetTitleTag(titleParts, 2));
            track.TitleExtendedSeparatorType = separatorType;

            track.Artist = performers.FirstOrDefault();
            track.Title = SpotifyStatus.GetTitleTag(titleParts, 1);
            track.TitleExtended = SpotifyStatus.GetTitleTag(titleParts, 2);
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

        private async void AuthOnAuthReceived(object sender, AuthorizationCode payload)
        {
            _authorizationCodeAuth = (AuthorizationCodeAuth)sender;

            _authorizationCodeAuth.Stop();

            _token = await _authorizationCodeAuth.ExchangeCode(payload.Code);
            _refreshToken = _token.RefreshToken;
        }

        private async Task<SpotifyWebAPI> GetSpotifyWebAPI()
        {
            if (_token == null) return null;

            if (_token.IsExpired())
            {
                _token = await _authorizationCodeAuth.RefreshToken(_token.RefreshToken ?? _refreshToken);
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
            catch (Exception)
            {
                _authorizationCodeAuth.Stop();
            }

            return api;
        }
    }
}