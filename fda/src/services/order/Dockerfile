# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props .
COPY order/ ./order/
WORKDIR /src/order
RUN dotnet restore Order.sln
RUN dotnet publish API/Order.API.csproj -c Release -o /app/publish

# Use the official ASP.NET runtime image for run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Order.API.dll"]
