using Microsoft.AspNet.Mvc;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Invenietis.DependencyCrawler.IO;

namespace DependencyManager.Controllers
{
    [Route("request")]
    public class RootsController : Controller
    {
        private readonly IPackageRepository _packRepo;
        private readonly IPackageSerializer _packSeria;

        public RootsController(IPackageRepository packRepo, IPackageSerializer packSeria)
        {
            _packRepo = packRepo;
            _packSeria = packSeria;
        }

        [HttpGet("listRoots")]
        public async Task<JsonResult> GetlistRoots()
        {
            IEnumerable<PackageId> listPackage = await _packRepo.GetAllPackageIds();
            List<string[]> listPackageSorted = new List<string[]>();

            foreach (PackageId package in listPackage)
            {
                listPackageSorted.Add(new string[] { package.Value, package.PackageManager });
            }

            JsonResult result = new JsonResult(listPackageSorted);
            return result;
        }

        [HttpGet("RootPackage/{name}")]
        public async Task<JsonResult> GetRootPackage(string name)
        {
            PackageId packageId = new PackageId(PackageId.NuGet, name);

            Package package = await _packRepo.GetPackageById(packageId);

            //string XmlLastRelease = "";
            //string XmlLastPreRelease = "";

            //if (package.LastRelease != null)
            //{
            //    XmlLastRelease = _packSeria.Serialize(package.LastRelease);
            //}

            //if(package.LastPreRelease != null)
            //{
            //    XmlLastPreRelease = _packSeria.Serialize(package.LastPreRelease);
            //}

            return new JsonResult(package);
        }
    }
}
