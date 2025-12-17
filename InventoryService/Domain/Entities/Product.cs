using System.ComponentModel.DataAnnotations.Schema;
using InventoryService.Domain.Common;

namespace InventoryService.Domain.Entities;

[Table("products")]
public class Product : BaseEntity
{
    [Column("name")]
    public string Name { get; set; }
    
    [Column("price")]
    public decimal Price { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
}