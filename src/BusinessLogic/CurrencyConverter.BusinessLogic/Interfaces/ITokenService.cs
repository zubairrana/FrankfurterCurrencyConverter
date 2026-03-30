using CurrencyConverter.BusinessLogic.DTOs.Auth;

namespace CurrencyConverter.BusinessLogic.Interfaces
{
    public interface ITokenService
    {
        LoginResponse GenerateToken(string userId, string role);
    }
}
