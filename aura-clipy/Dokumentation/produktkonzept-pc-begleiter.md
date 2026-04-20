# Produktkonzept: PC-Begleiter

## Überblick
PC-Begleiter ist als vertrauenswürdiger Windows-Assistent für nicht technikaffine Nutzer gedacht. Das Produkt soll Menschen im Alltag am Computer begleiten, Gefahren besser einordnen, digitale Unsicherheit reduzieren und bei konkreten Aufgaben am Bildschirm helfen, ohne ihnen die Verantwortung für ihr Handeln abzunehmen. Die Grundidee baut auf einem screen-aware Desktop-Assistenten auf, wie er im analysierten Zippy-for-Windows-Repo bereits technisch angedeutet ist: cursor-nahes Overlay, Bildschirmverständnis, Sprachsteuerung, lokale Tool-Handoffs und eine local-first Ausrichtung.[cite:42][cite:43][cite:44]

Ein zentraler strategischer Gedanke ist die lokale Betriebsfähigkeit. Das zugrunde liegende Repo ist bereits in Teilen local-first ausgelegt, etwa mit lokalem Whisper für Speech-to-Text und lokalen Handoffs an CLI-Tools wie OpenClaw, während einige Hauptfunktionen derzeit noch cloudbasiert sind.[cite:42][cite:44] Für das Zielprodukt ist genau diese Entwicklungsrichtung entscheidend: Je leistungsfähiger lokale Modelle werden, desto stärker kann sich das Produkt zu einem privat betriebenen, datensparsamen und vertrauenswürdigen Windows-Begleiter entwickeln, der den Gedanken „ich werde ausspioniert“ aktiv entschärft.[cite:44]

## Produktvision
Die langfristige Vision ist ein persönlicher Desktop-Begleiter, der Sprache, Text und Bildschirmkontext versteht und Nutzern in schwierigen oder riskanten PC-Situationen Orientierung gibt. Das Produkt soll nicht als klassischer Chatbot wahrgenommen werden, sondern als sichtbare, verständliche und vertrauensvolle Hilfe direkt dort, wo Probleme entstehen: auf dem Bildschirm des Nutzers.[cite:44][cite:50]

Das Produktversprechen lautet: Der Nutzer behält immer die Entscheidungshoheit, erhält aber bessere Informationen, klare Hinweise und konkrete Schritt-für-Schritt-Unterstützung. Besonders in Bereichen wie Phishing, verdächtigen Anhängen, fragwürdigen Download-Aufforderungen, irreführenden Webseiten oder verwirrenden Systemeinstellungen entsteht daraus ein hoher Alltagsnutzen.[cite:50]

## Problem und Marktbedarf
Viele Nutzer scheitern im digitalen Alltag nicht an fehlender Intelligenz, sondern an fehlender Einordnung in Echtzeit. Sie sehen eine E-Mail, ein Popup, einen Download-Hinweis oder eine Kontowarnung und wissen nicht, ob sie reagieren, abbrechen oder Hilfe holen sollen.[cite:50]

Hinzu kommt, dass klassische Hilfekanäle wie FAQ-Seiten, Dokumentationen, Support-Foren oder Video-Tutorials oft zu spät, zu abstrakt oder zu kompliziert sind. Ein Assistent, der den tatsächlichen Bildschirmzustand sieht und in natürlicher Sprache erklärt, was gerade los ist, senkt diese Hürde erheblich. Das Repo zeigt bereits, dass ein solcher Ansatz mit Multi-Screen-Capture, Prompting auf Basis visueller Eingaben, Sprachsteuerung und zielgenauer Pointer-Logik technisch realisierbar ist.[cite:42][cite:44]

## Zielgruppe
Die Kernzielgruppe sind Windows-Nutzer mit geringer bis mittlerer technischer Kompetenz. Dazu gehören ältere Menschen, Selbstständige ohne IT-Hintergrund, Familienmitglieder, Bürokräfte und Personen, die digitale Aufgaben erledigen müssen, aber sich dabei oft unsicher fühlen.[cite:50]

Eine zweite Zielgruppe sind indirekte Käufer oder Multiplikatoren: kleine Unternehmen, IT-Dienstleister, Familienangehörige, Coaches oder Bildungseinrichtungen, die Menschen sicherer und souveräner im Umgang mit dem PC machen möchten. Für eine spätere Produktstrategie ist das relevant, weil daraus sowohl B2C- als auch B2B-Modelle ableitbar sind.[cite:48][cite:50]

## Kernnutzen
Der größte Nutzen des Produkts liegt in drei Funktionen:

