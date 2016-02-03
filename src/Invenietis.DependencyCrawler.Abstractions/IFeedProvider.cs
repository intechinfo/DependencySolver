using System.Collections.Generic;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface IFeedProvider
    {
        IReadOnlyCollection<string> Feeds { get; }
    }
}