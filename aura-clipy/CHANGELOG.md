# CHANGELOG - Win Assistent (PC-Begleiter)

Alle Änderungen werden hier dokumentiert. Format: [Datum] - [Änderung]

## Version 0.3.0 - Build & Deployment (2026-04-17)


### Build-Skripte erstellt

#### build.cmd
- Windows Batch-Skript
- Einfacher Build ohne Parameter
- Prft .NET SDK
- Erstellt Release Build
- Self-contained Publish

#### build.ps1
- PowerShell Build-Skript
- Erweiterte Optionen:
  - `-Configuration` (Release/Debug)
  - `-Runtime` (win-x64)
  - `-Clean` (Bereinigung)
  - `-Publish` (Executable erstellen)
- Detaillierte Ausgabe
- Hilfe-Funktion (`-Help`)

#### test.ps1
- PowerShell Test-Skript
- Validiert Kern-Funktionalität:
  - Rule Engine Basis-Funktionen
  - Konfigurations-System
  - Kontext-Erkennung
- Detaillierte Testergebnisse
- Zusammenfassung

### Ordner-Struktur reorganisiert

```
Win Assistent/
├── *.cs                    # Alle Quelldateien im Hauptverzeichnis
├── AuraClipy.csproj        # .NET Projekt
├── Dokumentation/
│   ├── produktkonzept-pc-begleiter.md
│   └── CHANGELOG.md
├── Build-Skripte/
│   ├── build.cmd
│   ├── build.ps1
│   ├── test.ps1
│   └── README.md
└── publish/                 # Build-Ausgabe
```


### Projekt bereit für Build-Test

- Alle Kern-Komponenten implementiert
- Build-Skripte erstellt
- Test-Suite vorhanden
- Dokumentation vollständig
- Bereit für ersten Windows-Build

### Neue Komponenten

#### OpenClawHandoffService.cs
- **Intelligente Delegation an OpenClaw**
  - Automatischer Handoff bei unklaren Risiken
  - Handoff bei sensiblen Kategorien (Social Engineering, Financial Fraud)
  - Konfigurierbare Schwellwerte
  - Fallback auf Rule-Engine wenn OpenClaw nicht erreichbar

- **Strukturierte Anfragen**
  - Sicherheitsanalyse in strukturiertes Format
  - Screenshot-Übergabe optional
  - Erklärung der erkannten Regeln

- **Benutzer-Report**
  - Lesbare Sicherheitsberichte
  - Risk-Assessment
  - Empfehlungen

#### LocalConfiguration.cs
- **JSON-basierte Konfiguration**
  - `%APPDATA%/WinAssistent/config.json`
  - Versionierung
  - Automatische Migration

- **Kategorie-Einstellungen**
  - General: Auto-Start, Sprache, Theme
  - SafetyLayer: Risk-Threshold, Overlay
  - Analysis: Auto-Analyse, Vision-Settings
  - OpenClaw: URL, Handoff-Threshold
  - Rules: Custom Risk Scores, deaktivierte Regeln
  - Logging: Log-Level, Rotation
  - Privacy: Data-Storage, Cloud-Settings
  - UI: Position, Opacity, Compact-Mode

- **Rule-Engine Konfiguration**
  - Direkte Anpassung der Rule-Engine
  - Speichern/Laden von Custom Risk Scores

#### SecurityLogger.cs
- **Lokales Logging**
  - `%LOCALAPPDATA%/WinAssistent/Logs/`
  - Tägliche Rotierung
  - Größen-basierte Rotation
  - Aufräumen alter Logs

- **Log-Einträge**
  - Analysen
  - Warnungen
  - Benutzeraktionen
  - OpenClaw Handoffs
  - System-Events

- **Statistiken**
  - Risk-Scores über Zeit
  - Erkannte Kategorien
  - Durchschnittswerte
  - Total-Analysen

#### WinAssistantCoordinator.cs
- **Zentraler Koordinator**
  - Verbindet alle Services
  - Event-basierte Architektur
  - Automatische Initialisierung
  - Graceful Shutdown

- **Events**
  - OnSecurityWarning
  - OnAnalysisComplete
  - OnLogMessage

- **Haupt-Workflow**
  ```
  1. Kontext erkennen (ContextDetector)
  2. Rule-basierte Analyse (SimpleRuleEngine)
  3. Optional: Vision-Analyse (VisionAnalyzer)
  4. Optional: OpenClaw Handoff
  5. Logging (SecurityLogger)
  6. Events auslösen
  ```

### Architektur

```
WinAssistantCoordinator
├── LocalConfiguration
├── SimpleRuleEngine (via Config konfiguriert)
├── ScreenAnalysisService
├── VisionAnalyzer (optional)
├── ContextDetector
├── OpenClawHandoffService (optional)
└── SecurityLogger
```

### Neue Kategorien (komplett)

| Kategorie | Regeln | Beschreibung |
|-----------|--------|--------------|
| Phishing | 6 | E-Mail/Link Phishing |
| Malware | 3 | Schädliche Software |
| SocialEngineering | 3 | Manipulation |
| FinancialFraud | 3 | Finanzbetrug |
| CredentialTheft | 3 | Passwortklau |
| InsecureConnection | 1 | HTTP |
| Suspicious | 1 | Allgemein |

**Gesamt: 20 Regeln**

## Version 0.2.0 - Integration (2026-04-17)

### Neue Komponenten

