# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props .
COPY cart/ ./cart/
WORKDIR /src/cart
RUN dotnet restore Cart.sln
RUN dotnet publish API/Cart.API.csproj -c Release -o /app/publish

# Use the official ASP.NET runtime image for run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Cart.API.dll"]