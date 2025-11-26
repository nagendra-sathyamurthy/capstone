# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props .
COPY crm/ ./crm/
WORKDIR /src/crm
RUN dotnet restore crm.sln
RUN dotnet publish API/Crm.API.csproj -c Release -o /app/publish

# Use the official ASP.NET runtime image for run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose default port
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Crm.API.dll"]
