namespace Lyra.Features.Styles
{
    public class PresentationStyle
    {
        public string Id { get; set; }

        public BodyStyle Body { get; set; } = new();

        public TitleStyle Title { get; set; } = new();
    }
}