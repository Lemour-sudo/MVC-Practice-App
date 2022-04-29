using System;
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
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Packt.Shared;
using NorthwindService.Repositories;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using static System.Console;

namespace NorthwindService
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
            services.AddCors();

            string databasePath = Path.Combine("../database", "Northwind.db");
            services.AddDbContext<Northwind>(options => 
                options.UseSqlite($"Data Source={databasePath}"));

            services.AddControllers(options =>
                {
                    WriteLine("\nDefault output formatters:");

                    foreach(IOutputFormatter formatter in options.OutputFormatters)
                    {
                        var mediaFormatter = formatter as OutputFormatter;
                        if (mediaFormatter == null)
                        {
                            WriteLine($" {formatter.GetType().Name}");
                        }
                        else // OutputFormatter class has SupportedMediaTypes
                        {
                            WriteLine(" {0}, Media types: {1}",
                            arg0: mediaFormatter.GetType().Name,
                            arg1: string.Join(", ",
                            mediaFormatter.SupportedMediaTypes));
                        }
                    }

                    WriteLine();
                })
                .AddXmlDataContractSerializerFormatters()
                .AddXmlSerializerFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NorthwindService API", Version = "v1" });
            });

            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NorthwindService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(configurePolicy: options =>
            {
                options.WithMethods("GET", "POST", "PUT", "DELETE");
                options.WithOrigins("https://localhost:5002");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options => 
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind Service API Version 1");
                options.SupportedSubmitMethods(new[] {
                    SubmitMethod.Get, SubmitMethod.Post,
                    SubmitMethod.Put, SubmitMethod.Delete
                });
            });
        }
    }
}
