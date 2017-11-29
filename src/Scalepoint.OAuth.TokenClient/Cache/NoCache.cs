using System.Threading;
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

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            // Method intentionally left empty.
        }

        public void Refresh(string key)
        {
            // Method intentionally left empty.
        }

        public void Remove(string key)
        {
            // Method intentionally left empty.
        }

#if NETSTANDARD2_0
        public Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult((byte[])null);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(0);
        }
#else
        public Task<byte[]> GetAsync(string key)
        {
            return Task.FromResult((byte[])null);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            return Task.FromResult(0);
        }

        public Task RefreshAsync(string key)
        {
            return Task.FromResult(0);
        }

        public Task RemoveAsync(string key)
        {
            return Task.FromResult(0);
        }
#endif
    }
}
