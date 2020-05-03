using Comparator.Services;
using Comparator.Utils.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Comparator.Utils.Extensions {
    public static class ServiceExtensions {
        public static void ConfigureLoggerService(this IServiceCollection services) {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureSwagger(this IServiceCollection services) {
            services.AddSwaggerGen(opts => {
                opts.SwaggerDoc("APIDoc", new OpenApiInfo {
                    Title = "Comparator Web API",
                    Version = "v1.0",
                    Description = "Comparator Web API Documentation"
                });
            });
        }

        public static void ConfigureHttpRequestSender(this IServiceCollection services) {
            services.AddSingleton<IHttpRequestSender, HttpRequestSender>();
        }
    }
}