- Erkennen: riskante, verwirrende oder erklärungsbedürftige Situationen am Bildschirm einordnen.[cite:50]
- Erklären: dem Nutzer in einfacher Sprache sagen, was gerade passiert und warum Vorsicht oder eine bestimmte Handlung sinnvoll ist.[cite:50]
- Begleiten: konkrete Navigation und Schritt-für-Schritt-Hilfe direkt am sichtbaren Interface geben, statt nur allgemeine Textantworten auszuliefern.[cite:42]

Dadurch wird aus abstrakter KI ein alltagsnahes Assistenzprodukt. Das Repo liefert mit dem always-on Cursor-Companion, der Screen-Capture-Pipeline, den Zuständen Listening/Thinking/Speaking und der Pointer-Logik bereits technische Vorläufer dieser Experience.[cite:42][cite:44]

## Positionierung
PC-Begleiter sollte nicht als „AI-Agent-Plattform“ vermarktet werden. Für die Zielgruppe wären Begriffe wie OpenClaw, Orchestrator, Vision-Modell oder Whisper eher Abschreckung als Mehrwert.[cite:47][cite:50]

Die Positionierung sollte vielmehr auf Vertrauen, Verständlichkeit und Sicherheit beruhen. Geeignete Kernbotschaften wären:

- Dein Helfer für schwierige Situationen am PC.
- Erkennt Risiken und erklärt, was zu tun ist.
- Hilft beim Verstehen statt nur beim Antworten.
- Kann lokal betrieben werden und respektiert Privatsphäre.[cite:44]

## Sicherheitsrahmen
Das Produkt darf kein falsches Schutzversprechen geben. Es soll nicht behaupten, Verantwortung für Nutzerhandlungen zu übernehmen, sondern bekannte Risikomuster sichtbar machen und gute digitale Verhaltensregeln im richtigen Moment vermitteln.[cite:50]

Für typische Alltagsszenarien kann das Produkt dennoch klare Empfehlungen geben. Beispiele:

- Unbekannte Anhänge sollten nicht geöffnet werden, wenn der Absender nicht verifiziert ist.[cite:50]
- Zugangsdaten sollten nicht über Links in Mails geändert werden; offizielle Seiten sollten besser manuell aufgerufen werden.[cite:50]
- Dringlichkeit, Drohkulissen, Zeitdruck und verdächtige Zahlungsaufforderungen sind starke Warnsignale für Phishing oder Social Engineering.[cite:50]

Die Formulierung im Produkt sollte bewusst defensiv und vertrauenswürdig sein: „Das wirkt verdächtig“, „Ich würde davon eher abraten“, „Öffne die Seite lieber direkt selbst“ oder „Ich kann keine Garantie geben, aber mehrere Hinweise sprechen gegen diese Nachricht“.[cite:50]

## Lokale-first-Strategie
Die lokale Betriebsfähigkeit ist ein zentrales Differenzierungsmerkmal. Das analysierte Repo beschreibt bereits eine local-first Richtung: Lokales Whisper ist heute schon vorgesehen, lokale CLI-Handoffs an Codex, Claude Code und OpenClaw ebenfalls, während die screenshot-aware Hauptkonversation und TTS aktuell noch Cloud-Dienste nutzen.[cite:43][cite:44]

Das Zielprodukt sollte daraus eine klare Architekturstrategie machen:

- Sprache lokal, wenn möglich.
- Risikoregeln lokal.
- Screen-Parsing und Basislogik lokal.
- LLM-Orchestrierung modellagnostisch, bevorzugt lokal, optional hybrid.
- Cloud nur dann, wenn ein Nutzer das bewusst aktiviert oder für bessere Qualität benötigt.[cite:42][cite:44]

Je leistungsfähiger lokale Vision- und Sprachmodelle werden, desto realistischer wird ein vollständig lokaler Modus. Das stärkt Datenschutz, Vertrauen, Offline-Fähigkeit und Marktattraktivität erheblich.[cite:44]

## Leitprinzipien
Die Produktentwicklung sollte auf wenigen, klaren Regeln basieren:

- Der Nutzer bleibt in Kontrolle.
- Externe Aktionen passieren nicht still im Hintergrund.
- Riskante Handlungen benötigen explizite Freigabe.
- Das System erklärt Entscheidungen in Alltagssprache.
- Lokales Processing wird bevorzugt.
- Transparenz ist Teil des Produkts, nicht nur der Datenschutzseite.[cite:42][cite:44]

