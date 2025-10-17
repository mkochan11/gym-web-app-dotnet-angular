@echo off
echo Gym Web App Log Viewer
echo.

echo Choose an option:
echo 1. View logs from Docker container (if running with Docker)
echo 2. View local log files (if running locally)
echo 3. Follow live logs from Docker container
echo.
set /p choice="Enter your choice (1, 2, or 3): "

if "%choice%"=="1" (
    echo.
    echo Copying logs from Docker container...
    docker cp docker-api-1:/app/logs ./docker-logs
    echo Logs copied to ./docker-logs folder
    echo Opening logs folder...
    explorer ./docker-logs
) else if "%choice%"=="2" (
    echo.
    echo Opening local logs folder...
    explorer ../src/GymWebApp/GymWebApp.WebAPI/logs
) else if "%choice%"=="3" (
    echo.
    echo Following live logs from Docker container...
    echo Press Ctrl+C to stop following logs
    docker logs -f docker-api-1
) else (
    echo Invalid choice. Opening local logs folder...
    explorer ../src/GymWebApp/GymWebApp.WebAPI/logs
)

echo.
pause
