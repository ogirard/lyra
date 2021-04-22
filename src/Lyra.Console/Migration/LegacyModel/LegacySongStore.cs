using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Lyra.Console.Migration.LegacyModel
{
    public class LegacySongStore
    {
        public List<Song> Songs { get; set; }

        public List<Translation> Translations { get; set; }

        public List<Style> Styles { get; set; }

        public List<SongList> Lists { get; set; }

        public static LegacySongStore ParseXml(string lyraSongsPath, string stylesPath, string listsPath)
        {
            var lyraSongsXmlContent = File.ReadAllText(lyraSongsPath);
            var stylesXmlContent = File.ReadAllText(stylesPath);
            var listsXmlContent = File.ReadAllText(listsPath);

            var lyraSongsXml = XElement.Parse(lyraSongsXmlContent);
            var stylesXml = XElement.Parse(stylesXmlContent);
            var listsXml = XElement.Parse(listsXmlContent);

            return new()
            {
                Songs = lyraSongsXml.XPathSelectElements("//Song").Select(Song.ParseXml).ToList(),
                Translations = lyraSongsXml.XPathSelectElements("//Translation").Select(Translation.ParseXml).ToList(),
                Styles = stylesXml.XPathSelectElements("//Style").Select(Style.ParseXml).ToList(),
                Lists = listsXml.XPathSelectElements("//List").Select(SongList.ParseXml).ToList(),
            };
        }
    }
}