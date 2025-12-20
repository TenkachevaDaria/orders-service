using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Application.Services.DTOs;

namespace OrderService.API;

[Tags("Заказы")]
[ApiController]
[Route("api/orders")]
public class OrdersController : Controller
{
    private IOrdersPublisher _ordersPublisher;
    private readonly IOrderService _orderService;

    public OrdersController(IOrdersPublisher ordersPublisher, IOrderService orderService)
    {
        _ordersPublisher = ordersPublisher;
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderRequest request, CancellationToken cancellationToken)
    {
       var idResult = await _orderService.CreateOrder(request);
       if (!idResult.Succeeded)
       {
           return BadRequest(idResult.Errors);
       }
       request.OrderId = idResult.Data;
       await _ordersPublisher.PublishOrderCreatedAsync(request, cancellationToken);
       return Accepted($"/order/{idResult.Data}", new { idResult.Data });
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var result = await _orderService.GetOrders();
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }
    
    [HttpGet("{id}")]
        public async Task<IActionResult> GetOrders(Guid id)
        {
            var result = await _orderService.GetOrdersById(id);
            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
}
    