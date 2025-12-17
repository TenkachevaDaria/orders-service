namespace PaymentService.Application.DTOs;

public class AccountDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public decimal Balance { get; set; }
}
