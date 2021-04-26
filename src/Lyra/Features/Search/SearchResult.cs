using System.Windows.Documents;
using Lyra.Features.Songs;

namespace Lyra.Features.Search
{
    public class SearchResult
    {
        public Song Song { get; set; }

        public bool IsMatch { get; set; }

        public decimal Rank { get; set; }
    }
}