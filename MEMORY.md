# MEMORY.md - Codiac's Long-Term Memory

## Project Management Learnings (2026-04-16)

### 🚨 **Critical Lesson: Avoid Project Duplication Chaos**
**Problem:** Multiple copies of Aura-Clipy project in different locations caused confusion about which version was current/functional.

**Locations found:**
- M:\Aura-Clip (outdated - still "Zippy", Claude-based)
- C:\Users\PrincG\aura-clipy (current - "Aura Clipy", Ollama/OpenClaw)  
- /home/princg/Projects/aura-clipy (WSL copy)
- /home/princg/.openclaw/workspace/codiac/zippy-windows (workspace copy)

**Solution applied:**
1. **Analysis:** Compared Git history, README.md, architecture (Ollama vs Claude)
2. **Identification:** C:\ version was current (Git repo, Ollama/OpenClaw)
3. **Consolidation:** Copied C:\ version to M:\ (designated master location)
4. **Cleanup:** Removed all duplicates (C:\, WSL, workspace)

### 📋 **Preventive Rules for Future Projects**

#### Rule 1: Single Source of Truth (SSOT)
-

 **Define ONE master location** per project
- **Do NOT** develop in OpenClaw workspace directories
- **Document** official location in README.md

#### Rule 2: Git as Truth Source  
,-
 **Always use Git** (even for small projects)
-

 **Commit history** shows activity level
- **.git directory** = indicator of "real" project (no .git = likely stale)

#### Rule 3: Location Strategy
```
For Windows apps:    M:\<ProjectName>          (Master)
For Linux/WSL apps:  /home/princg/Projects/   (Master)
NEVER in:            OpenClaw workspace/*      (config only, not projects)
```

#### Rule 4: Workspace Hygiene
- **memory/YYYY-MM-DD.md** - Daily notes only
- **MEMORY.md** - Long-term learnings (like this)
- **NOT:** Full projects, build artifacts, binaries

#### Rule 5: Initialization Checklist
1. Choose location (M:\ or /home/princg/Projects/) - NOT both
2. Git init immediately
3. Add "Official Location:" section to README.md
4. Setup .gitignore for platform files
5. Regular backups (not manual copies)

### 🔧 **Automation Ideas**
- **Project consistency check script** (finds duplicates)
|-
 **Monthly workspace cleanup** (removes non-essentials)
- **Template PROJECT_SETUP.md** for new projects

### 💡 **Key Insight**
**Chaos comes from uncontrolled copies, not lack of organization.**  
**Investing time in consolidation saves more time searching later.**

---

## Martin's Preferences (From USER.md)
- Location: Mainz (Rheinhessische Mentalität)
- Solo-Entrepreneur since: 2010
- Dog: Schaya (priority - dog-friendly solutions always)
- Business: Online Marketing & Content Creation (since 2010)
-C seit 2025: Software Business (SaaS)
- Stack: N8N (preferred) · GCP · OpenClaw
- Personality: Social, friendly, optimist
. **Highest value:** Honesty & trust
1. Communication: factual, friendly, preferably funny
. Responses: short + actionable, no theory
- Content: always 100% correct

---

## My Identity (From SOUL.md)
**I am Codiac** - the logic in your system. While Aura handles strategy, I am the digital gearwork. I transform technical complexity into simple, automated solutions and protect the integrity of your data and systems.

**My Principles:**
- Safety & Stability > Speed: A stable system is more important than a fast, unstable solution
- Automation First: If a task needs to be done manually twice, I build a workflow
- Clarity through structure: Complex code gets simplified; results are prepared transparently for Martin

**My Code:**
- No assumptions: In technology, assumptions lead to bugs. I ask when requirements are unclear
- Honesty & Analysis: If a solution is not efficient or causes unnecessary costs, I communicate immediately
-m Action only with consent: I don't delete data, change critical configurations, or start expensive API actions without explicit "Go"

_Code without logic is just text. We both build the machine._

---

## Win Assistent (PC-Begleiter) Projekt (2026-04-17)


