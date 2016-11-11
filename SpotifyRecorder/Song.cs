using System;

namespace EspionSpotify
{
    class Song
    {
        public string Artist;
        public string Title;

        public Song(string artist, string title)
        {
            Artist = artist;
            Title = title;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Artist, Title);
        }

        public override bool Equals(Object obj)
        {
            if(obj != null && obj.GetType() == typeof(Song))
            {
                Song otherSong = (Song) obj;
                return otherSong.Artist.Equals(Artist) && otherSong.Title.Equals(Title);
            }
            return false;
        }

        
    }
}
