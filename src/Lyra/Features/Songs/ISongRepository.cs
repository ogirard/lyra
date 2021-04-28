using System.Collections.Generic;

namespace Lyra.Features.Songs
{
    public interface ISongRepository
    {
        Song GetSong(string id);

        IReadOnlyCollection<Song> GetSongs();

        void AddSong(Song song);

        void RemoveSong(Song song);
    }
}