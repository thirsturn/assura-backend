FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /src

COPY ["backend.API/backend.API.csproj", "backend.API/"]
COPY ["backend.Core/backend.Core.csproj", "backend/Core/"]
COPY ["backend.Infrastructure/backend.Infrastructure.csproj", "backend.Infrastructure/"]

RUN dotnet restore "backend.API/backend.API.csproj"

COPY . .
WORKDIR "/src/backend.API"
RUN dotnet publish "backend.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "backend.API.dll"]