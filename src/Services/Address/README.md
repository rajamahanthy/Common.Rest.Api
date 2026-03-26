# Address Service Template
# ========================
#
# To scaffold this service, run the following commands from the solution root:
#
#   dotnet new classlib -n Address.Domain -o src/Services/Address/Address.Domain
#   dotnet new classlib -n Address.Application -o src/Services/Address/Address.Application
#   dotnet new classlib -n Address.Infrastructure -o src/Services/Address/Address.Infrastructure
#   dotnet new webapi -n Address.Api -o src/Services/Address/Address.Api
#
#   dotnet sln add src/Services/Address/Address.Domain/Address.Domain.csproj
#   dotnet sln add src/Services/Address/Address.Application/Address.Application.csproj
#   dotnet sln add src/Services/Address/Address.Infrastructure/Address.Infrastructure.csproj
#   dotnet sln add src/Services/Address/Address.Api/Address.Api.csproj
#
# Then add project references following the same pattern as SurveyData.
#
# Suggested Domain Entities:
#   - Address (Id, Uprn, Line1, Line2, Town, County, PostCode, Country, Easting, Northing, etc.)
#   - AddressHistory (for tracking address changes over time)
#
# See SurveyData service for a complete implementation reference.
