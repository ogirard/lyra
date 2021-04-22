using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Lyra.Console.Migration.LegacyModel
{
    public class SongList
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Date { get; set; }

        public List<string> SongIds { get; set; }

        public static SongList ParseXml(XElement listNode)
            => listNode == null ? null : new()
            {
                Title = listNode.GetValue("Title").CleanText(),
                Author = listNode.GetValue("Author").CleanText(),
                Date = listNode.GetValue("Date"),
                SongIds = listNode.GetValue("Songs")?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList() ?? new List<string>(),
            };
    }
}