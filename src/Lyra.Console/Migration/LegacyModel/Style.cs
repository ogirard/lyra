using System.Xml.Linq;

namespace Lyra.Console.Migration.LegacyModel
{
    public class Style
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public StyleForeground Foreground { get; set; }

        public StyleBackground Background { get; set; }

        public StyleTitle Title { get; set; }

        public class StyleForeground
        {
            public string Color { get; set; }

            public string Font { get; set; }

            public decimal FontSize { get; set; }

            public static StyleForeground ParseXml(XElement foregroundNode)
                => foregroundNode == null ? null : new()
                {
                    Color = foregroundNode.GetValue("color"),
                    Font = foregroundNode.GetValue("font"),
                    FontSize = foregroundNode.GetDecimalValue("fontsize") ?? 0m,
                };
        }

        public class StyleBackground
        {
            public string Color { get; set; }

            public BackgroundImage Image { get; set; }

            public static StyleBackground ParseXml(XElement backgroundNode)
                => backgroundNode == null ? null : new()
                {
                    Color = backgroundNode.GetValue("color"),
                    Image = BackgroundImage.ParseXml(backgroundNode.Element("image")),
                };

            public class BackgroundImage
            {
                public string Uri { get; set; }

                public decimal Transparency { get; set; }

                public bool Scale { get; set; }

                public static BackgroundImage ParseXml(XElement backgroundImageNode)
                    => backgroundImageNode == null ? null : new()
                    {
                        Uri = backgroundImageNode.GetValue("uri"),
                        Transparency = backgroundImageNode.GetDecimalValue("transparency") ?? 0m,
                        Scale = backgroundImageNode.GetBoolValue("scale") ?? false,
                    };
            }
        }

        public class StyleTitle
        {
            public string Mode { get; set; }

            public StyleForeground Foreground { get; set; }

            public StyleBackground Background { get; set; }

            public static StyleTitle ParseXml(XElement titleNode)
                => titleNode == null ? null : new()
                {
                    Mode = titleNode.GetAttributeValue("mode"),
                    Foreground = StyleForeground.ParseXml(titleNode.Element("Foreground")),
                    Background = StyleBackground.ParseXml(titleNode.Element("Background")),
                };
        }

        public static Style ParseXml(XElement styleNode)
            => styleNode == null ? null : new()
            {
                Id = styleNode.GetAttributeValue("id"),
                Name = styleNode.GetAttributeValue("name"),
                IsDefault = styleNode.GetAttributeBoolValue("isdefault") ?? false,
                Foreground = StyleForeground.ParseXml(styleNode.Element("Foreground")),
                Background = StyleBackground.ParseXml(styleNode.Element("Background")),
                Title = StyleTitle.ParseXml(styleNode.Element("Title")),
            };
    }
}