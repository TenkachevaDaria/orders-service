using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Persistence;
using Rebus.Bus;

namespace PaymentService.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processing failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();

        var messages = await db.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(20)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            await ProcessMessageAsync(message, bus, ct);
        }

        await db.SaveChangesAsync(ct);
    }
    
    private async Task ProcessMessageAsync(
        OutboxMessage message,
        IBus bus,
        CancellationToken ct)
    {
        try
        {
            var messageType = Type.GetType(message.Type, throwOnError: true)!;
            var payload = JsonSerializer.Deserialize(message.Payload, messageType)!;

            if (!string.IsNullOrEmpty(message.Destination))
            {
                await bus.Send(payload);
            }

            message.MarkProcessed();
        }
        catch (Exception ex)
        {
            message.MarkFailed(ex.ToString());
        }
    }
}