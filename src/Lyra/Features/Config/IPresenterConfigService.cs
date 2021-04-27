using System.Collections.Generic;

namespace Lyra.Features.Config
{
    public interface IPresenterConfigService
    {
        IReadOnlyCollection<PresenterScreen> GetScreens();

        PresenterScreen GetSelectedPresenterScreen();

        void SelectPresenterScreen(PresenterScreen presenterScreen);
    }
}