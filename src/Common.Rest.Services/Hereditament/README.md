# Hereditament Service Template
# ========================
#
# To scaffold this service, run the following commands from the solution root:
#
#   dotnet new classlib -n Hereditament.Domain -o src/Services/Hereditament/Hereditament.Domain
#   dotnet new classlib -n Hereditament.Application -o src/Services/Hereditament/Hereditament.Application
#   dotnet new classlib -n Hereditament.Infrastructure -o src/Services/Hereditament/Hereditament.Infrastructure
#   dotnet new webapi -n Hereditament.Api -o src/Services/Hereditament/Hereditament.Api
#
#   dotnet sln add src/Services/Hereditament/Hereditament.Domain/Hereditament.Domain.csproj
#   dotnet sln add src/Services/Hereditament/Hereditament.Application/Hereditament.Application.csproj
#   dotnet sln add src/Services/Hereditament/Hereditament.Infrastructure/Hereditament.Infrastructure.csproj
#   dotnet sln add src/Services/Hereditament/Hereditament.Api/Hereditament.Api.csproj
#
# Then add project references following the same pattern as SurveyData.
#
# Suggested Domain Entities:
#   - Hereditament (Id, Uprn, Line1, Line2, Town, County, PostCode, Country, Easting, Northing, etc.)
#   - HereditamentHistory (for tracking Hereditament changes over time)
#
# See SurveyData service for a complete implementation reference.