Diese Prinzipien passen gut zur im Repo hinterlegten Persönlichkeits- und Verhaltenslogik, die bereits Vorsicht bei externen Aktionen, Respekt vor Nutzerdaten und eine kompetente, nicht übergriffige Hilfestellung betont.[cite:42]

## Produktmodule

### 1. Sicherheitsbegleiter
Dieses Modul soll verdächtige E-Mails, Popups, Login-Aufforderungen, Webseiten, Download-Situationen und Dateianhänge kontextbezogen einordnen. Nutzer können fragen: „Ist das echt?“, „Kann ich das öffnen?“ oder „Soll ich hier klicken?“[cite:50]

Der Assistent bewertet sichtbare Signale, erklärt typische Betrugsmuster in einfacher Sprache und empfiehlt sichere Alternativen. Das Ziel ist keine forensische Malware-Analyse, sondern klare Alltagsorientierung.[cite:50]

### 2. Bildschirmhilfe
Dieses Modul unterstützt bei Navigation und Verständnis sichtbarer Software-Oberflächen. Das Repo implementiert bereits eine Struktur, in der auf sichtbare UI-Elemente per Koordinaten-Tag verwiesen werden kann, um Controls, Tabs oder Fenster auf dem Bildschirm zu markieren.[cite:42]

Im Zielprodukt würde daraus eine Nutzererfahrung entstehen, bei der der Assistent sagt, wo geklickt werden soll, was ein Element bedeutet und welche Schritte als Nächstes sinnvoll sind. Das ist besonders wertvoll bei Windows-Einstellungen, Browser-Problemen, Formularen, unbekannter Software und Support-Situationen.[cite:42]

### 3. Sprachassistenz
Das Repo enthält Push-to-Talk, Mikrofonaufzeichnung, Speech-to-Text und Text-to-Speech. Diese Basis sollte übernommen und produktiv weiterentwickelt werden.[cite:42][cite:43]

Nicht technikaffine Nutzer profitieren stark von einer Sprachschnittstelle, weil sie ihre Unsicherheit direkt formulieren können. Wichtig ist, dass Antworten kurz, natürlich und gut hörbar sind, während detailliertere Erklärungen zusätzlich im Overlay oder Hauptfenster sichtbar bleiben.[cite:42]

### 4. Recherche- und Denkmodus
Wenn OpenClaw oder ein anderes Modell-Backend verbunden ist, kann das System Aufgaben wie Recherche, Brainstorming, Zusammenfassungen, Vergleiche oder einfache Arbeitsunterstützung übernehmen. Das Repo zeigt bereits One-shot-Handoffs an OpenClaw und andere lokale Tools.[cite:42][cite:44]

Für das Zielprodukt muss diese Funktion so abstrahiert werden, dass Nutzer nicht mit Toolnamen, Sessions oder technischen Workflows konfrontiert werden. Sie erleben stattdessen einen verständlichen „Ich prüfe das kurz für dich“-Modus.[cite:47]

## Nutzerflüsse

### Sicherheitsfall: Verdächtige E-Mail
1. Der Nutzer öffnet eine E-Mail und fragt per Sprache oder Text, ob sie vertrauenswürdig ist.[cite:50]
2. Das System analysiert den sichtbaren Inhalt, signaltypische Formulierungen und den Kontext des Screens.[cite:42]
3. Es erklärt die Warnzeichen in verständlicher Sprache, etwa Druck, ungewöhnliche Aufforderung oder Link-Risiko.[cite:50]
4. Es empfiehlt eine sichere Alternative, zum Beispiel den Dienst manuell im Browser zu öffnen.[cite:50]
5. Der Nutzer entscheidet selbst, wie er weiter verfährt.[cite:50]

### Bedienfall: Hilfe in Software oder Browser
1. Der Nutzer sagt: „Ich komme hier nicht weiter“ oder „Wo muss ich klicken?“[cite:50]
2. Das System analysiert den aktiven Bildschirm und priorisiert den Cursor-Screen, wie es im Repo bereits vorgesehen ist.[cite:42]
3. Es erklärt die sichtbaren Optionen und markiert bei Bedarf einen Zielbereich über die Pointer-Logik.[cite:42]
4. Es begleitet den Nutzer schrittweise zum Ziel, statt einen großen Textblock zu liefern.[cite:42]

### Delegationsfall: Recherche oder Brainstorming
1. Der Nutzer bittet um Recherche, Vergleich oder Ideenfindung.[cite:50]
2. Der Client entscheidet anhand von Regeln, ob eine lokale Antwort ausreicht oder ob ein Agent-/LLM-Handoff sinnvoll ist.[cite:47]
3. Die Ergebnisse werden in verständlicher Sprache zurückgegeben und auf Wunsch vertieft.[cite:42]

