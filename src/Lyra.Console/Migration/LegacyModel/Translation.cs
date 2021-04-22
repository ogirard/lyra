using System.Xml.Linq;

namespace Lyra.Console.Migration.LegacyModel
{
    public class Translation
    {
        public string Id { get; set; }

        public string Language { get; set; }

        public bool EnableUnform { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public static Translation ParseXml(XElement translationNode)
            => new()
            {
                Id = translationNode.GetAttributeValue("id"),
                Language = translationNode.GetAttributeValue("lang"),
                EnableUnform = translationNode.GetAttributeBoolValue("unform") ?? false,
                Title = translationNode.GetValue("Title").CleanText(),
                Text = translationNode.GetValue("Text").CleanText(),
            };
    }
}