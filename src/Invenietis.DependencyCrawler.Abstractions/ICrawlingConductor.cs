using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace Invenietis.DependencyCrawler.Abstractions
{
    public interface ICrawlingConductor
    {
        void AddOutQueue( IOutJobQueue queue );

        IPackageRepository PackageRepository { get; }

        IInJobQueue Queue { get; }

        Task Start();
    }
}
