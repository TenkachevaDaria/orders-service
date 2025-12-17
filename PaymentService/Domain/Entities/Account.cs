using PaymentService.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Domain.Entities;

public class Account : BaseEntity
{
    [Column("balance")]
    public decimal Balance { get; set; }
    [Column("fullname")]
    public string FullName { get; set; }
}
