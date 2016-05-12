using System;
using System.Threading.Tasks;

namespace OAuthJwtAssertionTokenClient
{
    /// <summary>
    /// Token cache
    /// </summary>
    public interface ITokenCache
    {
        /// <summary>
        /// Returns token from cache or fetches it from the underlying source if it is not cached
        /// </summary>
        /// <typeparam name="T">Token type</typeparam>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="underlyingSource">Underlying source</param>
        /// <returns>Token</returns>
        Task<T> GetAsync<T>(string cacheKey, Func<Task<Tuple<T, TimeSpan>>> underlyingSource);
    }
}
