using System.Xml.Linq;

namespace Lyra.Console.Migration.LegacyModel
{
    public class Song
    {
        public string Id { get; set; }

        public string TranslationId { get; set; }

        public string Addition { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public static Song ParseXml(XElement songNode) =>
            new()
            {
                Id = songNode.GetAttributeValue("id"),
                TranslationId = songNode.GetAttributeValue("trans"),
                Addition = songNode.GetAttributeValue("zus"),
                Number = songNode.GetIntValue("Number") ?? -1,
                Title = songNode.GetValue("Title").CleanText(),
                Text = songNode.GetValue("Text").CleanText(),
            };
    }
}