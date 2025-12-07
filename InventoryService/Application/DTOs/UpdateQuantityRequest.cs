using System.ComponentModel.DataAnnotations;

namespace InventoryService.Application.DTOs;

public class UpdateQuantityRequest
{
    [Required(ErrorMessage = "Количество обязательно.")]
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше нуля.")]
    public int Quantity { get; set; }
}