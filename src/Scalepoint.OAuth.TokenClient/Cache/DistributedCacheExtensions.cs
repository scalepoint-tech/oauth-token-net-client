using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Scalepoint.OAuth.TokenClient.Cache
{
    public static class DistributedCacheExtensions
    {
        public static async Task<string> GetOrCreateStringAsync(this IDistributedCache cache, string key, Func<DistributedCacheEntryOptions, Task<string>> factory)
        {
            var result = await cache.GetStringAsync(key).ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            var options = new DistributedCacheEntryOptions();
            result = await factory(options).ConfigureAwait(false);
            await cache.SetStringAsync(key, result, options).ConfigureAwait(false);
            return result;
        }
    }
}
