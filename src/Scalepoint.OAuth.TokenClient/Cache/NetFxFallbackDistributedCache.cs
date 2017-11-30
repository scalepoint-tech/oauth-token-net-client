using System;
using System.Threading.Tasks;

namespace Scalepoint.OAuth.TokenClient.Cache
{
#if NET45
    // A minimally required custom IDistributedCache elements for net451 target to avoid pulling in the whole NETStandard.Library
    using System.Runtime.Caching;

    public interface IDistributedCache
    {
        Task<string> GetStringAsync(string key);
        Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options);
    }

    public class FullFrameworkMemoryDistributedCache : IDistributedCache, IDisposable
    {
        private readonly MemoryCache _cache = new MemoryCache("FullFrameworkMemoryDistributedCache");

        public Task<string> GetStringAsync(string key)
        {
            return Task.FromResult((string)_cache.Get(key));
        }

        public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options)
        {
            var expirationDate = DateTimeOffset.Now + options.AbsoluteExpirationRelativeToNow;
            _cache.Set(key, value, expirationDate);
            return Task.FromResult(0);
        }

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FullFrameworkMemoryDistributedCache()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _cache.Dispose();
            }

            _disposed = true;
        }
    }

    public class DistributedCacheEntryOptions
    {
        public TimeSpan AbsoluteExpirationRelativeToNow { get; set; }
    }
#endif
}
