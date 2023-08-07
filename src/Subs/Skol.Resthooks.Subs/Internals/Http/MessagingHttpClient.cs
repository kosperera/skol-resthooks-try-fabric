using System.Text;
using System.Text.Json;

namespace Skol.Resthooks.Subs.Internals.Http;

internal sealed class MessagingHttpClient : IPublishingEndpoint
{
    readonly HttpClient _https;
    readonly JsonObjectSerializer _serializer;

    public MessagingHttpClient(HttpClient https, JsonObjectSerializer serializer)
    {
        _https = https;
        _serializer = serializer;
    }

    public async ValueTask PublishAsync<TContent>(TContent content, Action<HttpRequestMessage> configure, CancellationToken cancellationToken = default)
        where TContent : notnull
    {
        using HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "v1/messaging/publish");

        configure.Invoke(req);

        req.Content = new StringContent(
            _serializer.Serialize(content),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage res = await _https.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        res.EnsureSuccessStatusCode();
    }
}
