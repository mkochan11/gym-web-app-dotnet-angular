# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["GymWebApp.WebAPI/GymWebApp.WebAPI.csproj", "GymWebApp.WebAPI/"]
COPY ["GymWebApp.ApplicationCore/GymWebApp.ApplicationCore.csproj", "GymWebApp.ApplicationCore/"]
COPY ["GymWebApp.Data/GymWebApp.Data.csproj", "GymWebApp.Data/"]

# Restore dependencies
RUN dotnet restore "GymWebApp.WebAPI/GymWebApp.WebAPI.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/GymWebApp.WebAPI"
RUN dotnet build "GymWebApp.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "GymWebApp.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install SQL Server tools for health checks
RUN apt-get update && apt-get install -y curl gnupg && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Create logs directory
RUN mkdir -p /app/logs

COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

ENTRYPOINT ["dotnet", "GymWebApp.WebAPI.dll"]
