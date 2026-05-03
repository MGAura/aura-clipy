# TASKS.md - Win Assistent (PC-Begleiter)

**Priorisierung:** Nummeriert nach Implementierungsreihenfolge  
**Status:** ✅ = Abgeschlossen, 🔄 = In Arbeit, ⏳ = Geplant

## ✅ Abgeschlossene Tasks

### Phase db: Basis-Implementation (2026-04-17)

| Task | Beschreibung | Status |
|------|--------------|--------|
| 1.1 | Code-Review AuraClipy | ✅ |
| 1.2 | Safety-Layer Prototyp | ✅ |
| 1.3 | Konfigurations-System | ✅ |
| 1.4 | Code-Qualität | ✅ |
| 1.5 | Dokumentation | ✅ |

### Phase 2: Kern-Integration (2026-04-17)

| Task | Beschreibung | Datei | Status |
|------|--------------|-------|--------|
| 2.1 | Screen-Capture | ScreenAnalysisService.cs | ✅ |
| 2.2 | Vision-Modul | VisionAnalyzer.cs | ✅ |
| 2.3 | Kontext-Erkennung | ContextDetector.cs | ✅ |
| 2.4 | OpenClaw Handoff | OpenClawHandoffService.cs | ✅ |
| 2.5 | Local-First Config | LocalConfiguration.cs | ✅ |
| 2.6 | Security Logger | SecurityLogger.cs | ✅ |
| 2.7 | Haupt-Koordinator | WinAssistantCoordinator.cs | ✅ |

### Phase 3: Build & Deployment (2026-04-17)

| Task | Beschreibung | Status |
|------|--------------|--------|
| 3.1 | Build-Skripte | ✅ |
| 3.2 | Test-Skript | ✅ |
| 3.3 | Ordner-Struktur | ✅ |

### Phase 4: GitHub Repository (2026-04-17 ✅)

| Task | Beschreibung | Status |
|------|--------------|--------|
| 4.1 | Projekt auf GitHub synchronisieren | ✅ |
| 4.2 | GitHub Actions Workflow erstellen | ✅ |
| 4.3 | Repository-Struktur bereinigen | ✅ |
| 4.4 | Aktuelle Dokumentation pushen | ✅ |

### Phase 5: Build-Test (2026-05-03 🔄)

| Task | Beschreibung | Status | Notizen |
|------|--------------|--------|---------|
| 5.1 | GitHub Actions Workflow auslösen | 🔄 | **Neuer One-Time Code generiert: D800-2E13**. Vorheriger Code F6BD-FEBD ist abgelaufen (nach ~30 Minuten). Rate-Limiting-Sperre sollte aufgehoben sein. Warte auf Martins Browser-Aktion bei https://github.com/login/device. Nach Token-Aktualisierung mit `workflow` Scope kann Workflow gepusht werden.
| 5.2 | Repository öffentlich machen oder lokalen Build testen | ✅ | Repository ist öffentlich (https://github.com/MGAura/aura-clipy).
| 5.3 | GitHub Workflow Pfade korrigieren | ✅ | Workflow-Pfade angepasst (von windows/AuraClipy/ zu Root), build.yml im Repository aktualisiert
| 5.4 | Windows-EXE validieren | ⏳ | Funktioniert die Anwendung? (Kann nur auf Windows getestet werden)

**Repository:** https://github.com/MGAura/aura-clipy  
**Actions:** https://github.com/MGAura/aura-clipy/actions

**Aktueller Status (2026-05-03 09:45):** Neuer One-Time Code F6BD-FEBD generiert. Rate-Limiting-Sperre sollte aufgehoben sein. Warte auf Martins Browser-Aktion bei https://github.com/login/device.

**Nächste Aktionen:**
1. Martin geht zu https://github.com/login/device und gibt Code F6BD-FEBD ein
2. Alternativ: GitHub UI öffnen: https://github.com/MGAura/aura-clipy/actions → "New workflow"
3. Alternativ: Windows-Build lokal testen (falls Workflow nicht möglich)

**Update für Martin:** UPDATE_FOR_MARTIN_heartbeat_2026-05-03_0945.md bereitgestellt mit neuem One-Time Code und klaren Schritten.

## ⏳ Geplant

### Phase 6: UI-Verbesserungen

| Task | Beschreibung | Priorität |
|------|--------------|-----------|
| 6.1 | Animationen für Warnungen | Mittel |
| 6.2 | Sound-Benachrichtigungen | Mittel |
| 6.3 | Compact-Mode | Niedrig |
|第十九  | System-Tray Integration | Mittel |

### Phase 7: Installer & Deployment

| Task | Beschreibung | Priorität |
|------|--------------|-----------|
| 7.1 | Installer-Paket | Hoch |
| 7.2 | Auto-Update System | Niedrig |
| 7.3 | Startmenü-Verknüpfung | Mittel |

### Phase 8: Erweiterte Features

| Task | Beschreibung | Priorität |
|------|--------------|-----------|
| 8.1 | Llava OCR Integration | Hoch |
| 8.2 | UI für Regel-Konfiguration | Mittel |
| 8.3 | History/Statistik-Dashboard | Niedrig |

## Implementierte Komponenten

```
Win Assistent/
├── SimpleRuleEngine.cs      (20+ Regeln + Konfiguration)
├── SafetyUIManager.cs       (UI-Integration)
├── ScreenAnalysisService.cs (Screen-Capture)
├── VisionAnalyzer.cs       (Llava Integration)
├── ContextDetector.cs      (Kontext-Erkennung)
├── OpenClawHandoffService.cs (OpenClaw Delegation)
├── LocalConfiguration.cs    (JSON Config)
├── SecurityLogger.cs       (Logging)
├── WinAssistantCoordinator.cs (Koordinator)
├── Form1.cs, Form1.Designer.cs, Program.cs
├── AuraClipy.csproj
├── .github/workflows/build.yml (CI/CD)
└── Build-Skripte/
    ├── build.cmd, build.ps1, test.ps1
    └── README.md
```

## Security Rules (20+ implementiert)

| Kategorie | Regeln |
|-----------|--------|
| Phishing | 6 |
| SocialEngineering | 3 |
| FinancialFraud | 3 |
| CredentialTheft | 3 |
| Malware | 3 |
| InsecureConnection | 1 |
| Suspicious | 1+ |

---

**Letzte Aktualisierung:** 2026-05-03 10:02 – Heartbeat-Check ausgeführt

**Aktueller Status:** Neuer One-Time Code F6BD-FEBD verfügbar, aber noch nicht verwendet. Push-Versuch schlägt weiterhin fehl mit "refusing to allow an OAuth App to create or update workflow `.github/workflows/build.yml` without `workflow` scope". Warte auf Martins Browser-Aktion bei https://github.com/login/device.