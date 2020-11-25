using System;
using DistributedCache.DistributedCacheFile;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedCache
{
    /// <summary>
    ///
    /// </summary>
    public static class DistributedCacheFileExtensions
    {
        /// <summary>
        /// Add distributed cache using File (efs) => load balancing
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddDistributedCacheFile(this IServiceCollection services, IConfiguration configuration)
        {
            //Add Distributed Cache File
            var section = configuration.GetSection(nameof(DistributedCacheFileConfig));
            if (section.Exists())
            {
                services.Configure<DistributedCacheFileConfig>(options =>section.Bind(options));
                services.AddSingleton<IDistributedCache, DistributedCacheFile.DistributedCache>();
            }
            else
            {
                var ex = new ArgumentNullException(nameof(AddDistributedCacheFile), $"Section {nameof(DistributedCacheFileConfig)} null in file appsettings.json");
              
                //todo send notify
                throw ex;
            }

            return services;
        }
    }
}