namespace Subscriptions;

public class Subscription
{
    public required string Id { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
