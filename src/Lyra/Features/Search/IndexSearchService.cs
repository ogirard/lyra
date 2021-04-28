using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Lucene.Net.QueryParsers.Classic;
using Lyra.Features.Songs;
using Microsoft.Extensions.Logging;

namespace Lyra.Features.Search
{
    /// <summary>
    /// Search Utils
    /// </summary>
    public class IndexSearchService : ISearchService
    {
        public const int NumberBoost = 4;
        public const int TitleBoost = 2;
        public const int TextBoost = 1;

        private readonly ILogger<IndexSearchService> logger;
        private readonly SearchIndex searchIndex;

        private static HashSet<string> stopWords;

        public static HashSet<string> StopWords => stopWords ??= InitStopWords();

        private static HashSet<string> InitStopWords()
        {
            var result = new HashSet<string>(1024);
            using (var streamReader = new StreamReader(typeof(IndexSearchService).Assembly.GetManifestResourceStream("Lyra.Features.Search.stopwords.txt")))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    var stopWord = line.Trim().ToLowerInvariant();
                    if (!result.Contains(stopWord))
                    {
                        result.Add(stopWord);
                    }
                }
            }

            return result;
        }

        public IndexSearchService(ILogger<IndexSearchService> logger, SearchIndex searchIndex)
        {
            this.logger = logger;
            this.searchIndex = searchIndex;
        }

        private static (IReadOnlyCollection<string> Parts, IReadOnlyCollection<int> Numbers) GetQueryParts(string query, bool removeStopWords = true)
        {
            var parts = new List<string>();
            var numbers = new List<int>();
            var part = string.Empty;
            var isQuoteOpen = false;
            foreach (var character in query.Trim(' '))
            {
                switch (character)
                {
                    case '\"':
                        if (isQuoteOpen)
                        {
                            isQuoteOpen = false;
                            AddPart();
                        }
                        else
                        {
                            isQuoteOpen = true;
                        }

                        break;
                    case ' ':
                        if (isQuoteOpen)
                        {
                            part += character;
                        }
                        else
                        {
                            AddPart();
                        }

                        break;
                    default:
                        part += character;
                        break;
                }
            }

            AddPart();

            void AddPart()
            {
                part = part.ToLowerInvariant();
                if (int.TryParse(part, out var number))
                {
                    numbers.Add(number);
                }
                else if (!removeStopWords || !StopWords.Contains(part))
                {
                    parts.Add(part);
                }

                part = string.Empty;
            }

            return (parts, numbers);
        }

        private (string Query, IReadOnlyCollection<int> Numbers) CreateIndexQuery(string query)
        {
            var (parts, numbers) = GetQueryParts(query);

            var indexQuery = string.Join(
                $" {Operator.OR:G} ",
                numbers.Select(n => $"{SearchIndex.IndexFieldNumber}:{n}^{NumberBoost}")
                    .Concat(parts.Select(p => $"{SearchIndex.IndexFieldTitle}:{p}^{TitleBoost} {Operator.OR:G} {SearchIndex.IndexFieldText}:{p}^{TextBoost}")));
            logger.LogTrace($"Translated query '{query}' to index query '{indexQuery}'");
            return (indexQuery, numbers);
        }

        public IReadOnlyList<SearchResult> Search(string query, IReadOnlyCollection<string> tags, IEnumerable<Song> songs)
        {
            tags ??= new List<string>();

            if (string.IsNullOrEmpty(query) && tags.Count == 0)
            {
                return ArraySegment<SearchResult>.Empty;
            }

            var stopwatch = Stopwatch.StartNew();
            var tagFilteredSongs = tags?.Any() ?? false
                ? songs.Where(s => !tags.Any() || tags.Any(t => s.Tags.Contains(t, StringComparer.InvariantCultureIgnoreCase)))
                : songs;
            var (indexQuery, numbers) = CreateIndexQuery(query);
            var indexResults = searchIndex.Search(indexQuery);
            var results = indexResults
                .Select(r => new SearchResult
                {
                    Song = tagFilteredSongs.FirstOrDefault(x => x.Id == r.SongId),
                    IsMatch = true,
                    Score = r.Score,
                }).Where(x => x.Song != null).OrderByDescending(x => x.Score).ToList();
            stopwatch.Stop();
            logger.LogTrace($"{nameof(IndexSearchService)}: Found {results.Count} results for '{query}' (tags: {string.Join(',', tags)}) in {stopwatch.Elapsed:g}");
            return results;
        }
    }
}