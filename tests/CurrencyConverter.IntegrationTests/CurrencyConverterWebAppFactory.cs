using CurrencyConverter.API;
using CurrencyConverter.BusinessLogic.Common;
using CurrencyConverter.Domain.Interfaces;
using CurrencyConverter.Infrastructure.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CurrencyConverter.IntegrationTests
{
    public class CurrencyConverterWebAppFactory : WebApplicationFactory<Program>
    {
        public Mock<ICurrencyProvider> CurrencyProviderMock { get; } = new();
        public Mock<ICurrencyProviderFactory> CurrencyProviderFactoryMock { get; } = new();

        public CurrencyConverterWebAppFactory() 
        {
            CurrencyProviderFactoryMock.Setup(x=>x.GetProvider(It.IsAny<string?>())).Returns(CurrencyProviderMock.Object);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    [$"{JwtSettings.SectionName}:{nameof(JwtSettings.Key)}"] = "q9WZQ7W2yLk0bR5T2H8Yp9mXc3KkVnZz4YfZr8U6XyA=",
                    [$"{JwtSettings.SectionName}:{nameof(JwtSettings.Issuer)}"] = "CurrencyConverterAPI",
                    [$"{JwtSettings.SectionName}:{nameof(JwtSettings.Audience)}"] = "CurrencyConverterClients",
                    [$"{JwtSettings.SectionName}:{nameof(JwtSettings.ExpiryMinutes)}"] = "30",
                    
                    [$"{CurrencyProviderApiSettings.SectionName}:" +
                    $"{nameof(CurrencyProviderApiSettings.Frankfurter)}:" +
                    $"{nameof(CurrencyProviderApiSettings.Frankfurter.BaseUrl)}"] = "https://api.frankfurter.dev/v2/",
                    
                    [$"{CurrencyProviderApiSettings.SectionName}:" +
                    $"{nameof(CurrencyProviderApiSettings.Frankfurter)}:" +
                    $"{nameof(CurrencyProviderApiSettings.Frankfurter.CacheTimeMinutes)}"] = "10",
                });
            });

            builder.ConfigureServices(services =>
            {
                // Replace the factory with our mock so no real HTTP calls are made
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICurrencyProviderFactory));
                if (descriptor is not null) services.Remove(descriptor);

                services.AddSingleton(CurrencyProviderFactoryMock.Object);
            });
        }
    }
}
