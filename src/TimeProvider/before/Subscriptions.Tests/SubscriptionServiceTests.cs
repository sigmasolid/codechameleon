namespace Subscriptions.Tests;

public class SubscriptionServiceTests
{
    private readonly SubscriptionService _sut = new();

    [Fact]
    public void IsActive_WhenExpiryIsInTheFuture_ReturnsTrue()
    {
        // We can't choose "now", so every date has to be built relative to it
        var sub = new Subscription { Id = "sub-1", ExpiresAt = DateTime.UtcNow.AddDays(1) };

        Assert.True(_sut.IsActive(sub));
    }

    [Fact]
    public void IsInGracePeriod_OneDayAfterExpiry_ReturnsTrue()
    {
        var sub = new Subscription { Id = "sub-1", ExpiresAt = DateTime.UtcNow.AddDays(-1) };

        Assert.True(_sut.IsInGracePeriod(sub));
    }

    [Fact]
    public void IsInGracePeriod_RightAtTheEndOfTheGracePeriod_ReturnsFalse()
    {
        // Grace period ends exactly now... or a few ticks ago... or a few ticks from now?
        // Time keeps moving between this line and the two DateTime.UtcNow calls inside the service.
        var sub = new Subscription { Id = "sub-1", ExpiresAt = DateTime.UtcNow.AddDays(-3) };

        Assert.False(_sut.IsInGracePeriod(sub));
    }

    [Fact]
    public void IsInGracePeriod_RightAfterExpiry_ReturnsTrue()
    {
        var sub = new Subscription { Id = "sub-1", ExpiresAt = DateTime.UtcNow.AddSeconds(2) };

        // The only way to move "now" past the expiry is to actually wait for it
        Thread.Sleep(TimeSpan.FromSeconds(3));

        Assert.True(_sut.IsInGracePeriod(sub));
    }

    [Fact(Skip = "flaky, only fails around midnight")]
    public void ExpiresToday_WhenExpiringInOneHour_ReturnsTrue()
    {
        // Fails whenever the test runs after 23:00 UTC - in one hour it's tomorrow
        var sub = new Subscription { Id = "sub-1", ExpiresAt = DateTime.UtcNow.AddHours(1) };

        Assert.True(_sut.ExpiresToday(sub));
    }

    // Can't be written at all without waiting for a real month-end or leap day:
    // public void ExpiresToday_OnLeapDay_ReturnsTrue() { ... }
}
