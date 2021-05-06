using System.Drawing;

namespace Lyra.Features.Config
{
    public class PresenterScreen
    {
        public string DisplayName => $"Bildschirm {ScreenId + 1}{(IsPrimary ? " (Primär)" : string.Empty)}";

        public int ScreenId { get; set; }

        public string DeviceName { get; set; }

        public Rectangle Bounds { get; set; }

        public bool IsPrimary { get; set; }
    }
}