using AutoMapper;
using AutoMapper.QueryableExtensions;
using OrderService.Application.Common;
using OrderService.Application.Interfaces;
using OrderService.Application.Services.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Services;

public class OrdersService : IOrderService
{
    private readonly IOrderDbContext _orderDbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<OrdersService> _logger;

    public OrdersService(IOrderDbContext orderDbContext, IMapper mapper, ILogger<OrdersService> logger)
    {
        _orderDbContext = orderDbContext;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<Result<None>> UpdateOrderStatus(Guid orderId, OrderStatus orderStatus, string comment = "")
    {
        try
        {
            var order = _orderDbContext.Orders.Find(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order with id {orderId} not found", orderId);
                return await Result<None>.FailureAsync("Order not found");
            }

            order.Status = orderStatus;
            order.Comment = comment;
            await _orderDbContext.SaveChangesAsync();
            _logger.LogInformation("Order with id {orderId} updated", orderId);
            return await Result<None>.SuccessAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _logger.LogWarning($"Order with id {orderId} failed: {ex.Message}", orderId);
            return await Result<None>.FailureAsync(ex.Message);
        }
    }

    public async Task<Result<Guid>> CreateOrder(OrderRequest request)
    {
        try
        {
            var order = _mapper.Map<Order>(request);
            _orderDbContext.Orders.Add(order);
            await _orderDbContext.SaveChangesAsync();
            _logger.LogInformation("Order with id {orderId} added", order.Id);
            return await Result<Guid>.SuccessAsync(order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Order failed: {ex.Message}");
            return await Result<Guid>.FailureAsync(ex.Message);
        }
    }

    public async Task<Result<List<OrderDto>>> GetOrders()
    {
        var orders = _orderDbContext.Orders.ProjectTo<OrderDto>(_mapper.ConfigurationProvider).ToList();
        return await Result<List<OrderDto>>.SuccessAsync(orders);
    }
    
    public async Task<Result<List<OrderDto>>> GetOrdersById(Guid id)
    {
        var orders = _orderDbContext.Orders.Where(x => x.Id == id).ProjectTo<OrderDto>(_mapper.ConfigurationProvider).ToList();
        return await Result<List<OrderDto>>.SuccessAsync(orders);
    }
}