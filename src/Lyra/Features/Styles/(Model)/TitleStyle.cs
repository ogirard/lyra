namespace Lyra.Features.Styles
{
    public class TitleStyle
    {
        public TitleMode Mode { get; set; } = TitleMode.Hide;

        public string ForegroundColor { get; set; }

        public string BackgroundColor { get; set; }

        public float FontSize { get; set; }

        public string TitleFont { get; set; }

        public enum TitleMode
        {
            Hide,
            ShowTitleOnly,
            ShowNumberAndTitle,
        }
    }
}