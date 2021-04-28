using System;
using System.IO;
using System.Linq;
using Lyra.Console.Migration.LegacyModel;
using Lyra.Features.Songs;
using Lyra.Features.Styles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Core;

namespace Lyra.Console.Migration
{
    public class Migrator
    {
        private readonly ILogger<Migrator> logger;
        private readonly IConfiguration configuration;
        private readonly DatabaseOptions databaseOptions;
        private readonly ISongRepository songRepository;
        private readonly IStyleRepository styleRepository;

        public Migrator(ILogger<Migrator> logger, IConfiguration configuration, IOptionsMonitor<DatabaseOptions> dataOptions, ISongRepository songRepository, IStyleRepository styleRepository)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.databaseOptions = dataOptions.CurrentValue;
            this.songRepository = songRepository;
            this.styleRepository = styleRepository;
        }

        public static void Cleanup(string dbPath, Serilog.ILogger logger)
        {
            var dbFile = new FileInfo(dbPath);
            if (dbFile.Exists)
            {
                dbFile.Delete();
                logger.Information($"Deleted {dbFile.FullName}");

                var dbLogPath = new FileInfo(dbFile.FullName.Replace(".db", "-log.db"));
                if (dbLogPath.Exists)
                {
                    dbLogPath.Delete();
                    logger.Information($"Deleted {dbLogPath.FullName}");
                }
            }
        }

        public LegacySongStore InitializeLegacyStore()
        {
            var legacyStore = LegacySongStore.ParseXml(@".\Migration\Data\lyrasongs.xml", @".\Migration\Data\lyrastyles.xml", @".\Migration\Data\lists.xml");

            var defaultStyle = new PresentationStyle
            {
                Id = "Style/1",
                Body = new BodyStyle
                {
                    BackgroundColor = "#FF000000",
                    ForegroundColor = "#FFFFFFFF",
                    FontSize = 25,
                    NormalFont = "Roboto",
                    SpecialFont = "Roboto Slab",
                    MutedFont = "Roboto Mono",
                    UseBackgroundImage = false,
                },
                Title = new TitleStyle
                {
                    BackgroundColor = "#FFCCCCCC",
                    ForegroundColor = "#FFB76BAB",
                    FontSize = 35,
                    Mode = TitleStyle.TitleMode.ShowNumberAndTitle,
                    TitleFont = "Roboto Slab",
                },
            };
            styleRepository.AddStyle(defaultStyle);

            var songId = 1;
            foreach (var song in legacyStore.Songs.OrderBy(s => s.Number))
            {
                songRepository.AddSong(new()
                {
                    Id = $"Song/{songId++}",
                    Number = song.Number,
                    Title = song.Title,
                    Text = song.Text,
                    StyleId = defaultStyle.Id,
                });
            }

            return legacyStore;
        }
    }
}