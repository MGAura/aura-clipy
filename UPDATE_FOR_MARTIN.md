## 📋 Kurzanleitung für Martin

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
          echo "Main EXE: aura-clipy/bin/Release/net10.0-windows/AuraClipy.exe"
        }
        
        if (Test-Path "aura-clipy/publish/win-x64/AuraClipy.exe") {
          echo "Published EXE: aura-clipy/publish/win-x64/AuraClipy.exe"
        }
    
    - name: Upload build artifacts
      uses: actions/upload-artifact@v4
      with:
        name: windows-build-artifacts
        path: |
          aura-clipy/bin/Release/net10.0-windows/
          aura-clipy/publish/win-x64/
        if-no-files-found: warn
```

**Status:** Alle Device Flow Codes abgelaufen → manuelle GitHub UI Lösung erforderlich
**Aktuelle Zeit:** 2026-05-03 18:45 (Herzschlag-Prüfung)
**Nächster Schritt:** GitHub UI Workflow erstellen (wie oben)

**Warum GitHub UI?**
- Workflow liegt in `aura-clipy/.github/workflows/build.yml` statt Root `.github/workflows/`
- GitHub Actions sucht nur im Root-Verzeichnis
- GitHub CLI Device Flow funktioniert nicht (alle Codes abgelaufen)
- Einfache Lösung: GitHub UI manuelle Erstellung
