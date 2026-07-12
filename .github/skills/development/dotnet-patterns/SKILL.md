---
name: dotnet-patterns
description: Standard .NET/C# patterns for this repo — DI, immutable models, async I/O, options, error handling. Use before writing services or models.
---

# .NET Patterns

> **Related skills:** `development/implementation-workflow`, `development/code-checklist`

## Pattern 1 — Dependency Injection
Register services in `Program.cs`/`Startup` via the built-in container with an appropriate lifetime (`Singleton`/`Scoped`/`Transient`). Inject via constructor — prefer **primary constructors**. No `static` singletons, no service locator.

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();

// Primary-constructor injection
public sealed class OrderService(IOrderRepository repo, ILogger<OrderService> logger)
{
    public Task<Order?> GetAsync(int id, CancellationToken ct) => repo.GetAsync(id, ct);
}
```

## Pattern 2 — Immutable models
Use `record` for value objects/DTOs; validate invariants in the constructor.

```csharp
public sealed record Money(decimal Amount, string Currency)
{
    public Money { if (Amount < 0) throw new ArgumentOutOfRangeException(nameof(Amount)); }
}
```

## Pattern 3 — Async I/O end-to-end
All I/O `async`; pass `CancellationToken` through. Never `.Result`/`.Wait()`/`.GetAwaiter().GetResult()`.

```csharp
public async Task<Order?> GetAsync(int id, CancellationToken ct) =>
    await _db.Orders.FindAsync([id], ct);
```

## Pattern 4 — Options
Configuration via strongly-typed options (`IOptions<T>`), not loose `IConfiguration` reads deep in the code. Validate at startup with data-annotation attributes so bad config fails fast.

```csharp
builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection("Smtp"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

## Pattern 5 — Error handling
- Throw specific exceptions; catch only what you can handle.
- Validate input at the edge (model validation / guard clauses).
- Money = `decimal`. Timestamps = `DateTimeOffset`.

## Pattern 6 — Results over exceptions (optional)
For expected error paths: a `Result<T>` type instead of exceptions for control flow.

## Pattern 7 — Structured logging
Use `Microsoft.Extensions.Logging` with **message templates** (not string interpolation) so log data stays queryable; add scopes for context. No `Console.WriteLine`.

```csharp
logger.LogInformation("Order {OrderId} placed by {UserId}", order.Id, userId);
using (logger.BeginScope("Checkout {CartId}", cart.Id)) { /* ... */ }
```

## Pattern 8 — Disposal
Own an `IDisposable`/`IAsyncDisposable`? Dispose it — `using`/`await using`, or implement the pattern and let DI manage the lifetime. **Unsubscribe from events you subscribe to** (a common leak).

## Pattern 9 — Collection initialization
When the target type is obvious, prefer concise collection initialization (`[]`) and target-typed `new()` for elements instead of repeating full type names.

```csharp
public static List<WhatsNewEntry> Items =>
[
    new() { Id = 2, Version = "1.1.0", PageType = typeof(WhatsNew110Page) },
    new() { Id = 3, Version = "1.2.0", PageType = typeof(WhatsNew120Page) },
];
```

## Anti-patterns (avoid)
- `async void` (except event handlers).
- Swallowed exceptions (`catch {}`).
- God classes / static mutable state.
- Over-abstraction: no interface with a single implementation "just in case".
