using System.Collections.Concurrent;
using Grpc.Net.Client;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Gateway.API
{
    public class GrpcChannelPool : IDisposable
    {
        private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new();
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public GrpcChannelPool()
        {
            // Retry 3 lần, delay 300ms giữa mỗi lần
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(300));

            // Circuit breaker: sau 5 lỗi thì ngắt 10s
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(10));
        }

        public GrpcChannel GetOrCreateChannel(string address)
        {
            return _channels.GetOrAdd(address, addr =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                var channel = GrpcChannel.ForAddress(addr, new GrpcChannelOptions
                {
                    HttpHandler = handler,
                    DisposeHttpClient = true
                });

                Console.WriteLine($"[GrpcChannelPool] Created channel: {addr}");
                return channel;
            });
        }

        public async Task<TResult> ExecuteWithPolicyAsync<TResult>(
            string address,
            Func<GrpcChannel, Task<TResult>> action)
        {
            var channel = GetOrCreateChannel(address);

            return await _retryPolicy
                .WrapAsync(_circuitBreakerPolicy)
                .ExecuteAsync(() => action(channel));
        }

        public void Dispose()
        {
            foreach (var kv in _channels)
            {
                kv.Value.Dispose();
            }
            _channels.Clear();
        }
    }
}