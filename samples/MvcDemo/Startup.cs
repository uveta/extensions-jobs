using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Uveta.Extensions.Jobs.Extensions;
using Uveta.Extensions.Jobs.Endpoints.Mvc.Extensions;
using Uveta.Extensions.Jobs.Workers.Extensions;
using MvcDemo.Models;
using MvcDemo.Jobs;

namespace MvcDemo
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
            services
                .AddMvc()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(
                        new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
                });

            services.AddJobs(jobs => jobs
                .AddDefaultRepository()
                .AddDefaultQueue()
                .AddControllerEndpoints(controllers => controllers
                    .AddController<PingEndpoint, PingRequest, PingResponse>(endpoint => endpoint
                        .UseService("demo")
                        .UseArea("ping")
                    )
                )
                .AddWorkers(workers => workers
                    .AddWorker<PingWorker, PingRequest, PingResponse>(worker => worker
                        .UseService("demo")
                        .UseArea("ping")
                        .UseMaximumAllowedExecutionTime(TimeSpan.FromMinutes(5))
                        .UseMaximumConcurrentCalls(10)
                    )
                )
            );

            services.AddSwaggerGen();
            services.AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jobs API V1");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
