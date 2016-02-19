using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Invenietis.DependencyCrawler.Abstractions;
using Invenietis.DependencyCrawler.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace DependencyManager
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MvcJsonOptions>(o =>
            {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddMvc();
            
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config["Data:AzureStorage:ConnectionString"];
            string packageTable = config["Data:AzureStorage:PackageTable"];
            string vPackageTable = config["Data:AzureStorage:VPackageTable"];
            string notCrawledVPackageTable = config["Data:AzureStorage:NotCrawledVPackageTable"];
            string vPackageCacheBlobContainer = config["Data:AzureStorage:VPackageCacheBlobContainer"];

            services.AddTransient<IPackageRepository, AzureTablePackageRepository>(sp => new AzureTablePackageRepository(connectionString, packageTable, vPackageTable, notCrawledVPackageTable, vPackageCacheBlobContainer, TimeSpan.FromHours(10)));

            services.AddTransient<IPackageSerializer, XmlPackageSerializer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
