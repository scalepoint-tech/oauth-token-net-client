using System;
using System.Threading;
using System.Threading.Tasks;
#if !NET45
using Microsoft.Extensions.Caching.Distributed;
#endif

namespace Scalepoint.OAuth.TokenClient.Cache
{
    public static class DistributedCacheExtensions
    {
        public static async Task<string> GetOrCreateStringAsync(this IDistributedCache cache, string key, Func<DistributedCacheEntryOptions, CancellationToken, Task<string>> factory, CancellationToken token = default(CancellationToken))
        {
            var result = await cache.GetStringAsync(key).ConfigureAwait(false);

            if (result != null)
            {
                return result;
            }

            var options = new DistributedCacheEntryOptions();
            result = await factory(options, token).ConfigureAwait(false);

            await cache.SetStringAsync(key, result, options).ConfigureAwait(false);

            return result;
        }
    }
}
