using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comparator.Services;
using Comparator.Utils.Extensions;
using Comparator.Utils.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Comparator {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.ConfigureLoggerService();
            services.ConfigureSwagger();
            services.ConfigureConfigLoader();
            services.ConfigureElasticSearchService();
            services.ConfigureClassifier();
            services.ConfigureWatsonService();
            services.ConfigureDataAnalyser();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(opts => {
                opts.SwaggerEndpoint("/swagger/APIDoc/swagger.json", "Comparator Web API");
                opts.RoutePrefix = "docs/web-api";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}