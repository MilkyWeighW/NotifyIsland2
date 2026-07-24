@echo off
setlocal enabledelayedexpansion
title NotifyIsland2 CIPX Build

set "CURRENT_DIR=%~dp0"
set "PROJECT=%CURRENT_DIR%cn.lixiaotuan.notifyisland2_BatchRan.csproj"
set "PWSH=C:\Program Files\PowerShell\7\pwsh.exe"

if not exist "%PWSH%" (
    echo [ERROR] pwsh.exe not found at %PWSH%
    pause
    exit /b 1
)

echo ============================================
echo   NotifyIsland2 CIPX Build
echo ============================================
echo.

echo [1/2] Cleaning old bin/obj...
if exist "%CURRENT_DIR%bin" rd /s /q "%CURRENT_DIR%bin"
if exist "%CURRENT_DIR%obj" rd /s /q "%CURRENT_DIR%obj"
if exist "%CURRENT_DIR%cipx" rd /s /q "%CURRENT_DIR%cipx"

echo [2/2] Building and packaging...
set "PATH=%PWSH%\..;%PATH%"
dotnet publish "%PROJECT%" -p:CreateCipx=true -c Release
if %ERRORLEVEL% neq 0 (
    echo.
    echo [FAIL] Build failed. Check errors above.
    pause
    exit /b 1
)

if exist "%CURRENT_DIR%cipx\cn.lixiaotuan.notifyisland2_BatchRan.cipx" (
    echo.
    echo ============================================
    echo   Build SUCCESS
    echo   CIPX: cipx\cn.lixiaotuan.notifyisland2_BatchRan.cipx
    echo   Path: %CURRENT_DIR%cipx\
    echo ============================================
) else (
    echo [WARN] CIPX file not found
)

echo.
pause
