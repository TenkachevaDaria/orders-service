using AutoMapper;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.DTOs;

public class CreateProductRequest
{
    public string Name { get; set; }
    
    public int Quantity { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateProductRequest, Product>();
        }
    }
}