## MVP-Umfang
Der erste marktfähige MVP sollte eng und nutzerorientiert bleiben. Ziel ist nicht maximale Funktionsfülle, sondern hohe Zuverlässigkeit in wenigen Schlüsselsituationen.[cite:50]

**Empfohlener MVP:**

- Windows-Overlay nahe Cursor oder Bildschirmrand.[cite:44]
- Texteingabe und Push-to-Talk.[cite:43]
- Analyse des sichtbaren Bildschirms.[cite:42]
- Sicherheitsbewertung für E-Mails, Links, Popups und Anhänge.[cite:50]
- Navigation zu sichtbaren UI-Elementen.[cite:42]
- Modellagnostische LLM-Anbindung mit OpenClaw als bevorzugter Orchestrierungsoption.[cite:47]
- Transparenzansicht: lokal vs. cloud, aktive Analyse, Freigabe vor externen Aktionen.[cite:44]

## Erweiterungen nach MVP
Nach erfolgreichem MVP bieten sich folgende Ausbaustufen an:

- Browser-Erweiterung für URL-, Domain- und Seitenkontext.
- Lokale Vorprüfung von Dateien und Anhängen.
- Familien- oder Seniorenmodus mit vereinfachten Erklärungen.
- Verlaufsfunktion: Was wurde empfohlen, warum und wann.
- Lernmodus: Das System merkt sich wiederkehrende Unsicherheiten und erklärt proaktiv verständlicher.
- Teilautomatisierung ungefährlicher Routineaufgaben mit expliziter Bestätigung.[cite:50]

## Technische Architektur
Eine vollständige Produktarchitektur kann in fünf Schichten gegliedert werden:

### 1. Client Layer
Windows-App mit Overlay, Tray, Texteingabe, Push-to-Talk, Zustandsanzeige und Einstellungen. Das Repo zeigt bereits eine kompakte WinForms-Implementierung mit diesen Grundelementen.[cite:42][cite:44]

### 2. Context Layer
Erfassung von Multi-Screen-Screenshots, Cursorbezug, Fenstersituation und eventuell Browsermetadaten. Das Repo priorisiert bereits den Bildschirm, auf dem sich der Cursor befindet, skaliert Aufnahmen und versieht sie mit Labels.[cite:42]

### 3. Safety Layer
Regelwerk für typische Risikosignale, heuristische Vorbewertung und Eskalationslogik. Diese Schicht sollte bewusst deterministisch und nachvollziehbar bleiben, um Vertrauen zu schaffen.[cite:50]

### 4. Reasoning Layer
LLM- oder Agent-Orchestrierung, lokal oder hybrid. OpenClaw bietet sich als starke Integrationsoption an, da bereits lokale Erfahrung damit vorhanden ist und das Repo ähnliche Handoff-Muster kennt.[cite:47][cite:42]

### 5. Action Layer
Antworten, Navigation, strukturierte Empfehlungen, Recherche, Zusammenfassungen und optional freigegebene lokale Aktionen. Im Repo sind One-shot-Handoffs mit Logging bereits implementiert, was ein gutes Ausgangsmuster für kontrollierte Delegation ist.[cite:42][cite:43]

## Technologierichtung
Kurzfristig ist eine hybride Architektur realistisch. Das Repo selbst zeigt, dass bestimmte Teile heute noch mit Cloud-Diensten einfacher oder leistungsfähiger umzusetzen sind, während Speech-to-Text und Tool-Handoffs bereits lokal gedacht sind.[cite:43][cite:44]

Mittelfristig sollte das Produkt auf einen vollständig lokalen Betriebsmodus hinarbeiten. Das ist nicht nur ein Datenschutzvorteil, sondern auch ein Produktmerkmal: Menschen, die sich vor Ausspähung, Datenabfluss oder ständiger Cloud-Abhängigkeit sorgen, gewinnen dadurch Vertrauen in die Software.[cite:44]

## UX-Anforderungen
Vertrauen muss in der Benutzeroberfläche sichtbar werden. Der Nutzer sollte jederzeit erkennen können:

- ob das System gerade zuhört,
- ob der Bildschirm analysiert wird,
- ob die Anfrage lokal oder cloudbasiert läuft,
- und ob eine Aktion nur empfohlen oder tatsächlich ausgelöst werden soll.[cite:42][cite:44]

Die Zustandslogik aus dem Repo mit Idle, Listening, Transcribing, Thinking und Speaking kann dafür als Grundlage dienen. Im Zielprodukt sollte sie stärker visualisiert und für Privatsphäre verständlich formuliert werden.[cite:42]

