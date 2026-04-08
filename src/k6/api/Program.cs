var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

# region ── Seed data ──
var products = new List<Product>
{
    new(1, "Laptop",        999.99m, "Electronics"),
    new(2, "Desk Chair",    249.99m, "Furniture"),
    new(3, "Coffee Maker",   49.99m, "Kitchen"),
    new(4, "Headphones",    149.99m, "Electronics"),
    new(5, "Standing Desk", 399.99m, "Furniture"),
    new(6, "Keyboard",       89.99m, "Electronics"),
    new(7, "Monitor",       329.99m, "Electronics"),
    new(8, "Notebook",        4.99m, "Stationery"),
};
# endregion

#region  ── Endpoints ──

// Returns the full product catalogue
app.MapGet("/products", () => Results.Ok(products));

// Always returns a product — uses the id as a key into the catalogue (id % count)
// Simulate requests that takes >500 ms to demonstrate threshold breaches in K6
var requestCount = 0;

app.MapGet("/products/{id:int}", async (int id) =>
{
    var count = Interlocked.Increment(ref requestCount);

    var delay = count % 25 == 0
        ? Random.Shared.Next(600, 900)
        : Random.Shared.Next(20, 150);

    await Task.Delay(delay);

    var template = products[Math.Abs(id) % products.Count];
    return Results.Ok(template with { Id = id });
});
#endregion

app.Run();

// ── Model ─────────────────────────────────────────────────────────────────────
record Product(int Id, string Name, decimal Price, string Category);
