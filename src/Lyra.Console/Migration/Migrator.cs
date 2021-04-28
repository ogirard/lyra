using System;
using System.IO;
using System.Linq;
using Lyra.Console.Migration.LegacyModel;
using Lyra.Features.Songs;
using Lyra.Features.Styles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        public void Cleanup()
        {
            var dbPath = new FileInfo(databaseOptions.ConnectionString.Replace("Filename=", string.Empty).Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
            if (dbPath.Exists)
            {
                dbPath.Delete();
                logger.LogInformation($"Deleted {dbPath.FullName}");

                var dbLogPath = new FileInfo(dbPath.FullName.Replace(".db", "-log.db"));
                if (dbLogPath.Exists)
                {
                    dbLogPath.Delete();
                    logger.LogInformation($"Deleted {dbLogPath.FullName}");
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