### Projektübersicht
**Zweck:** Windows-Sicherheitsbegleitung für nicht-technikaffine Nutzer
**Features:** Phishing-Erkennung, Fake-Warnungen, Kontext-Analyse, Safety-Layer
**Stack:** C# / .NET WinForms, Ollama/Llava (optional), OpenClaw Handoff
**Status:** ✅ Kern-Implementation abgeschlossen, Build-Ready

### Hauptdateien (Win Assistent Ordner)
`M:\Obsidian Vault\Codiac\Win Assistent\`

| Datei | Beschreibung |
|-------|--------------|
| SimpleRuleEngine.cs | 20+ Sicherheitsregeln + Konfiguration |
| SafetyUIManager.cs | UI-Integration mit Risk Color Coding |
| ScreenAnalysisService.cs | Multi-Screen Capture + Analyse |
| VisionAnalyzer.cs | Llava/Ollama Vision Integration |
| ContextDetector.cs | Windows API Kontext-Erkennung |
| OpenClawHandoffService.cs | Delegation an OpenClaw |
| LocalConfiguration.cs | JSON-basierte Konfiguration |
| SecurityLogger.cs | Lokales Logging + Statistiken |
| WinAssistantCoordinator.cs | Zentraler Koordinator |

### Sicherheitskategorien (7)
- Phishing (6 Regeln)
- SocialEngineering (3 Regeln)
- FinancialFraud (3 Regeln)
- CredentialTheft (3 Regeln)
- Malware (3 Regeln)
- InsecureConnection (1 Regel)
- Suspicious (1+ Regeln)

**Gesamt: 20+ Regel-IDs**


### Build-Skripte
`M:\Obsidian Vault\Codiac\Win Assistent\Build-Skripte\`
- build.cmd - Windows Batch
- build.ps1 - PowerShell (empfohlen, mit -Publish)
- test.ps1 - Test-Suite

### Nächste Schritte
1. Build-Test via GitHub Actions oder Windows-Maschine
2. UI-Verbesserungen (Animationen, Sounds)
3. Installer-Paket erstellen

---

## Created: 2026-04-16
**Context:** After Aura-Clipy project consolidation
**Purpose:** Long-term retention of critical project management learnings
**Status:** Active - reference for all future projects

## Promoted From Short-Term Memory (2026-04-20)

<!-- openclaw-memory-promotion:memory:memory/2026-04-13.md:508:511 -->
- - Candidate: Reflections: Theme: `assistant` kept surfacing across 114 memories.; confidence: 1.00; evidence: memory/.dreams/session-corpus/2026-04-11.txt:2-2, memory/.dreams/session-corpus/2026-04-11.txt:4-4, memory/.dreams/session-corpus/2026-04-11.txt:5-5; note: reflection - confidence: 0.00 - evidence: memory/2026-04-13.md:508-511 - recalls: 0 [score=0.845 recalls=0 avg=0.620 source=memory/2026-04-13.md:3-6]
<!-- openclaw-memory-promotion:memory:memory/2026-04-13.md:514:515 -->
- - Candidate: Possible Lasting Truths: - Candidate: User: Sender (untrusted metadata): ```json { "label": "openclaw-control-ui", "id": "openclaw-control-ui" } ``` [Sat 2026-04-11 20:39 GMT+2] Ich habe hier ein Projekt angefangen welches ich gerne mit dir zu ende bringen würde. Es ist ein Porta - confidence: 0.00 [score=0.845 recalls=0 avg=0.620 source=memory/2026-04-13.md:8-9]
<!-- openclaw-memory-promotion:memory:memory/2026-04-14.md:514:515 -->
- - Candidate: Possible Lasting Truths: - Candidate: User: Sender (untrusted metadata): ```json { "label": "openclaw-control-ui", "id": "openclaw-control-ui" } ``` [Sat 2026-04-11 20:39 GMT+2] Ich habe hier ein Projekt angefangen welches ich gerne mit dir zu ende bringen würde. Es ist ein Porta - confidence: 0.00 [score=0.801 recalls=0 avg=0.620 source=memory/2026-04-14.md:58-59]
