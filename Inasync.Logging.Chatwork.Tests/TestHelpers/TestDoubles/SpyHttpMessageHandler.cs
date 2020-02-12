using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inasync.Logging.Chatwork.Tests {

    internal sealed class SpyHttpMessageHandler : HttpMessageHandler {
        private readonly Func<HttpResponseMessage> _result;

        public SpyHttpMessageHandler(Func<HttpResponseMessage>? result = null) {
            _result = result ?? (() => new HttpResponseMessage(HttpStatusCode.OK));
        }

        public (HttpRequestMessage request, string content) ActualRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            ActualRequest = (request, request.Content.ReadAsStringAsync().Result);
            return Task.FromResult(_result());
        }
    }
}
