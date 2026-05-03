# Win Assistent - PC-Begleiter

**Sicherheitsbegleitung für nicht-technikaffine Windows-Nutzer**

## Was ist das?

Win Assistent (PC-Begleiter) ist eine Windows-Desktop-Anwendung, die Benutzer vor Online-Bedrohungen schützt. Die App analysiert Bildschirminhalte und warnt vor:
- Phishing-E-Mails
- Fake-Warnungen und Popups
- Betrügerische Webseiten
- Identitätsdiebstahl-Versuche
- Malware-Download-Links

## Kernfeatures

### 🔒 Safety-Layer (20+ Regeln)
- **20+ Sicherheitsregeln** für automatische Risikoerkennung
- **Risk Score** (0-10) für jede Analyse
- **Farbcodierte Warnungen** (Grün/Gelb/Orange/Rot)
- **Konfigurierbare Regeln** - eigene Gewichtung möglich

### 🌍 Local-First Architektur
- **Alle Analysen lokal** - keine Cloud-Abhängigkeit
- **JSON Konfiguration** in `%APPDATA%`
- **Lokales Logging** in `%LOCALAPPDATA%`
- **Privacy-First** - keine Daten verlassen den Computer

### 🤖 Intelligente Analyse
- **Rule-Engine** - schnelle Mustererkennung
- **Vision-Analyzer** - Llava/Ollama für OCR (optional)
- **Kontext-Erkennung** - E-Mail/Browser/Popup automatisch erkennen
- **OpenClaw Handoff** - delegiert komplexe Fragen an OpenClaw

### 💡 Verständliche Empfehlungen
- **Klare Warnungen** in einfacher Sprache
- **Sichere Alternativen** werden vorgeschlagen
- **Schritt-für-Schritt Hilfe** für unsichere Situationen

## Installation & Build

### Voraussetzungen
- Windows 10/11
- .NET SDK 10.0 (für Build)
- PowerShell 5.1+ (für Scripts)

### Build ausführen
```powershell
cd "Win Assistent/Build-Skripte"
.\build.ps1 -Publish
```

### Tests ausführen
```powershell
cd "Win Assistent/Build-Skripte"
.\test.ps1 -Verbose
```

Ausführbare Datei: `publish/win-x64/AuraClipy.exe`

## Projektstruktur

```
Win Assistent/
├── *.cs                    # Alle C# Quelldateien
├── AuraClipy.csproj        # .NET Projekt
├── Dokumentation/
│   ├── produktkonzept-pc-begleiter.md
│   └── CHANGELOG.md
├── Build-Skripte/
│   ├── build.cmd           # Batch Build
│   ├── build.ps1          # PowerShell Build (empfohlen)
│   ├── test.ps1           # Tests
│   └── README.md
├── publish/                # Build-Ausgabe
└── README.md
```

## Komponenten

| Komponente | Beschreibung |
|------------|--------------|
| SimpleRuleEngine.cs | 20+ Sicherheitsregeln |
| SafetyUIManager.cs | UI-Integration |
| ScreenAnalysisService.cs | Screen-Capture |
| VisionAnalyzer.cs | Llava/Ollama Integration |
| ContextDetector.cs | Windows API Kontext-Erkennung |
| OpenClawHandoffService.cs | OpenClaw Delegation |
| LocalConfiguration.cs | JSON Config |
| SecurityLogger.cs | Lokales Logging |
| WinAssistantCoordinator.cs | Zentraler Koordinator |

## Sicherheitskategorien (7)

| Kategorie | Regeln | Beschreibung |
|-----------|--------|--------------|
| Phishing | 6 | E-Mail/Link Phishing |
| Malware | 3 | Schädliche Software |
| Social Engineering | 3 | Manipulation |
| Financial Fraud | 3 | Finanzbetrug |
| Credential Theft | 3 | Passwortklau |
| Insecure Connection | 1 | Unverschlüsselt |
| Suspicious | 1 | Allgemein |

## Verwendung (nach Build)

### Nach dem Start
1. Anwendung zeigt Hauptfenster
2. Text zur Analyse eingeben oder Screenshot machen lassen
3. Kontext wählen (E-Mail, Browser, Popup)
4. "Sicherheit analysieren" klicken
5. Ergebnis und Empfehlungen werden angezeigt

### Konfiguration
```csharp
// Regel deaktivieren
ruleEngine.SetRuleEnabled("PHISH_EMAIL_PERSONAL", false);

// RiskScore anpassen
ruleEngine.SetCustomRiskScore("PHISH_EMAIL_PERSONAL", 5);

// Kategorie deaktivieren
ruleEngine.SetCategoryEnabled(RiskCategory.SocialEngineering, false);

// Zurücksetzen
ruleEngine.ResetToDefaults();
```

## Status

**Aktuelle Version:** 0.3.0

- ✅ Safety-Layer mit 20+ Regeln
- ✅ Konfigurations-System
- ✅ Screen-Capture Integration
- ✅ Vision/OCR (Llava ready)
- ✅ Kontext-Erkennung
- ✅ OpenClaw Handoff
- ✅ Local-First Architektur
- ✅ Security Logging
- ✅ Build-Skripte erstellt
- 🔄 Build-Test steht aus

## Lizenz

Proprietär - Nur für internen Gebrauch

## Kontakt

Für Fragen und Feedback: Martin (via OpenClaw)
# Build trigger
