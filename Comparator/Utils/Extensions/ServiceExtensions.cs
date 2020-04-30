using Comparator.Utils.Logger;
using Microsoft.Extensions.DependencyInjection;

namespace Comparator.Utils.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
    }
}