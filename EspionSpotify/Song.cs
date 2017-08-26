namespace EspionSpotify
{
    internal class Song
    {
        public readonly string Artist;
        public readonly string Title;

        public Song(string artist, string title)
        {
            Artist = artist;
            Title = title;
        }

        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Song)) return false;

            var otherSong = (Song) obj;
            return otherSong.Artist.Equals(Artist) && otherSong.Title.Equals(Title);
        }

        protected bool Equals(Song other)
        {
            return string.Equals(Artist, other.Artist) && string.Equals(Title, other.Title);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Artist != null ? Artist.GetHashCode() : 0) * 397) ^ (Title != null ? Title.GetHashCode() : 0);
            }
        }
    }
}
