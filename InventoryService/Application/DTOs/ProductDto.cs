using AutoMapper;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>();

        }
    }
}