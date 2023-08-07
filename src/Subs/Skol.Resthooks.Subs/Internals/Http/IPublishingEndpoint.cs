namespace Skol.Resthooks.Subs.Internals.Http;

public interface IPublishingEndpoint
{
    ValueTask PublishAsync<TContent>(TContent content, Action<HttpRequestMessage> configure, CancellationToken cancellationToken = default);
}
