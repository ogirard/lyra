namespace Lyra.Features.Styles
{
    public class BodyStyle
    {
        public string ForegroundColor { get; set; }

        public string BackgroundColor { get; set; }

        public float FontSize { get; set; }

        public string NormalFont { get; set; }

        public string SpecialFont { get; set; }

        public string MutedFont { get; set; }

        public bool UseBackgroundImage { get; set; }

        public ImageScaling BackgroundImageScaling { get; set; }

        public string BackgroundImagePath { get; set; }

        public float BackgroundImageOpacity { get; set; } = 1.0f;

        public enum ImageScaling
        {
            None,
            Fill,
            Uniform,
            UniformToFill,
        }
    }
}