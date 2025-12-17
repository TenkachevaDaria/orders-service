using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Domain.Common;

public class BaseEntity : IEntity
{
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("created_at")]
    public DateTime? CreatedDate { get; set; } = DateTime.Now.ToUniversalTime();

    [Column("updated_at")]
    public DateTime? UpdatedDate { get; set; } = DateTime.Now.ToUniversalTime();
}
