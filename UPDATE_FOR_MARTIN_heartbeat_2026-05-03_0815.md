# HEARTBEAT CHECK – Win Assistent (PC-Begleiter)
**Datum:** 2026-05-03 08:15 (Europe/Berlin)
**Status:** 🔄 Arbeit fortgesetzt – Blockierung durch OAuth Token bleibt

## Aktueller Status

GitHub Actions Workflow für den **Windows-Build** ist vollständig implementiert und im lokalen Repository committet. Allerdings verhindern **OAuth Token-Beschränkungen** (`workflow` Scope fehlt) den automatischen Push und die Aktivierung des Workflows.

**Blockierendes Problem:**
- Workflow-Datei `.github/workflows/build.yml` liegt lokal bereit
- GitHub CLI Token hat nicht den erforderlichen `workflow` Scope
- Push schlägt fehl, Workflow kann nicht automatisch aktiviert werden

## Lösungsschritte für Dich

### Option 1: GitHub UI (Empfohlen)
1. Öffne https://github.com/MGAura/aura-clipy/actions
2. Klicke auf **"New workflow"**
3. Klicke **"set up a workflow yourself"**
4. Kopiere den gesamten Inhalt aus der lokalen Datei `.github/workflows/build.yml`
5. Füge ihn ein, speichere und aktiviere den Workflow

### Option 2: GitHub CLI Token aktualisieren
1. Gehe zu https://github.com/settings/tokens
2. Erstelle einen neuen **Personal Access Token** mit `workflow` Scope
3. Führe aus: `gh auth login --web` und verwende den neuen Token

### Option 3: Lokalen Windows-Build testen
1. Auf deinem Windows-PC: `Build-Skripte\build.cmd` ausführen
2. Die generierte EXE im `Build-Skripte\bin\Release\net8.0-windows` Verzeichnis testen

## Was bisher erreicht wurde

✅ **Alle Phasen 1-4 komplett implementiert:**
- Safety-Layer mit 20+ Sicherheitsregeln
- Screen-Capture, Vision-Integration, Kontext-Erkennung
- OpenClaw Handoff, JSON-Konfiguration, Logging
- Build-Skripte für Windows/Linux
- GitHub Repository öffentlich & synchronisiert
- Vollständige Dokumentation (README, CHANGELOG, STATUS, TASKS)

✅ **Repository:**
- https://github.com/MGAura/aura-clipy (öffentlich)
- Alle Commits gepusht
- Workflow-Datei vorbereitet

🔄 **Blockiert in Phase 5:**  
GitHub Actions Workflow muss manuell über UI aktiviert werden

## Nächste Schritte nach Workflow-Aktivierung

1. **CI/CD Pipeline starten** → Windows Build automatisiert
2. **EXE als Artefakt** → Download von GitHub Actions
3. **Windows-Test** → Läuft die Anwendung?
4. **UI-Verbesserungen** → Animationen, Sounds
5. **Installer-Paket** → Setup erstellen

## Workflow-Inhalt (bereit zum Kopieren)

```yaml
name: Build Windows EXE

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]
  workflow_dispatch:

jobs:
  build:
    runs-on: windows-latest

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
          include-prerelease: false

      - name: Install Windows Desktop Workload
        run: dotnet workload install windowsdesktop

      - name: Restore dependencies
        run: dotnet restore

      - name: Build Release
        run: dotnet build -c Release --no-restore

      - name: Publish Self-contained EXE
        run: |
          dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true

      - name: Upload Build Artifacts
        uses: actions/upload-artifact@v4
        with:
          name: WinAssistent
          path: |
            bin/Release/net8.0-windows/win-x64/publish/
            Build-Skripte/bin/Release/net8.0-windows/
          if-no-files-found: error
```

## Handlungsempfehlung

**Für heute:** Option 1 (GitHub UI) ist der schnellste Weg, um den Workflow zu aktivieren.  
**Zeitaufwand:** Ca. 2-3 Minuten.

Sobald der Workflow aktiviert ist, startet der automatische Build bei jedem Push zu main/master. Du bekommst dann eine fertige Windows-EXE als Download.

---

**Wenn du Fragen hast oder Hilfe bei einem der Schritte brauchst, sag Bescheid!**

*- Aura*