## Wettbewerbsvorteil
Das eigentliche Alleinstellungsmerkmal ist die Kombination aus mehreren Eigenschaften:

- deutschsprachige Alltagshilfe,
- Sicherheitsfokus,
- Bildschirmverständnis,
- verständliche Sprache statt Technikjargon,
- lokale oder lokale-first Betriebsfähigkeit,
- Assistenz für reale Windows-Situationen statt abstrakter Chatfenster.[cite:44][cite:50]

Diese Kombination ist deutlich zugänglicher als klassische AI-Agent-Produkte und zugleich vertrauenswürdiger als rein cloudzentrierte Assistenten.

## Geschäftsmodell
Mehrere Modelle sind plausibel:

| Modell | Beschreibung | Bewertung |
|---|---|---|
| B2C Freemium | Basisfunktionen gratis, Pro für Sprache, History, stärkere Modelle, Recherche | guter Einstieg für Reichweite [cite:50] |
| Familienabo | Mehrere Nutzerprofile, Schutz- und Hilfefunktionen für Angehörige | starkes Vertrauensprodukt [cite:50] |
| Solo-/SMB-Abo | Hilfe und Sicherheitsbegleitung für kleine Unternehmen ohne eigene IT | hoher Praxisnutzen [cite:48][cite:50] |
| White-Label | Anpassbar für IT-Dienstleister, Bildung, Banken, Versicherer | später interessant [cite:50] |

Ein früher Markttest sollte wahrscheinlich mit B2C oder Family/Solo-Business beginnen, weil dort Nutzen, Story und Differenzierung am leichtesten erklärbar sind.[cite:48][cite:50]

## Roadmap

### Phase 1: Validierung
- Klickbarer Prototyp und UX-Tests.
- MVP mit Sicherheitsbegleiter und Bildschirmhilfe.
- OpenClaw-/LLM-Anbindung im Hybridmodus.
- Fokus auf deutschsprachige Windows-Nutzer.[cite:47][cite:50]

### Phase 2: Produktreife
- Browser-Integration.
- Bessere Kontextsignale.
- Dateirisiko-Checks.
- Nutzerhistorie und Lernmodus.
- Installations- und Onboarding-Optimierung.[cite:50]

### Phase 3: Vertrauensplattform
- Voll lokaler Betriebsmodus für Kernfunktionen.
- Familien- und Teamfunktionen.
- Partner- und White-Label-Modelle.
- Erweiterte Automationsfunktionen mit Freigabemechanik.[cite:44][cite:50]

## Risiken und Gegenmaßnahmen

| Risiko | Beschreibung | Gegenmaßnahme |
|---|---|---|
| Übergriffigkeitsgefühl | Bildschirm- und Sprachzugriff kann Nutzer abschrecken | maximale Transparenz, klare Zustände, lokale-first Kommunikation [cite:42][cite:44] |
| Falsches Sicherheitsvertrauen | Nutzer verlassen sich zu stark auf Hinweise | defensive Sprache, keine Garantien, Nutzerentscheidung bleibt zentral [cite:50] |
| Technische Komplexität | Vision, Sprache, UI und Agentik zusammen sind komplex | MVP eng halten, Schichtenarchitektur, modulare Backends [cite:42][cite:47] |
| Cloud-Abhängigkeit | Datenschutz- und Kostenfragen bei externen Diensten | lokale Roadmap, optionale Hybridmodi, modellagnostische Architektur [cite:44] |
| UI-Überforderung | Zu viele Optionen verwirren Zielgruppe | klare Defaults, einfache Sprache, progressive Offenlegung [cite:50] |

## Zusammenfassung
PC-Begleiter ist als vollständiges Produkt deutlich mehr als ein adaptiertes Repo. Das Repo liefert wertvolle technische Denkanstöße für Overlay, Sprache, Screen-Awareness und lokale Tool-Handoffs, aber das eigentliche Produkt entsteht erst durch eine konsequente Fokussierung auf Vertrauen, Sicherheit, Verständlichkeit und lokale Betriebsfähigkeit.[cite:42][cite:44]

Die Idee ist besonders stark, weil sie ein reales Problem adressiert: Menschen fühlen sich am PC oft unsicher, allein gelassen und bei riskanten Situationen überfordert. Ein lokaler oder lokale-first Assistenzbegleiter, der den Bildschirm versteht, Gefahren einordnet und konkrete Hilfe gibt, hat daher eine glaubwürdige Produktstory und ein klares Differenzierungsmerkmal im Markt.[cite:50][cite:47]
