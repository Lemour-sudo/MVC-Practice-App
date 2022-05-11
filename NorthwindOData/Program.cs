using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Packt.Shared;

namespace NorthwindOData
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://localhost:5004");
                });

        IEdmModel GetEdmModelForCatalog()
        {
            ODataConventionModelBuilder builder = new();
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Supplier>("Suppliers");
            return builder.GetEdmModel();
        }

        IEdmModel GetEdmModelForOrderSystem()
        {
            ODataConventionModelBuilder builder = new();
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<Employee>("Employees");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Shipper>("Shippers");
            return builder.GetEdmModel();
        }
    }
}