#### ScreenAnalysisService.cs
- **Volle Screen-Capture Integration**
  - Multi-Screen Support
  - Cursor-Tracking
  - Skalierte Screenshots
  - Text-Extraktion (Platzhalter für OCR)
  - Direkte Safety-Analyse

- **Context-Aware Analyse**
  - Automatische Kontext-Erkennung
  - Risiko-Bewertung pro Screen
  - Logging-System

#### VisionAnalyzer.cs
- **Llava/Ollama Vision Integration**
  - Ollama API Anbindung
  - Bildanalyse für Sicherheitsrisiken
  - OCR via Vision-Modell
  - Verdächtige UI-Element-Erkennung

- **Kombinierte Analyse**
  - Rule-Based + Vision
  - Höherer Risk-Score wenn Vision etwas erkennt
  - Detaillierte Empfehlungen

#### ContextDetector.cs
- **Windows API Integration**
  - GetForegroundWindow()
  - Process-Erkennung
  - Fenstertitel-Analyse

- **Automatische Kontext-Erkennung**
  - E-Mail Client (Outlook, Thunderbird, Web.de, GMX, etc.)
  - Browser (Chrome, Firefox, Edge, etc.)
  - Popup/Dialoge
  - System-Dialoge
  - File Explorer

- **Kontext-spezifische Empfehlungen**
  - E-Mail: Absender prüfen, Anhänge hinterfragen
  - Browser: URL/Zertifikat prüfen
  - Popup: Fake-Warnungen erkennen
  - System: Vertrauenswürdig wenn echt

### Verbesserungen

- Nahtlose Integration der neuen Services
- Flexible Fehlerbehandlung
- Detailliertes Logging
- OCR-Platzhalter für Llava-Integration vorbereitet

## Version 0.1.0 - Prototyp (2026-04-17)

### Neue Features

#### Safety-Layer Erweiterungen
- **14 neue Sicherheitsregeln** hinzugefügt:
  - Social Engineering Patterns (Impersonation, Fake Prize, Fake Support)
  - Financial Fraud Patterns (Payment Urgency, Fake Bank, Cryptocurrency)
  - Download/File Patterns (Suspicious Extensions, Crack/Piracy)
  - Credential Theft Patterns (Password Request, Fake 2FA)
  - Additional Urgency Patterns (Fake Deadline)

- **Konfigurations-System** implementiert:
  - `GetAllRules()` - Liste aller Regeln abrufen
  - `SetRuleEnabled(ruleId, enabled)` - Einzelne Regel aktivieren/deaktivieren
  - `SetCustomRiskScore(ruleId, score)` - Individueller RiskScore
  - `SetCategoryEnabled(category, enabled)` - Ganze Kategorie steuern
  - `ResetToDefaults()` - Auf Standard zurücksetzen

- **Erweiterte Empfehlungen** für alle neuen Kategorien

### Bug Fixes

- **SafetyUIManager.cs**:
  - `Color.FromArgb` korrigiert
  - `Point` Koordinate korrigiert

- **SimpleRuleEngine.cs**:
  - RiskScore korrigiert

---

## Technische Details

### Neue Enum: ContextSourceType
```csharp
Unknown         // Unbekannt
EmailClient     // Outlook, Thunderbird, Webmail
Browser         // Chrome, Firefox, Edge
Popup           // Popups, Dialoge
SystemDialog    // Windows System-Dialoge
FileExplorer    // Windows Explorer
```

### Service-Architektur

```
┌─────────────────────────────────────────────────────┐
│              ScreenAnalysisService                    │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────┐  │
│  │   Screen    │  │  Context     │  │  Rule     │  │
│  │  Capture    │→ │  Detector    │→ │  Engine   │  │
│  └─────────────┘  └──────────────┘  └───────────┘  │
│         ↓                                    ↓      │
│  ┌─────────────────┐                ┌──────────┐   │
│  │  VisionAnalyzer  │                │SafetyUI  │   │
│  │    (Llava)       │                │ Manager  │   │
│  └─────────────────┘                └──────────┘   │
└─────────────────────────────────────────────────────┘
```

### Konfigurations-Beispiele
```csharp
// Screen-Analyse
var analysis = screenService.AnalyzeScreen();

// Kontext-Erkennung
var context = contextDetector.DetectCurrentContext();
Console.WriteLine($"Kontext: {context.DetectedType}");

// Vision-Analyse
var visionResult = await vision.AnalyzeImageAsync(imageBytes);

// Kombinierte Analyse
var combined = await vision.CombinedAnalysisAsync(text, imageBytes, ruleEngine);
```

## Geplante Features (Backlog)

- [ ] Echte OCR-Integration (Llava fertigstellen)
- [ ] Local-First Konfiguration (JSON Config File)
- [ ] UI für Regel-Konfiguration
- [ ] History/Logging System
- [ ] OpenClaw Handoff Integration
- [ ] Echte Windows-Build Tests
- [ ] Installer-Paket erstellen
- [ ] Sound-Benachrichtigungen
- [ ] Animationen für Warnungen

## Bekannte Limitationen

- OCR benötigt Llava-Integration (noch nicht getestet)
- Kontext-Erkennung abhängig von Fenster-Titeln
- Vision-Analyse benötigt laufenden Ollama-Server
- Build noch nicht auf echter Windows-Maschine getestet

---

**Nächste Version:** Geplant für wenn alle Kernfeatures implementiert und getestet sind.
