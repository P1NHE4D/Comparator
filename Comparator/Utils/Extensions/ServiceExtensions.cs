using Comparator.Services;
using Comparator.Utils.Configuration;
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

        public static void ConfigureConfigLoader(this IServiceCollection services) {
            services.AddSingleton<IConfigLoader, ConfigLoader>();
        }

        public static void ConfigureElasticSearchService(this IServiceCollection services) {
            services.AddSingleton<IElasticSearchService, ElasticSearchService>();
        }
        
        public static void ConfigureClassifier(this IServiceCollection services) {
            services.AddSingleton<IClassifier, Classifier>();
        }

        public static void ConfigureWatsonService(this IServiceCollection services) {
            services.AddSingleton<IWatsonService, WatsonService>();
        }

        public static void ConfigureDataAnalyser(this IServiceCollection services) {
            services.AddSingleton<IDataAnalyser, DataAnalyser>();
        }
    }
}