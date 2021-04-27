using System.Drawing;

namespace Lyra.Features.Config
{
    public class PresenterScreen
    {
        public string DeviceName { get; set; }

        public Rectangle Bounds { get; set; }

        public bool IsPrimary { get; set; }
    }
}