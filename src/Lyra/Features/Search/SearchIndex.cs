using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Flexible.Standard;
using Lucene.Net.QueryParsers.Flexible.Standard.Config;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lyra.Features.Songs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lyra.Features.Search
{
    public class SearchIndex : IHostedService
    {
        public const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        public const string IndexFieldId = "__ID";
        public const string IndexFieldNumber = "NUMBER";
        public const string IndexFieldTitle = "TITLE";
        public const string IndexFieldText = "TEXT";

        private readonly ILogger<SearchIndex> logger;
        private readonly ISongRepository songRepository;
        private readonly StandardQueryParser queryParser;
        private IndexWriter writer;

        public SearchIndex(ILogger<SearchIndex> logger, ISongRepository songRepository)
        {
            this.logger = logger;
            this.songRepository = songRepository;

            queryParser = new StandardQueryParser();
            queryParser.QueryConfigHandler.Set(ConfigurationKeys.ALLOW_LEADING_WILDCARD, true);
            queryParser.QueryConfigHandler.Set(ConfigurationKeys.ANALYZER, new WhitespaceAnalyzer(AppLuceneVersion));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            var songs = songRepository.GetSongs();
            var stopwatch = Stopwatch.StartNew();
            logger.LogTrace($"Indexing {songs.Count} songs at startup...");

            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            writer = new IndexWriter(new RAMDirectory(), indexConfig);

            writer.AddDocuments(songs.Select(GetLuceneDocument));
            writer.Flush(triggerMerge: false, applyAllDeletes: true);

            stopwatch.Stop();
            logger.LogTrace($"Done setting up index at startup. Indexing duration: {stopwatch.Elapsed:g}.");

            return Task.CompletedTask;
        }

        private static Document GetLuceneDocument(Song song) =>
            new()
            {
                new StringField(IndexFieldId, song.Id, Field.Store.YES),
                new StringField(IndexFieldNumber, song.Number.ToString("D"), Field.Store.NO),
                new StringField(IndexFieldTitle, song.Title, Field.Store.NO),
                new TextField(IndexFieldText, CleanText(song.Text), Field.Store.NO),
            };

        private static string CleanText(string text)
        {
            var cleanedText = string.Empty;
            var isTag = false;
            foreach (var c in text)
            {
                switch (c)
                {
                    case '<':
                        isTag = true;
                        break;
                    case '>':
                        isTag = false;
                        break;
                    default:
                        if (!isTag)
                        {
                            cleanedText += c;
                        }

                        break;
                }
            }

            return cleanedText;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            writer.Dispose();
            return Task.CompletedTask;
        }

        public IReadOnlyCollection<IndexResult> Search(string indexQuery)
        {
            using var reader = writer.GetReader(applyAllDeletes: true);
            var searcher = new IndexSearcher(reader);
            var searchQuery = queryParser.Parse(indexQuery, IndexFieldTitle);
            var hits = searcher.Search(searchQuery, int.MaxValue).ScoreDocs;

            return hits.Select(x =>
                    new IndexResult
                    {
                        SongId = searcher.Doc(x.Doc).Get(IndexFieldId),
                        Score = x.Score,
                    })
                .ToList();
        }
    }
}