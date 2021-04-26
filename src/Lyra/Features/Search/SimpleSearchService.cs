using System;
using System.Collections.Generic;
using System.Linq;
using Lyra.Features.Songs;

namespace Lyra.Features.Search
{
    public class SimpleSearchService : ISearchService
    {
        public IReadOnlyList<SearchResult> Search(string query, IEnumerable<string> tags, IEnumerable<Song> songs)
        {
            var tagList = tags?.ToList() ?? new List<string>();
            return songs.Select(s => FilterSong(query, tagList, s)).ToList();
        }

        private static SearchResult FilterSong(string query, List<string> tags, Song song)
        {
            var normalizedQuery = query?.ToLowerInvariant() ?? string.Empty;

            if (!tags.Any() || tags.Any(t => song.Tags.Contains(t, StringComparer.InvariantCultureIgnoreCase)))
            {
                var words = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var numbers = words.Where(w => int.TryParse(w, out _)).Select(int.Parse).ToList();
                if (string.IsNullOrEmpty(normalizedQuery)
                    || numbers.Contains(song.Number)
                    || song.Title.ToLowerInvariant().Contains(normalizedQuery)
                    || song.Text.ToLowerInvariant().Contains(normalizedQuery))
                {
                    return new SearchResult { IsMatch = true, Rank = 1m, Song = song };
                }
            }

            return new SearchResult { IsMatch = false, Rank = 0m, Song = song };
        }
    }
}