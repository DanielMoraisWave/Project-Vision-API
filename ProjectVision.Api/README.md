# Project Vision API

Project Vision API is an ASP.NET Core REST API to manage products and
their available stock.

## Technologies

-.NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server Express LocalDB
- xUnit
- FluentAssertions
- OpenAPI

## Project structure

- `ProjectVision.Api` > ASP.NET Core API
- `ProjectVision.Tests` > unit and controller tests

## Requirements

To run the project locally:

- Visual Studio 2026
- .NET 10 SDK
- SQL Server Express LocalDB

The following Visual Studio workloads are recommended:

- ASP.NET and web development
- Data storage and processing

## Database configuration

The application uses SQL Server Express LocalDB.

Default connection string:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ProjectVisionDb;Trusted_Connection=True;TrustServerCertificate=True"

Create Database:
Package Manager Console > Update-Database