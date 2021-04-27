using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Lyra.Features.Config
{
    public class PresenterConfigService : IPresenterConfigService
    {
        private readonly ILogger<PresenterConfig> logger;
        private readonly ILiteRepository dbRepository;
        private readonly string sessionCollectionName;

        public PresenterConfigService(ILogger<PresenterConfig> logger, ILiteRepository dbRepository)
        {
            this.logger = logger;
            this.dbRepository = dbRepository;
            sessionCollectionName = this.dbRepository.Database.GetCollection<PresenterConfig>().Name;
        }

        public IReadOnlyCollection<PresenterScreen> GetScreens()
        {
            return Screen.AllScreens.Select(s => new PresenterScreen
            {
                DeviceName = s.DeviceName,
                Bounds = s.Bounds,
                IsPrimary = s.Primary,
            }).ToList();
        }

        public PresenterScreen GetSelectedPresenterScreen()
        {
            var config = dbRepository.FirstOrDefault<PresenterConfig>(x => x.Id == PresenterConfig.ConfigId);
            var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName == config?.SelectedScreen)
                         ?? Screen.AllScreens.First(x => x.Primary);

            return new PresenterScreen
            {
                DeviceName = screen.DeviceName,
                Bounds = screen.Bounds,
                IsPrimary = screen.Primary,
            };
        }

        public void SelectPresenterScreen(PresenterScreen presenterScreen)
        {
            var config = dbRepository.FirstOrDefault<PresenterConfig>(x => x.Id == PresenterConfig.ConfigId) ?? new PresenterConfig();
            config.SelectedScreen = presenterScreen.DeviceName;
            dbRepository.Upsert(config);
        }
    }
}