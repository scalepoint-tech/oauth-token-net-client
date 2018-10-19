using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient.Internals
{
    internal static class MemoryCacheExtensions
    {
        public static async Task<T> GetOrCreateAsync<T>(this MemoryCache cache, string key, Func<CancellationToken, Task<Tuple<T, TimeSpan>>> factory, CancellationToken token = default(CancellationToken))
        {
            var value = (T)cache.Get(key);

            if (value != null)
            {
                return value;
            }

            var result = await factory(token).ConfigureAwait(false);

            cache.Set(key, result.Item1, DateTimeOffset.Now.Add(result.Item2));

            return result.Item1;
        }
    }
}
