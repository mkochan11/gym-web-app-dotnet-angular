# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GymWebApp.WebAPI/GymWebApp.WebAPI.csproj", "GymWebApp.WebAPI/"]
COPY ["../GymWebApp.ApplicationCore/GymWebApp.ApplicationCore.csproj", "GymWebApp.ApplicationCore/"]
COPY ["../GymWebApp.Data/GymWebApp.Data.csproj", "GymWebApp.Data/"]
RUN dotnet restore "GymWebApp.WebAPI/GymWebApp.WebAPI.csproj"

COPY . .
WORKDIR "/src/GymWebApp.WebAPI"
RUN dotnet build "GymWebApp.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "GymWebApp.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GymWebApp.WebAPI.dll"]
