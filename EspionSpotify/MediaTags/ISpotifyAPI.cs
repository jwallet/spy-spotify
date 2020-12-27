using EspionSpotify.Models;
using SpotifyAPI.Web.Models;

namespace EspionSpotify.MediaTags
{
    public interface ISpotifyAPI
    {
        void MapSpotifyTrackToTrack(Track track, FullTrack spotifyTrack);

        void MapSpotifyAlbumToTrack(Track track, FullAlbum spotifyAlbum);
    }
}
