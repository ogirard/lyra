using System;
using System.IO;
using Lyra.Console.Migration.LegacyModel;
using Lyra.Features.Songs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lyra.Console.Migration
{
    public class Migrator
    {
        private readonly ILogger<Migrator> logger;
        private readonly DatabaseOptions databaseOptions;
        private readonly ISongRepository songRepository;

        public Migrator(ILogger<Migrator> logger, IOptionsMonitor<DatabaseOptions> dataOptions, ISongRepository songRepository)
        {
            this.logger = logger;
            this.databaseOptions = dataOptions.CurrentValue;
            this.songRepository = songRepository;
        }

        public LegacySongStore InitializeLegacyStore()
        {
            var legacyStore = LegacySongStore.ParseXml(@".\Migration\Data\lyrasongs.xml", @".\Migration\Data\lyrastyles.xml", @".\Migration\Data\lists.xml");
            foreach (var song in legacyStore.Songs)
            {
                songRepository.AddSong(new()
                {
                    Id = song.Id,
                    Number = song.Number,
                    Title = song.Title,
                    Text = song.Text,
                });
            }

            return legacyStore;
        }
    }
}