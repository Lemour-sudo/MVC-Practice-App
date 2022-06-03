using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GraphQL.Server; // GraphQLOptions
using NorthwindGraphQL; // GreetQuery, NorthwindSchema
using Microsoft.EntityFrameworkCore;
using Packt.Shared; // AddNorthwindContext extension method

namespace NorthwindGraphQL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string databasePath = Path.Combine("../database", "Northwind.db");
            services.AddDbContext<Northwind>(
                options => options
                    .UseSqlite($"Data Source={databasePath}")
                    .UseLoggerFactory(new ConsoleLoggerFactory())
            );

            services.AddControllers();

            services.AddScoped<NorthwindSchema>();

            services.AddGraphQL()
                .AddGraphTypes(typeof(NorthwindSchema), ServiceLifetime.Scoped)
                .AddDataLoader()
                .AddSystemTextJson(); // serialize responses as JSON
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseGraphQLPlayground(); // default path is /ui/playground
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseGraphQL<NorthwindSchema>(); // default path is /graphql
            app.UseHttpsRedirection();
        }
    }
}
