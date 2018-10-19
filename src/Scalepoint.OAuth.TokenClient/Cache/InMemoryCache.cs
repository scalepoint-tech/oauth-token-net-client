using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Scalepoint.OAuth.TokenClient.Internals;

namespace Scalepoint.OAuth.TokenClient.Cache
{
    public sealed class InMemoryCache<T> : ICache<T>, IDisposable
    {
        private readonly MemoryCache _memoryCache = new MemoryCache("InMemoryTokenCache");

        public Task<T> GetOrCreateAsync(string key, Func<CancellationToken, Task<Tuple<T, TimeSpan>>> factory, CancellationToken token)
        {
            return _memoryCache.GetOrCreateAsync(key, factory, token);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~InMemoryCache()
        {
            Dispose(false);
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _memoryCache.Dispose();

                _disposed = true;
            }
        }
    }
}
