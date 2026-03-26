# Market Data Service Template (Feature-Flagged)
# ================================================
#
# This service is OPTIONAL and should be behind a feature flag.
#
# To scaffold this service, run the following commands from the solution root:
#
#   dotnet new classlib -n MarketData.Domain -o src/Services/MarketData/MarketData.Domain
#   dotnet new classlib -n MarketData.Application -o src/Services/MarketData/MarketData.Application
#   dotnet new classlib -n MarketData.Infrastructure -o src/Services/MarketData/MarketData.Infrastructure
#   dotnet new webapi -n MarketData.Api -o src/Services/MarketData/MarketData.Api
#
# Also add the following package for feature flags:
#   dotnet add MarketData.Api package Microsoft.FeatureManagement.AspNetCore
#
# Example feature flag configuration in appsettings.json:
#   "FeatureManagement": {
#     "MarketDataService": true
#   }
#
# Suggested Domain Entities:
#   - MarketTransaction (SalePrice, SaleDate, PropertyType, etc.)
#   - MarketComparable (ComparisonData, AdjustmentFactor, etc.)
#
# See SurveyData service for a complete implementation reference.
