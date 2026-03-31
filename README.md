# Currency Converter API Test  
Currency Converter API Assignment

---

## Description  
This project is a **.NET 10 Web API** developed using **ASP.NET Core**, focusing on building a scalable and maintainable currency conversion service.

The implementation follows a **layered architecture** with separation of concerns across API, Business Logic, Domain, and Infrastructure layers. The goal was to cover all required aspects, such as resilience, security, and extensibility, while keeping the implementation practical within the given timeframe.

---

## Prerequisites For Local Testing and Code Review
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2026](https://visualstudio.microsoft.com/vs/)

## Features Implemented  

### Currency Endpoints
- Retrieve the latest exchange rates based on the base currency  
- Convert amount between currencies  
- Retrieve historical exchange rates with pagination  

### Business Rules
- Restricted currencies:
  - TRY, PLN, THB, MXN
- Returns BadRequest when restricted currencies are used  

### Architecture & Design
- Layered architecture (API, Business, Domain, Infrastructure)  
- Dependency Injection across services  
- Factory pattern for currency provider selection  

### Resilience
- HTTP client resilience using retry policies (Polly)  
- Correlation ID propagation for tracing external API calls  

### Security
- JWT Authentication  
- Role-based authorization  

### Additional
- API Versioning (v1, v2)  
- Rate limiting  
- Global exception handling middleware  
- Structured logging using Serilog  

---

## External API  

- Frankfurter API  
  - https://frankfurter.dev/  

---

## Additional Notes  

- To keep the implementation within a reasonable timeframe, focus was placed on covering all required areas rather than implementing every feature to full production depth  

- Some areas are implemented in a minimal but extensible way, such as:
  - Caching (not fully implemented and only InMemoryCache used.)
  - Configuration settings within the Program.cs file (e.g., Logger settings should be inside appSettings)
  - Unit and Integration tests do not cover the project fully.

---

## Possible Improvements  

- Add distributed caching (Redis)  
- Add CI/CD pipeline
