using System;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient
{
    public class NoCache : ITokenCache
    {
        public async Task<string> GetAsync(string cacheKey, Func<Task<Tuple<string, TimeSpan>>> underlyingSource)
        {
            return (await underlyingSource()).Item1;
        }
    }
}