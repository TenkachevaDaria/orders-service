using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;

namespace PaymentService.Controllers;

[Tags("Аккаунты")]
[ApiController]
[Route("api/accounts")]
public class AccountController(IAccountService accounts) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var data = await accounts.GetAccountsAsync();
        return Ok(data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var data = await accounts.DeleteAccountAsync(id);
        return data.Succeeded ? NoContent() : StatusCode(422, data);
    }

    [HttpPatch("money/{id:guid}")]
    public async Task<IActionResult> AddMoney(Guid id, [FromQuery] decimal money)
    {
        var data = await accounts.AddMoneyAsync(id, money);
        return data.Succeeded ? NoContent() : StatusCode(422, data);
    }
    
    [HttpPost()]
    public async Task<IActionResult> CreateAccountAsync([FromBody] CreateAccountRequest request)
    {
        var data = await accounts.CreateAccountAsync(request);
        return data.Succeeded ? Created() : StatusCode(422, data);
    }
}
