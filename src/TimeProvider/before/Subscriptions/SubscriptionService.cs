namespace Subscriptions;

public class SubscriptionService
{
    public bool IsActive(Subscription sub) =>
        DateTime.UtcNow < sub.ExpiresAt;

    // Two separate reads of the clock: "now" can move between them
    public bool IsInGracePeriod(Subscription sub) =>
        DateTime.UtcNow >= sub.ExpiresAt &&
        DateTime.UtcNow < sub.ExpiresAt.AddDays(3);

    // Used for "your subscription expires today" reminders
    public bool ExpiresToday(Subscription sub) =>
        sub.ExpiresAt.Date == DateTime.UtcNow.Date;
}
