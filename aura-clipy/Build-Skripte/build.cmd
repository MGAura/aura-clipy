@echo off
echo PC-Begleiter Build Script
echo ===========================

REM Set error handling
setlocal enabledelayedexpansion
set "ERRORLEVEL=0"

REM Check for .NET SDK
echo Checking for .NET SDK...
where dotnet >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: .NET SDK not found. Please install .NET SDK 10.0 or later.
    echo Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Set project directory
set "PROJECT_DIR=%~dp0.."
set "SOLUTION_FILE=%PROJECT_DIR%\AuraClipy.sln"
set "PROJECT_FILE=%PROJECT_DIR%\AuraClipy.csproj"

REM Check if solution file exists, if not use project file
if exist "%SOLUTION_FILE%" (
    set "TARGET=%SOLUTION_FILE%"
    echo Building solution: %SOLUTION_FILE%
) else if exist "%PROJECT_FILE%" (
    set "TARGET=%PROJECT_FILE%"
    echo Building project: %PROJECT_FILE%
) else (
    echo ERROR: No solution (.sln) or project (.csproj) file found.
    echo Please check if the project is properly configured.
    pause
    exit /b 1
)

REM Clean previous builds
echo Cleaning previous builds...
dotnet clean "%TARGET%" --configuration Release
if %ERRORLEVEL% neq 0 (
    echo WARNING: Clean failed, continuing anyway...
)

REM Restore packages
echo Restoring packages...
dotnet restore "%TARGET%"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Package restore failed.
    pause
    exit /b 1
)

REM Build Release
echo Building Release configuration...
dotnet build "%TARGET%" --configuration Release --no-restore
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed.
    pause
    exit /b 1
)

REM Publish as self-contained Windows executable
echo Publishing as self-contained Windows executable...
dotnet publish "%PROJECT_FILE%" ^
    --configuration Release ^
    --runtime win-x64 ^
    --self-contained true ^
    --output "%PROJECT_DIR%\publish\win-x64"

if %ERRORLEVEL% neq 0 (
    echo ERROR: Publish failed.
    pause
    exit /b 1
)

REM Check output
if exist "%PROJECT_DIR%\publish\win-x64\AuraClipy.exe" (
    echo.
    echo SUCCESS: Build completed!
    echo.
    echo Output location: %PROJECT_DIR%\publish\win-x64\
    echo Executable: AuraClipy.exe
    echo.
    echo You can run: "%PROJECT_DIR%\publish\win-x64\AuraClipy.exe"
) else (
    echo ERROR: Executable not found at expected location.
    pause
    exit /b 1
)

pause