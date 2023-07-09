using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Skol.Messaging.Egress.Internals.Http
{
    public sealed class WebhookHttpClient
    {
        readonly HttpClient _https;
        readonly JsonObjectSerializer _serializer;

        public WebhookHttpClient(HttpClient https, JsonObjectSerializer serializer)
        {
            _https = https;
            _serializer = serializer;
        }

        public async ValueTask PostJsonAsync<TContent>(string url, TContent content, Action<HttpRequestMessage> configure, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);

            configure.Invoke(req);

            req.Content = new StringContent(
                _serializer.Serialize(content),
                Encoding.UTF8,
                "application/json");

            using HttpResponseMessage res = await _https.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            res.EnsureSuccessStatusCode();
        }
    }
}
