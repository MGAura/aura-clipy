**Letzter Check:** 2026-05-03 06:45
**Status:** Repository ist öffentlich, GitHub Actions Workflow-Datei jetzt im Root-Verzeichnis, aber OAuth Token-Beschränkungen (`workflow` Scope fehlt) verhindern automatischen Push. Workflow muss manuell über GitHub UI erstellt werden.
**Nächster Schritt:** Martin muss:
1. GitHub UI öffnen (https://github.com/MGAura/aura-clipy/actions → "New workflow"), Workflow-Inhalt kopieren und aktivieren ODER
2. GitHub CLI Token mit `workflow` Scope aktualisieren ODER
3. Lokalen Windows-Build testen
**Update:** UPDATE_FOR_MARTIN_heartbeat_2026-05-03_0645.md erstellt