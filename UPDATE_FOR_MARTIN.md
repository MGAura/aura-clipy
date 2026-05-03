## 📋 Kurzanleitung für Martin - AKTUELLER STAND

**Problem:** GitHub blockiert automatische Workflow-Erstellung wegen fehlendem OAuth `workflow` Scope.
- Workflow-Datei existiert lokal korrekt in `.github/workflows/build.yml`
- Git Push wird abgelehnt mit Fehler: "refusing to allow an OAuth App to create or update workflow `.github/workflows/build.yml` without `workflow` scope"
- **Lösung:** Nur manuelle Erstellung über GitHub UI möglich

**Was zu tun ist:**
1. **GitHub Actions öffnen:** https://github.com/MGAura/aura-clipy/actions
2. **"New workflow" klicken** (grüner Button)
3. **"Set up a workflow yourself" wählen**
4. **YAML-Code kopieren** (unten bereit)
5. **Name:** `build.yml` eintragen
6. **Commit to main branch** → Workflow startet automatisch

**Workflow-Code zum Kopieren:**
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

**Status:** 
- ✅ Repository ist öffentlich und synchronisiert
- ✅ Workflow-Datei existiert lokal korrekt
- ❌ GitHub API/PUSH blockiert Workflow-Creation/Update (fehlender `workflow` Scope)
- 🔄 **Manuelle GitHub UI-Erstellung erforderlich**

**Warum nur GitHub UI?**
- GitHub verweigert OAuth Apps ohne `workflow` Scope, Workflow-Dateien zu erstellen/aktualisieren
- Alle Device Flow Codes sind abgelaufen
- Workflow-Datei kann nicht automatisch auf GitHub gepusht werden
- Einfache Lösung: GitHub UI manuelle Erstellung

**Alternative (falls du GitHub CLI neu konfigurieren willst):**
```bash
gh auth login --scopes workflow,repo
```
Dann könnten wir es automatisch versuchen.

**Aktuelle Zeit:** 2026-05-03 19:18 (Herzschlag-Prüfung)
**Nächster Schritt:** GitHub UI Workflow erstellen (wie oben)