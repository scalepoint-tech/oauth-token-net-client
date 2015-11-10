using System;
using System.Threading.Tasks;

namespace OAuthJwtAssertionTokenClient
{
    public interface IReadThroughCache
    {
        Task<T> GetAsync<T>(string cacheKey, Func<Task<Tuple<T, TimeSpan>>> originalTokenSource);
    }
}
