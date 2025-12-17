using Microsoft.AspNetCore.Mvc;
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

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> AddMoney(Guid id, [FromQuery] decimal money)
    {
        var data = await accounts.AddMoneyAsync(id, money);
        return data.Succeeded ? NoContent() : StatusCode(422, data);
    }
}
