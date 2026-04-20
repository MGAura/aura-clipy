# Build-Skripte - PC-Begleiter

Enthält Build- und Test-Skripte für den PC-Begleiter.

## Skripte

### build.cmd
Windows Batch-Skript für einfachen Build.

**Verwendung:**
```cmd
build.cmd
```

### build.ps1
PowerShell Build-Skript mit erweiterten Optionen.

**Verwendung:**
```powershell
# Einfacher Build
.\build.ps1

# Mit Publish
.\build.ps1 -Publish

# Debug Build
.\build.ps1 -Configuration Debug

# Hilfe
.\build.ps1 -Help
```

### test.ps1
PowerShell Test-Skript zur Validierung der Kern-Funktionalität.

**Verwendung:**
```powershell
# Tests ausführen
.\test.ps1

# Detaillierte Ausgabe
.\test.ps1 -Verbose

# Hilfe
.\test.ps1 -Help
```

## Voraussetzungen

- Windows 10/11
- .NET SDK 10.0 oder höher
- PowerShell 5.1 oder höher

## Workflow

1. **Entwicklung** - Code in `Win Assistent/Code/` ändern
2. **Test** - `test.ps1` ausführen
3. **Build** - `build.ps1 -Publish` ausführen
4. **Ausführen** - `..\publish\win-x64\AuraClipy.exe`

## Build-Ausgabe

Der fertige Build liegt in:
```
Win Assistent/
└── publish/
    └── win-x64/
        └── AuraClipy.exe  (Self-contained, ca. 50-100 MB)
```
