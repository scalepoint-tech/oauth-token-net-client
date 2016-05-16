using System;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient.Cache
{
    /// <summary>
    /// Token cache
    /// </summary>
    public interface ITokenCache
    {
        /// <summary>
        /// Returns token from cache or fetches it from the underlying source if it is not cached
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="underlyingSource">Underlying source</param>
        /// <returns>Token</returns>
        Task<string> GetAsync(string cacheKey, Func<Task<Tuple<string, TimeSpan>>> underlyingSource);
    }
}
