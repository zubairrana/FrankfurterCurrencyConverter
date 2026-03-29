using Asp.Versioning;
using CurrencyConverter.API.Configurations;
using CurrencyConverter.API.Extensions;
using CurrencyConverter.API.Middleware;
using CurrencyConverter.Infrastructure;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CurrencyConverter.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService("CurrencyConverter.API"))
                .WithTracing(tracing =>
                {
                    tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        options.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/traces");
                    });
                });

            // Add services to the container.
            builder.Services.AddDependencyInjections(builder.Configuration);
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            builder.Services.AddControllers();
            
            // API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            //// Swagger
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "Currency Converter API",
            //        Version = "v1",
            //        Description = "A robust currency conversion API backed by Frankfurter"
            //    });
            //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        Description = "JWT Authorization header. Example: 'Bearer {token}'",
            //        Name = "Authorization",
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer"
            //    });
            //    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    //{
            //    //    {
            //    //        new OpenApiSecurityScheme {  Reference = new BaseOpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            //    //        Array.Empty<string>()
            //    //    }
            //    //});
            //});

            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
            //    {
            //        Title = "Currency Converter API",
            //        Version = "v1",
            //        Description = "A robust currency conversion API backed by Frankfurter"
            //    });
            //});

            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            builder.Services.AddSwaggerGen();

            builder.Services.AddApiRateLimiting();
            builder.Services.AddHttpContextAccessor(); // needed by CorrelationIdDelegatingHandler

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Converter API v1");
                    c.RoutePrefix = string.Empty; // Swagger at root
                });
            }
            
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseApiRequestLogging();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}


/*
 
 using Asp.Versioning;
using CurrencyConverter.API.Configurations;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CurrencyConverter.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Controllers & Swagger
            builder.Services.AddControllers();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

 */