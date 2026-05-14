# GitHub Actions Workflow Setup für Win Assistant

## Problem
GitHub blockiert das automatische Pushen der Workflow-Datei wegen fehlendem OAuth `workflow` Scope im Token.

## Lösung (manuell über GitHub UI)

### Schritt 1: GitHub Actions öffnen
Gehe zu: **https://github.com/MGAura/aura-clipy/actions**

### Schritt 2: Neuen Workflow erstellen
Klicke auf den grünen Button **"New workflow"**

### Schritt 3: Eigenen Workflow einrichten
Wähle **"Set up a workflow yourself"**

### Schritt 4: Workflow-Code einfügen
Kopiere den folgenden Code und füge ihn in das Editor-Feld ein:

```yaml
name: Windows Build Test

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]
  workflow_dispatch:

jobs:
  build-windows:
    runs-on: windows-latest
    
    steps:
    - name: Checkout repository
      uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '10.0.x'
    
    - name: Install Windows Desktop workload
      run: dotnet workload install windowsdesktop
    
    - name: Restore dependencies
      run: |
        cd aura-clipy
        dotnet restore AuraClipy.csproj
    
    - name: Build Release
      run: |
        cd aura-clipy
        dotnet build AuraClipy.csproj --configuration Release --no-restore
    
    - name: Publish Windows executable (self-contained)
      run: |
        cd aura-clipy
        dotnet publish AuraClipy.csproj --configuration Release --runtime win-x64 --self-contained true --output ./publish/win-x64
    
    - name: List output files
      run: |
        echo "=== Build Output ==="
        if (Test-Path "aura-clipy/bin/Release/net10.0-windows/AuraClipy.exe") {
          echo "✅ Main EXE: aura-clipy/bin/Release/net10.0-windows/AuraClipy.exe"
          Get-Item "aura-clipy/bin/Release/net10.0-windows/AuraClipy.exe" | Format-List *
        } else {
          echo "❌ Main EXE not found in bin/"
        }
        
        if (Test-Path "aura-clipy/publish/win-x64/AuraClipy.exe") {
          echo "✅ Published EXE: aura-clipy/publish/win-x64/AuraClipy.exe"
          Get-Item "aura-clipy/publish/win-x64/AuraClipy.exe" | Format-List *
        } else {
          echo "❌ Published EXE not found"
        }
    
    - name: Upload build artifacts
      uses: actions/upload-artifact@v4
      with:
        name: windows-build-artifacts
        path: |
          aura-clipy/bin/Release/net10.0-windows/
          aura-clipy/publish/win-x64/
        if-no-files-found: warn
    
    - name: Run simple tests (if executable exists)
      run: |
        if (Test-Path "aura-clipy/bin/Release/net10.0-windows/AuraClipy.exe") {
          echo "✅ Main EXE exists, would run tests here"
          # Add actual test execution when implemented
        } else {
          echo "⚠️ No executable found for tests"
        }
```

### Schritt 5: Datei benennen
Ändere den Dateinamen oben links zu **`build.yml`**

### Schritt 6: Commit & Push
Klicke auf **"Start commit"** → **"Commit directly to the main branch"** → **"Commit changes"**

## Was danach passiert
1. GitHub erstellt die Datei `.github/workflows/build.yml`
2. Der Workflow startet automatisch
3. Windows-Build wird auf GitHub Actions Runnern ausgeführt (~5-10 Minuten)
4. EXE-Dateien werden als Artefakte verfügbar
5. Build-Erfolg/Misserfolg ist in GitHub Actions sichtbar

## Alternativer direkter Link
Du kannst auch direkt hier klicken: **https://github.com/MGAura/aura-clipy/new/main/.github/workflows/build.yml**

---

## Projektstatus
- ✅ Repository: öffentlich & synchronisiert
- ✅ Code: komplett implementiert (Phasen 1-4)
- ✅ Workflow-Datei: lokal vorhanden
- ❌ Workflow auf GitHub: fehlt (wegen OAuth Scope)
- 🔄 Lösung: GitHub UI manuelle Erstellung (wie oben)

**Zeitaufwand:** ~2 Minuten  
**Nachher:** Automatische CI/CD für alle zukünftigen Commits