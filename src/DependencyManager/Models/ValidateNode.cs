using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invenietis.DependencyCrawler.Core;

namespace DependencyManager.Models
{
    public class ValidateNode
    {
        public PackageId PackageId { get; set; }

        public VPackageId VPackageId { get; set; }
    }
}
