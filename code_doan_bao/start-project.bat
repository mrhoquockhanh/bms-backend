@echo off
chcp 65001 >nul
title BMS Project - Start System

echo ==================================================
echo   1. Starting Docker Containers
echo ==================================================
cd /d "%~dp0be"
docker compose up -d --build

echo.
echo ==================================================
echo   2. Starting All .NET Backend Services (1 CMD Window)
echo ==================================================
start "BMS - Backend .NET Services" cmd /k "cd /d "%~dp0be\Payment-service" && start dotnet run && cd /d "%~dp0be\warehouse-service" && start dotnet run && cd /d "%~dp0be\api-gateway" && dotnet run"

echo.
echo ==================================================
echo   3. Starting Frontend
echo ==================================================
start "BMS - Frontend (fe)" cmd /k "cd /d "%~dp0fe" && npm start"

echo.
timeout /t 5 >nul
start http://localhost:3000