using Subscriptions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SubscriptionService>();

var app = builder.Build();

#region ── Seed data ──
var subscriptions = new List<Subscription>
{
    new() { Id = "active",   ExpiresAt = DateTime.UtcNow.AddDays(30) },
    new() { Id = "grace",    ExpiresAt = DateTime.UtcNow.AddDays(-1) },
    new() { Id = "expired",  ExpiresAt = DateTime.UtcNow.AddDays(-10) },
};
#endregion

#region ── Endpoints ──
app.MapGet("/subscriptions/{id}", (string id, SubscriptionService service) =>
{
    var sub = subscriptions.FirstOrDefault(s => s.Id == id);
    if (sub is null)
        return Results.NotFound();

    return Results.Ok(new
    {
        sub.Id,
        sub.ExpiresAt,
        IsActive = service.IsActive(sub),
        IsInGracePeriod = service.IsInGracePeriod(sub),
        ExpiresToday = service.ExpiresToday(sub),
    });
});
#endregion

app.Run();
