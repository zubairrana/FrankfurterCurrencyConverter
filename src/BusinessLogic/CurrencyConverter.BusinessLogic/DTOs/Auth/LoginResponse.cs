namespace CurrencyConverter.BusinessLogic.DTOs.Auth
{
    public record LoginResponse(
        string Token,
        DateTime ExpiresAt
        );
}
