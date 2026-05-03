# UPDATE für Martin – Codiac Heartbeat Check

**Zeit:** 2026-05-03 09:15 Uhr (Sonntag)
**Projekt:** Win Assistent (aura-clipy)
**Status:** GitHub CLI Token-Erneuerung benötigt

## Aktuelle Situation

GitHub CLI wartet auf **Browser-Authentifizierung**, um einen neuen Token mit `workflow` Scope zu erhalten.

**One-Time Code:** `9F8C-5989`
**Link:** https://github.com/login/device

## Lösungsschritte

### Option A (Empfohlen – dauert 2 Minuten)

1. **Öffne:** https://github.com/login/device im Browser
2. **Gib ein:** Den Code `9F8C-5989`
3. **Klicke:** "Continue" → "Authorize"
4. **Token-Scopes:** Stelle sicher, dass `workflow` ausgewählt ist
5. **Zurück zu Terminal:** Der Push sollte jetzt funktionieren

### Option B (Alternative – manuelle Workflow-Erstellung)

1. **Gehe zu:** https://github.com/MGAura/aura-clipy/actions
2. **Klicke:** "New workflow"
3. **Wähle:** "Set up a workflow yourself"
4. **Kopiere** den Inhalt von `build.yml`:
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

### Option C (Nur bei Windows-Build verfügbar)

1. **Auf Windows-Maschine:** `Build-Skripte\build.cmd` ausführen
2. **Manuell testen:** Ob die Anwendung funktioniert

## Was bereits erledigt ist

- ✅ GitHub Repository ist öffentlich (https://github.com/MGAura/aura-clipy)
- ✅ GitHub Actions Workflow-Datei ist vorhanden und korrekt
- ✅ Alle Commits sind lokal bereit
- ✅ Authentifizierung ist bis auf `workflow` Scope komplett

## Nächster Heartbeat-Check

Der nächste Heartbeat-Check erfolgt in 60 Minuten um **10:15 Uhr**. Bis dahin:

1. **Wenn du den Code eingibst:** Ich pushe den Workflow automatisch
2. **Wenn du manuell erstellst:** Sende mir Bescheid
3. **Wenn nichts passiert:** Ich wiederhole die Token-Anfrage

## Projekt-Zusammenfassung

**Win Assistent Status:** Phase 5 (Build-Test) blockiert durch Token-Scope
**Bereit für CI/CD:** Sobald Workflow aktiv ist, läuft automatischer Windows-Build
**Repository:** https://github.com/MGAura/aura-clipy
**Commit:** `cb95d4d` (Add GitHub Actions workflow for Windows build)

**Deine Aktion:** Code `9F8C-5989` bei https://github.com/login/device eingeben → fertig.

---

*Codiac Heartbeat läuft stabil. Win Assistent wartet auf deine Entscheidung.*