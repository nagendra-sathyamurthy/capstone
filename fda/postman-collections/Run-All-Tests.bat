@echo off
REM Run-All-Tests.bat
REM Windows batch file wrapper for Run-All-Tests.ps1
REM Makes it easy to run tests without opening PowerShell

echo ========================================
echo   Food Delivery Application
echo   Comprehensive Test Suite
echo ========================================
echo.

REM Check if PowerShell is available
where powershell >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo ERROR: PowerShell is not installed or not in PATH
    pause
    exit /b 1
)

REM Get the directory where this batch file is located
set SCRIPT_DIR=%~dp0

REM Change to script directory
cd /d "%SCRIPT_DIR%"

REM Run PowerShell script
echo Running tests...
echo.
powershell -ExecutionPolicy Bypass -File "%SCRIPT_DIR%Run-All-Tests.ps1" %*

REM Check exit code
if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================
    echo   All tests passed successfully!
    echo ========================================
) else (
    echo.
    echo ========================================
    echo   Some tests failed. Check reports.
    echo ========================================
)

echo.
echo Press any key to exit...
pause >nul
