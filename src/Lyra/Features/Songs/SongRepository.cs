using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Lyra.Features.Songs
{
    public class SongRepository : ISongRepository
    {
        private readonly ILogger<SongRepository> logger;
        private readonly ILiteRepository dbRepository;
        private readonly string songCollectionName;

        public SongRepository(ILogger<SongRepository> logger, ILiteRepository dbRepository)
        {
            this.logger = logger;
            this.dbRepository = dbRepository;
            songCollectionName = this.dbRepository.Database.GetCollection<Song>().Name;
        }

        public Song GetSong(string id)
            => dbRepository.SingleOrDefault<Song>(s => s.Id == id);

        public IReadOnlyCollection<Song> GetSongs()
        {
            return dbRepository.Fetch<Song>(_ => true).ToList();
        }

        public void AddSong(Song song)
        {
            dbRepository.Insert(song);
            logger.LogTrace($"Added '{song.Id}' to '{songCollectionName}' collection");
        }

        public void RemoveSong(Song song)
            => dbRepository.Delete<Song>(song.Id);
    }
}