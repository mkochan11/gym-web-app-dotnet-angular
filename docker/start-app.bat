@echo off
echo Starting Gym Web App...
echo.

echo Building and starting containers...
docker-compose up --build

echo.
echo Application should be available at:
echo - API: http://localhost:5000
echo - Swagger UI: http://localhost:5000/swagger
echo - Database: localhost:1433
echo.
pause
