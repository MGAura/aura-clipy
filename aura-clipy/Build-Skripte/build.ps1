# PC-Begleiter Build Script (PowerShell)
# Erstellt: 2026-04-17

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$Clean,
    [switch]$Publish,
    [switch]$Help
)

# Konfiguration
$ProjectName = "WinAssistent"
$ProjectFile = Join-Path $PSScriptRoot "..\AuraClipy.csproj"
$PublishDir = Join-Path $PSScriptRoot "..\publish"

function Write-Step($message) {
    Write-Host "[STEP] $message" -ForegroundColor Cyan
}

function Write-Success($message) {
    Write-Host "[SUCCESS] $message" -ForegroundColor Green
}

function Write-Error($message) {
    Write-Host "[ERROR] $message" -ForegroundColor Red
}

# Help
if ($Help) {
    Write-Host @"
PC-Begleiter Build Script
========================

Verwendung:
    .\build.ps1 [-Configuration Release|Debug] [-Runtime win-x64] [-Clean] [-Publish] [-Help]

Parameter:
    -Configuration  Build-Konfiguration (Standard: Release)
    -Runtime        Ziel-Runtime (Standard: win-x64)
    -Clean           Bereinigt vorherige Builds
    -Publish         Erstellt selbst-contained Executable
    -Help            Diese Hilfe anzeigen

Beispiele:
    .\build.ps1                      # Einfacher Release Build
    .\build.ps1 -Clean -Publish       # Vollständiger Rebuild mit Publish
    .\build.ps1 -Configuration Debug   # Debug Build

"@
    exit 0
}

Write-Host "PC-Begleiter Build Script" -ForegroundColor Magenta
Write-Host "==========================" -ForegroundColor Magenta
Write-Host ""

# Prüfe .NET SDK
Write-Step "Prüfe .NET SDK..."
try {
    $dotnetVersion = dotnet --version 2>$null
    Write-Host "  .NET SDK Version: $dotnetVersion" -ForegroundColor Gray
} catch {
    Write-Error ".NET SDK nicht gefunden. Bitte installiere .NET SDK 10.0 oder höher."
    Write-Host "  Download: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Prüfe Projektdatei
if (-not (Test-Path $ProjectFile)) {
    Write-Error "Projektdatei nicht gefunden: $ProjectFile"
    Write-Host "  Bitte stelle sicher, dass AuraClipy.csproj im Hauptverzeichnis liegt." -ForegroundColor Yellow
    exit 1
}

Write-Host "  Projekt: $ProjectFile" -ForegroundColor Gray

# Clean
if ($Clean) {
    Write-Step "Bereinige vorherige Builds..."
    dotnet clean $ProjectFile --configuration $Configuration --verbosity quiet 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Warnung: Clean fehlgeschlagen, wird fortgesetzt..." -ForegroundColor Yellow
    } else {
        Write-Host "  Bereinigung abgeschlossen" -ForegroundColor Gray
    }
}

# Restore
Write-Step "Stelle NuGet-Pakete wieder her..."
dotnet restore $ProjectFile --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Error "Package Restore fehlgeschlagen."
    exit 1
}
Write-Host "  Restore abgeschlossen" -ForegroundColor Gray

# Build
Write-Step "Baue Projekt ($Configuration)..."
dotnet build $ProjectFile --configuration $Configuration --no-restore --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build fehlgeschlagen."
    exit 1
}
Write-Success "Build erfolgreich!"

# Publish
if ($Publish) {
    Write-Step "Erstelle selbst-contained Executable..."
    
    # Erstelle Publish-Verzeichnis
    $targetDir = Join-Path $PublishDir $Runtime
    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }
    
    dotnet publish $ProjectFile `
        --configuration $Configuration `
        --runtime $Runtime `
        --self-contained true `
        --output $targetDir `
        --verbosity quiet
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Publish fehlgeschlagen."
        exit 1
    }
    
    # Prüfe Ausgabe
    $exePath = Join-Path $targetDir "AuraClipy.exe"
    if (Test-Path $exePath) {
        $exeInfo = Get-Item $exePath
        Write-Success "Executable erstellt!"
        Write-Host ""
        Write-Host "  Speicherort: $targetDir" -ForegroundColor Cyan
        Write-Host "  Dateigröße:  $([math]::Round($exeInfo.Length / 1MB, 2)) MB" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "  Ausführen mit:" -ForegroundColor Yellow
        Write-Host "    .\publish\$Runtime\AuraClipy.exe" -ForegroundColor White
    } else {
        Write-Error "Executable nicht gefunden: $exePath"
        exit 1
    }
}

Write-Host ""
Write-Success "Build-Prozess abgeschlossen!"
