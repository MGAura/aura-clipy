# PC-BEGLEITER GAP-ANALYSIS
**Erstellt:** 2026-04-16  
**Basis:** Aura-Clipy/Zippy-for-Windows Codebase Review  
**Ziel:** Identifizieren was für MVP PC-Begleiter fehlt

## 1. EXISTIERENDE KOMPONENTEN (Wiederverwendbar)

### 1.1 UI/Overlay System (✅ Vorhanden)
/B **WinForms Application** mit MainForm
- **Multi-Screen Support** (Screen.AllScreens)
. **Cursor-Priorisierung** (identifiziert welcher Screen Cursor hat)
- **Transparente/Overlay Fenster** (konzeptionell)
- **Settings/Config System** (JSON-basiert)

### 1.2 Screen-Capture Pipeline (✅ Vorhanden)
- **Multi-Screen Capture** (alle Bildschirme gleichzeitig)
- **Scaled Screenshots** (skalierte Bilder für Übertragung)
. **Screen Labeling** ("screen X of Y - cursor is on this screen")
- **Bitmap Encoding** (Base64 für API Übertragung)

### 1.3 Sprachintegration (✅ Teilweise)
- **Push-to-Talk** (Hotkey F8 konfigurierbar)
+ **Speech-to-Text** (Whisper lokal oder ElevenLabs cloud)
. **Text-to-Speech** (ElevenLabs API)

