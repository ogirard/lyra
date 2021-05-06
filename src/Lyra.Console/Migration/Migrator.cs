using System.IO;
using System.Linq;
using Lyra.Console.Migration.LegacyModel;
using Lyra.Features.Config;
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
        private readonly LyraOptions lyraOptions;
        private readonly ISongRepository songRepository;
        private readonly IStyleRepository styleRepository;
        private readonly IPresenterConfigService presenterConfigService;

        public Migrator(
            ILogger<Migrator> logger,
            IConfiguration configuration,
            IOptionsMonitor<LyraOptions> lyraOptions,
            ISongRepository songRepository,
            IStyleRepository styleRepository,
            IPresenterConfigService presenterConfigService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.lyraOptions = lyraOptions.CurrentValue;
            this.songRepository = songRepository;
            this.styleRepository = styleRepository;
            this.presenterConfigService = presenterConfigService;
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
                    BackgroundColor = "#FF5B3656",
                    ForegroundColor = "#FFFFFFFF",
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

            var screens = presenterConfigService.GetScreens().ToList();
            var index = 0;
            foreach (var screen in screens)
            {
                System.Console.WriteLine($"[{index}] {screen.DeviceName} (bounds: {screen.Bounds})");
                index++;
            }

            System.Console.WriteLine("Choose default screen: ");
            var key = System.Console.ReadKey();
            if (int.TryParse(key.ToString(), out var selectedIndex))
            {
                if (selectedIndex >= screens.Count || selectedIndex < 0)
                {
                    selectedIndex = 0;
                }
            }

            var selectedScreen = screens[selectedIndex];
            presenterConfigService.SelectPresenterScreen(screens[selectedIndex]);
            logger.LogInformation(
                $"Configured {selectedScreen.DeviceName} (bounds: {selectedScreen.Bounds}) as default");
            return legacyStore;
        }
    }
}