# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Services/crm.csproj", "Services/"]
COPY . .
RUN dotnet restore "Services/crm.csproj"
RUN dotnet publish "Services/crm.csproj" -c Release -o /app/publish

# Use the official ASP.NET runtime image for run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose default port
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "crm.dll"]