### 1.4 OpenClaw Integration (✅ Vorhanden)
- **WebSocket Gateway Connection** (ws://127.0.0.1:18789)
- **Session Management** (Session Key "main")
- **Handoff Mechanism** (One-shot Delegation an OpenClaw)
- **Timeout Handling** (configurable timeout seconds)

### 1.5 Ollama Vision Integration (✅ Vorhanden)
+ **Llava Model Support** (llava:7b default)
- **Vision API Calls** (Screenshot + Prompt an Ollama)
- **Local Processing** (localhost:11434)

## 2. FEHLENDE KOMPONENTEN FÜR PC-BEGLEITER (GAPS)

### 2.1 SAFETY-LAYER (❌ KOMPLETT FEHLEND)
**Beschreibung:** Deterministische Sicherheitsregeln für Risikobewertung

**Fehlende Komponenten:**
1. **Regel-Engine** - Heuristische Risikomuster-Erkennung
2. **Phishing-Pattern Matching** - E-Mail/Link/Webseiten Analyse
3. **Popup/Dialog Risk Assessment** - Verdächtige System-Dialoge
4. **File/Attachment Scanner** - Anhang-Risikobewertung
5. **Risk-Scoring System** - Quantifizierte Risikobewertung (0-10)
6. **Evidence Collection** - Screenshot + Kontext für Regelauswertung

**Priorität:** **HIGH** (Kernfunktion des PC-Begleiters)

### 2.2 LOCAL-First Konfiguration & Transparenz (❌ TEILWEISE)
**Beschreibung:** Klare lokale vs. cloud Modi + Transparenz für Nutzer

**Fehlende Komponenten:**
1. **Modus-Konfiguration** (lokal/hybrid/cloud) mit UI-Steuerung
2. **Transparenz-UI** - Anzeige wo Verarbeitung stattfindet
3. **Privacy Indicators** - "Lokal verarbeitet" vs. "Cloud-Analyse"
4. **Performance Monitoring** lokaler Komponenten
5. **Fallback-Logik** bei lokalen Komponenten-Ausfall

**Priorität:** **HIGH** (Differenzierungsmerkmal des Produkts)

### 2.3 PC-BEGLEITER SPEZIFISCHE UI (❌ TEILWEISE)
**Beschreibung:** UI optimiert für Sicherheitshinweise & verständliche Empfehlungen

**Fehlende Komponenten:**
1. **Risk Color Coding** - grün/gelb/rot für Risikobewertung
2. **Safety Recommendation UI** - klare, verständliche Empfehlungen
3. **Step-by-Step Navigation Helper** - sichere Alternativen zeigen
4. **"Analysiere Sicherheit..." Status** - transparente Prozessanzeige
5. **Verdächtigkeits-Indikatoren** - visuelle Hinweise auf Risiken

**Priorität:** **MEDIUM** (User Experience entscheidend)

### 2.4 SPEZIFISCHE RISIKO-SZENARIEN HANDLING (❌ FEHLEND)
**Beschreibung:** Konkrete Behandlung typischer PC-Risikosituationen

**Fehlende Szenarien-Handling:**
1. **E-Mail Security Check** - verdächtige Mails erkennen & bewerten
2. **Link Risk Assessment** - Phishing-Links identifizieren
3. **Popup Warning Evaluation** - verdächtige Dialoge bewerten
4. **Browser Safety Navigation** - sichere Alternativen vorschlagen
5. **Download Risk Warning** - verdächtige Downloads markieren

**Priorität:** **HIGH** (MVP Kernfunktionalität)

### 2.5 INTEGRIERTE SICHERHEITS-RECHTLINIEN (❌ FEHLEND)
**Beschreibung:** Vordefinierte, verständliche Sicherheitsrichtlinien

**Fehlende Richtlinien:**
1. **Phishing Detection Rules** - typische Betrugsmuster
2. **Social Engineering Patterns** - Druck/Dringlichkeit/Zeitdruck
3. **Financial Risk Indicators** - Zahlungsaufforderungen
4. **Credential Theft Patterns** - Login/Account-Phishing
5. **Malware Distribution Signs** - verdächtige Downloads/Anhänge

**Priorität:** **MEDIUM** (Kann initial einfache Regeln haben)

## 3. ARCHITEKTUR-MAPPING (Bestehend → PC-Begleiter)

### 3.1 Client Layer (✅ EXISTIERT)
**Aura-Clipy:** WinForms App mit Overlay-Konzept  
**PC-Begleiter:** UI-Anpassungen für Sicherheitshinweise + Transparenz

### 3.2 Context Layer (✅ EXISTIERT)
**Aura-Clipy:** Multi-Screen Capture + Cursor-Priorisierung  
**PC-Begleiter:** Zusätzlich Kontext für Risikoanalyse (E-Mail/Browser/Popup)

### 3.3 Safety Layer (❌ NEU ZU IMPLEMENTIEREN)
**Aura-Clipy:** Keine Sicherheitsregeln  
**PC-Begleiter:** Deterministische Regel-Engine + Risk-Scoring

### 3.4 Reasoning Layer (✅ TEILWEISE)
**Aura-Clipy:** Ollama Vision + OpenClaw Handoffs  
**PC-Begleiter:** Erweiterung für Sicherheitsfragen + Risiko-Eskalation

### 3.5 Action Layer (✅ TEILWEISE)
**Aura-Clipy:** Antworten + einfache Navigation  
**PC-Begleiter:** Sicherheitsempfehlungen + Schritt-für-Schritt sichere Alternativen

## 4. ENTSCHEIDUNG: ANPASSEN vs. NEUIMPLEMENTIERUNG

### **Empfehlung: ANPASSEN der bestehenden Codebase**
**Gründe:**
1. **Screen-Capture Pipeline** bereits voll funktionsfähig
2. **OpenClaw Integration** vorhanden und getestet
3. **Sprachintegration** Grundgerüst existiert
4. **UI/Overlay System** WinForms Basis stabil
5. **Build-System** (.cmd/.ps1) funktioniert

### **Anpassungs-Strategie:**
1. **Safety-Layer als neue Komponente** hinzufügen
2. **UI für Sicherheitshinweise** erweitern
3. **Local-First Konfiguration** integrieren
4. **Spezifische Risiko-Handler** implementieren
5. **Bestehende Codebase als Foundation** nutzen

## 5. MVP IMPLEMENTIERUNGSPLAN

### **Phase 1: Safety-Layer Grundgerüst**
1. Rule-Engine für einfache Risikomuster (E-Mail Phishing)
2. Risk-Scoring System (0-10 Punkte)
3. UI Integration für Sicherheitsbewertungen
4. Transparenz-Anzeige für Verarbeitungsart

### **Phase 2: Spezifische Risiko-Szenarien**
1. E-Mail Security Check (verdächtige Absender, Links, Anhänge)
2. Link Risk Assessment (Phishing-URL Detection)
3. Popup/Dialog Warning Evaluation
4. Browser Safety Navigation Helper

### **Phase 3: Local-First Optimierung**
1. Lokale vs. Cloud Modi konfigurierbar
2. Performance Monitoring lokaler Komponenten
3. Fallback-Logik bei Cloud-Ausfall
4. Privacy-First UI Indikatoren

### **Phase 4: UX-Verbesserungen**
1. Color-Coded Risk Anzeige
2. Verständliche Sicherheitsempfehlungen
3. Step-by-Step sichere Alternativen
4. Lernmodus für wiederkehrende Unsicherheiten

## 6. TECHNISCHE ENTWICKLUNGSPRIORITÄTEN

### **P1 (SOFORT):** Safety-Layer Grundimplementierung
- Rule-Engine mit einfachen Phishing-Patterns
---

 Risk-Scoring (0-10)
 - UI Integration für Sicherheitshinweise

### **P2 (KURZFRISTIG):** E-Mail Security MVP
- E-Mail Screenshot-Analyse
-- Phishing-Pattern Matching
- Risikobewertung + Empfehlungen
- Sichere Alternativen vorschlagen

### **P3 (MITTELFRISTIG):** Local-First Konfiguration
- Betriebsmodi (lokal/hybrid/cloud)
- Transparenz-UI
- Performance Monitoring
- Privacy Indikatoren

### **P4 (LANGFRISTIG):** Vollständiger PC-Begleiter
- Alle Risiko-Szenarien
-B UX-Optimierungen
- Lernmodus & History
– Family/Team-Funktionen

## 7. NÄCHSTE KONKRETE SCHRITTE

### **Task 1.2: Safety-Layer Prototyp implementieren**
1. **SimpleRuleEngine.cs** - Basis-Rule-Engine
2. **PhishingPatternMatcher.cs** - E-Mail/Link Pattern Matching
3. **RiskScorer.cs** - Risk-Scoring (0-10)
4. **SafetyUIManager.cs** - UI Integration für Sicherheitshinweise

### **Task 1.3: Aura-Clipy Codebase anpassen**
1. **MainForm erweitern** - Safety-Hinweise UI
2. **ScreenCaptureService erweitern** - Kontext für Risikoanalyse
3. **OpenClaw Integration anpassen** - Sicherheitsfragen spezifisch
4. **Build-System aktualisieren** - Neue Komponenten integrieren

### **Task activity, 4: Test-Szenarien erstellen**
1. **Test-E-Mail Screenshots** (verdächtig vs. legitim)
2. **Test-Phishing Links** (Fake vs. legitime URLs)
3. **Test-Popup Dialoge** (verdächtig vs. system)
4. **Integration Tests** - End-to-End Sicherheitsbewertung

---
**Fazit:** Aura-Clipy Codebase bietet **solide Foundation** (70% wiederverwendbar).  
**Safety-Layer muss neu implementiert werden** (Kernfunktion PC-Begleiter).  
**Empfehlung:** Anpassen + Erweitern, nicht neu implementieren.