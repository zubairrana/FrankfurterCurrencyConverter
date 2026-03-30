namespace CurrencyConverter.BusinessLogic.Common
{
    public sealed class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string Key { get; init; } = default!;
        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public int ExpiryMinutes { get; init; }
    }
}
