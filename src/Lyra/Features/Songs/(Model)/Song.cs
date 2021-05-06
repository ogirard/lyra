using System.Collections.Generic;
using LiteDB;

namespace Lyra.Features.Songs
{
    public class Song
    {
        public string Id { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string StyleId { get; set; }

        public List<string> Tags { get; set; } = new();

        [BsonIgnore]
        public string DisplayText => $"{Number} - {Title} (id: {Id})";
    }
}