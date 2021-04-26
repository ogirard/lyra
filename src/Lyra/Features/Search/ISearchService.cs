using System.Collections.Generic;
using Lyra.Features.Songs;

namespace Lyra.Features.Search
{
    public interface ISearchService
    {
        IReadOnlyList<SearchResult> Search(string query, IEnumerable<string> tags, IEnumerable<Song> songs);
    }
}