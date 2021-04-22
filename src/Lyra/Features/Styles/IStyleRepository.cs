using System.Collections.Generic;

namespace Lyra.Features.Styles
{
    public interface IStyleRepository
    {
        PresentationStyle GetStyle(string id);

        IReadOnlyCollection<PresentationStyle> GetStyles();

        void AddStyle(PresentationStyle style);

        void RemoveStyle(PresentationStyle style);
    }
}