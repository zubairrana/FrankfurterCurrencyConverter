using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CurrencyConverter.API.Configurations
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $"My API {description.GroupName.ToUpper()}",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated ? "This API version is deprecated" : null
                });
            }

            //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    Name = "Authorization",
            //    Type = SecuritySchemeType.Http,
            //    Scheme = "bearer",
            //    BearerFormat = "JWT",
            //    In = ParameterLocation.Header
            //});

            //options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        { 
            //            Reference = new OpenApiReference
            //            {
            //                Type = ReferenceType.SecurityScheme,
            //                Id = "Bearer"
            //            }
            //        },
            //        Array.Empty<string>()
            //    }
            //});
        }
    }
}
