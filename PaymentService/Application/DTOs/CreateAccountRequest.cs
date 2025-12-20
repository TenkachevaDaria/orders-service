namespace PaymentService.Application.DTOs;

public class CreateAccountRequest
{
    public string FullName { get; set; }
    public decimal Balance { get; set; }
}