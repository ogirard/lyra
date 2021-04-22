using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Lyra.Features.Styles
{
    public class StyleRepository : IStyleRepository
    {
        private readonly ILogger<StyleRepository> logger;
        private readonly ILiteRepository dbRepository;
        private readonly ILiteDatabase db;
        private readonly string styleCollectionName;

        public StyleRepository(ILogger<StyleRepository> logger, ILiteRepository dbRepository)
        {
            this.logger = logger;
            this.dbRepository = dbRepository;
            this.db = dbRepository.Database;
            styleCollectionName = this.dbRepository.Database.GetCollection<PresentationStyle>().Name;
        }

        public PresentationStyle GetStyle(string id)
            => dbRepository.SingleOrDefault<PresentationStyle>(s => s.Id == id);

        public IReadOnlyCollection<PresentationStyle> GetStyles()
            => dbRepository.Fetch<PresentationStyle>(_ => true).ToList();

        public void AddStyle(PresentationStyle style)
        {
            dbRepository.Insert(style);
            logger.LogTrace($"Added {nameof(PresentationStyle)} '{style.Id}' to '{styleCollectionName}' collection");
        }

        public void RemoveStyle(PresentationStyle style)
            => dbRepository.Delete<PresentationStyle>(style.Id);
    }
}