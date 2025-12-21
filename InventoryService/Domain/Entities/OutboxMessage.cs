using System.Text.Json;
using InventoryService.Domain.Common;

namespace InventoryService.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    public DateTime OccurredOn { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public string Destination { get; set; } = default!;
    public DateTime? ProcessedOn { get; set; }
    public string? Error { get; set; }
    
    private OutboxMessage() { }

    public static OutboxMessage ForSend(object message, string destination)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException("Destination is required");

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            Type = message.GetType().AssemblyQualifiedName!,
            Payload = JsonSerializer.Serialize(message),
            Destination = destination
        };
    }
    
    public void MarkProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        Error = error;
    }
}