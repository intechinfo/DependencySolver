using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Abstractions;

namespace Invenietis.DependencyCrawler.IO
{
    public class FeedProvider : IFeedProvider
    {
        public FeedProvider( IReadOnlyCollection<string> feeds )
        {
            Feeds = feeds;
        }

        public IReadOnlyCollection<string> Feeds { get; }
    }
}
