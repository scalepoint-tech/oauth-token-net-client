using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Scalepoint.OAuth.TokenClient.Cache
{
    public class NoCache : IDistributedCache
    {
        public byte[] Get(string key)
        {
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
            return null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null
        }

        public Task<byte[]> GetAsync(string key)
        {
            return Task.FromResult((byte[])null);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            // Method intentionally left empty.
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            return Task.FromResult(0);
        }

        public void Refresh(string key)
        {
            // Method intentionally left empty.
        }

        public Task RefreshAsync(string key)
        {
            return Task.FromResult(0);
        }

        public void Remove(string key)
        {
            // Method intentionally left empty.
        }

        public Task RemoveAsync(string key)
        {
            return Task.FromResult(0);
        }
    }
}
