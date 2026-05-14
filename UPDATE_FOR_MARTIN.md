# Update für Martin – Codiac Heartbeat Check

## Status (14. Mai 2026, 02:43 Uhr)

**⚠️ KRITISCHES PROBLEM:** GitHub blockiert Workflow-Creation seit **10 Tagen** wegen fehlendem OAuth `workflow` Scope.

## Was ist passiert?

1. **GitHub API/PUSH blockiert** Workflow-Creation/Update
2. **Token-Scope Problem:** Dein GitHub Token hat nur `gist`, `read:org`, `repo` Scopes, aber nicht `workflow`
3. **Workflow-Datei existiert lokal** (`.github/workflows/build.yml`) im Repository
4. **Push wird abgelehnt** mit Fehler: "refusing to allow an OAuth App to create or update workflow `.github/workflows/build.yml` without `workflow` scope"
5. **GitHub Actions zeigt 0 Workflows** (weil Workflow noch nicht auf GitHub ist)

## Was muss getan werden?

### 🎯 **EINZIGE LÖSUNG:** GitHub UI manuell nutzen

**Schritt-für-Schritt:**

1. **Gehe zu:** https://github.com/MGAura/aura-clipy/actions
2. **Klicke auf "New workflow"**
3. **Oder direkter Link:** https://github.com/MGAura/aura-clipy/new/main/.github/workflows/build.yml
4. **Füge den Workflow-Code ein** (siehe unten)
5. **Speicere** als `.github/workflows/build.yml`
6. **Klicke "Start commit"** → "Commit directly to the main branch"
7. **Workflow startet automatisch** und Windows Build wird ausgeführt

## Workflow-Code (kopiere in GitHub UI)

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

## Alternative Option (falls GitHub CLI neu auth)

Wenn du GitHub CLI neu authentifizieren willst:
```bash
gh auth login --scopes workflow,repo
```
Aber **Achtung:** Das erfordert erneute Authentifizierung und ist komplexer als die UI-Lösung.

## Projektstand

- ✅ **Phasen 1-4 komplett implementiert**
- ✅ **Repository ist öffentlich und synchronisiert**
- ✅ **21 lokale Commits sind auf GitHub**
- ✅ **Workflow-Datei existiert lokal korrekt**
- ❌ **Workflow kann nicht automatisch gepusht werden** (OAuth Restriction)
- ⚠️ **Projekt seit 10 Tagen blockiert**

## Nächste Schritte nach Workflow-Creation

1. **GitHub Actions Workflow** wird automatisch ausgelöst
2. **Windows Build** wird auf GitHub Runnern ausgeführt (~5-10 Minuten)
3. **EXE-Dateien** werden als Artefakte verfügbar sein
4. **Build-Erfolg/Misserfolg** wird in GitHub Actions sichtbar

---

**Zusammenfassung:** Die einzige verbleibende Hürde ist die manuelle Erstellung des GitHub Actions Workflows über die Web UI. Sobald das erledigt ist, wird der CI/CD-Prozess automatisch laufen und die Windows-EXE produziert werden.

**Frage:** Brauchst du Hilfe dabei, oder soll ich den Workflow-Code noch einmal anders aufbereiten?