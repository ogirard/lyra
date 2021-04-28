using System.Collections.Generic;
using Lyra.Features.Songs;

namespace Lyra.Features.Search
{
    public interface ISearchService
    {
        IReadOnlyList<SearchResult> Search(string query, IReadOnlyCollection<string> tags, IEnumerable<Song> songs);
    }
}