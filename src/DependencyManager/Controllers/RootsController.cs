using Microsoft.AspNet.Mvc;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Invenietis.DependencyCrawler.IO;
using System.IO;
using System.Xml.Serialization;
using System;
using DependencyManager.Models;

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
        public async Task<string> GetRootPackage(string name)
        {
            PackageId packageId = new PackageId(PackageId.NuGet, name);

            Package package = await _packRepo.GetPackageById(packageId);

            string XmlLastRelease = "";
            string XmlLastPreRelease = "";

            if (package.LastRelease != null)
            {
                XmlLastRelease = _packSeria.Serialize(package.LastRelease);
            }

            if (package.LastPreRelease != null)
            {
                XmlLastPreRelease = _packSeria.Serialize(package.LastPreRelease);
            }

            return "<VPackages>" + XmlLastRelease + "\n" + XmlLastPreRelease + "<VPackages>";
        }

        [HttpPost("ListVersions")]
        public async Task<List<PackageLastVersion>> GetListVersions([FromBody] PackageId[] listPackages)
        {
            IReadOnlyDictionary<PackageId, CombinedVPackageId> lastVersions = await _packRepo.GetLastVersionsOf(listPackages);

            return lastVersions.Select(kv => new PackageLastVersion
            {
                Id = kv.Key.Value,
                PackageManager = kv.Key.PackageManager,
                LastVersions = kv.Value
            }).ToList();
        }

        [HttpPost("AddValidateNodes")]
        public async Task<bool> AddValidateNodes([FromBody] ValidateNode data)
        {
            return await _packRepo.AddValidateNodes( data.PackageId, data.VPackageId );
        }

        [HttpPost( "RemoveValidateNodes" )]
        public async Task<bool> RemoveValidateNodes( [FromBody] ValidateNode data )
        {
            return await _packRepo.RemoveValidateNodes( data.PackageId, data.VPackageId );
        }

        [HttpPost("GetValidateNodes")]
        public async Task<string> GetValidateNodes([FromBody] PackageId package )
        {
            return await _packRepo.GetValidateNodes( package );
        }
    }
}
