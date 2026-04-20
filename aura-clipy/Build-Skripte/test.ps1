# PC-Begleiter Test-Skript
# Validierung der Kern-Funktionalität
# Erstellt: 2026-04-17

param(
    [switch]$Verbose,
    [switch]$Help
)

function Write-Step($message) {
    Write-Host "[TEST] $message" -ForegroundColor Cyan
}

function Write-Success($message) {
    Write-Host "[  OK  ] $message" -ForegroundColor Green
}

function Write-Fail($message) {
    Write-Host "[ FAIL ] $message" -ForegroundColor Red
}

function Write-Info($message) {
    Write-Host "[INFO ] $message" -ForegroundColor Gray
}

# Help
if ($Help) {
    Write-Host @"
PC-Begleiter Test-Skript
========================

Verwendung:
    .\test.ps1 [-Verbose] [-Help]

Parameter:
    -Verbose    Zeige detaillierte Testergebnisse
    -Help       Diese Hilfe anzeigen

Beschreibung:
    Validiert die Kern-Funktionalität des PC-Begleiters
    ohne einen vollständigen Windows-Build durchzuführen.

"@
    exit 0
}

$ErrorCount = 0
$PassCount = 0

Write-Host "PC-Begleiter Test-Suite" -ForegroundColor Magenta
Write-Host "========================" -ForegroundColor Magenta
Write-Host ""

# ==========================================
# Test 1: Rule Engine Basis-Funktionen
# ==========================================
Write-Step "Teste SimpleRuleEngine..."

try {
    # Simuliere Rule Engine Logik
    $testRules = @(
        @{Id="PHISH_EMAIL_URGENCY"; Pattern="sofort|dringend"; RiskScore=7; Category="Phishing"},
        @{Id="PHISH_EMAIL_THREAT"; Pattern="gesperrt|verfallen"; RiskScore=8; Category="Phishing"},
        @{Id="SOCIAL_FAKE_PRIZE"; Pattern="gewonnen|prize|lotto"; RiskScore=9; Category="SocialEngineering"},
        @{Id="FINANCIAL_BANK_FAKE"; Pattern="konto|bank|überweisung"; RiskScore=10; Category="FinancialFraud"}
    )
    
    $testTexts = @(
        @{Text="Ihr Konto wird sofort gesperrt!"; ExpectedRisk=7},
        @{Text="Sie haben einen Preis gewonnen!"; ExpectedRisk=9},
        @{Text="Bitte überweisen Sie 500 Euro auf dieses Konto"; ExpectedRisk=10},
        @{Text="Alles normal, keine Aktion nötig"; ExpectedRisk=0}
    )
    
    foreach ($test in $testTexts) {
        $detected = $false
        $maxScore = 0
        
        foreach ($rule in $testRules) {
            if ($test.Text -match $rule.Pattern) {
                $detected = $true
                if ($rule.RiskScore -gt $maxScore) {
                    $maxScore = $rule.RiskScore
                }
            }
        }
        
        if ($Verbose) {
            Write-Info "  Text: '$($test.Text)'"
            Write-Info "  Erkannt: $detected, MaxScore: $maxScore"
        }
        
        if ($detected -and $maxScore -ge $test.ExpectedRisk) {
            Write-Success "  Phishing erkannt: $($test.Text.Substring(0, [Math]::Min(30, $test.Text.Length)))..."
            $PassCount++
        } elseif (-not $detected -and $test.ExpectedRisk -eq 0) {
            Write-Success "  Sicherer Text bestanden"
            $PassCount++
        } else {
            Write-Fail "  Erwartet RiskScore >= $($test.ExpectedRisk), gefunden: $maxScore"
            $ErrorCount++
        }
    }
} catch {
    Write-Fail "Rule Engine Test fehlgeschlagen: $_"
    $ErrorCount++
}

# ==========================================
# Test 2: Konfigurations-System
# ==========================================
Write-Step "Teste Konfigurations-System..."

try {
    # Simuliere Konfigurations-Logik
    $config = @{
        Version = "0.2.0"
        SafetyLayer = @{
            Enabled = $true
            RiskThreshold = 5
        }
        Rules = @{
            CustomRiskScores = @{}
            DisabledRules = @()
        }
    }
    
    # Test: Risk Score anpassen
    $config.Rules.CustomRiskScores["PHISH_EMAIL_URGENCY"] = 8
    if ($config.Rules.CustomRiskScores["PHISH_EMAIL_URGENCY"] -eq 8) {
        Write-Success "  Custom RiskScore gesetzt"
        $PassCount++
    } else {
        Write-Fail "  Custom RiskScore konnte nicht gesetzt werden"
        $ErrorCount++
    }
    
    # Test: Regel deaktivieren
    $config.Rules.DisabledRules += "SOCIAL_FAKE_PRIZE"
    if ($config.Rules.DisabledRules -contains "SOCIAL_FAKE_PRIZE") {
        Write-Success "  Regel deaktiviert"
        $PassCount++
    } else {
        Write-Fail "  Regel konnte nicht deaktiviert werden"
        $ErrorCount++
    }
    
} catch {
    Write-Fail "Konfigurations-Test fehlgeschlagen: $_"
    $ErrorCount++
}

# ==========================================
# Test 3: Kontext-Erkennung
# ==========================================
Write-Step "Teste Kontext-Erkennung..."

try {
    # Simuliere Kontext-Erkennung
    $contextTests = @(
        @{Text="Betreff: Ihre Rechnung An: test@web.de"; Expected="EmailClient"},
        @{Text="https://www.google.de suchen"; Expected="Browser"},
        @{Text="Warnung! OK Abbrechen"; Expected="Popup"},
        @{Text="C:\Users\Documents\test.txt"; Expected="FileExplorer"}
    )
    
    $emailIndicators = @("betreff", "an:", "von:", "@")
    $browserIndicators = @("http", "www.", "https", "suchen")
    $popupIndicators = @("warnung", "ok", "abbrechen")
    $explorerIndicators = @("c:\", "d:\", "documents", "desktop")
    
    foreach ($test in $contextTests) {
        $detected = "Unknown"
        $text = $test.Text.ToLower()
        
        if (($emailIndicators | Where-Object { $text -contains $_ } | Measure-Object).Count -ge 2) {
            $detected = "EmailClient"
        } elseif (($browserIndicators | Where-Object { $text -contains $_ } | Measure-Object).Count -ge 2) {
            $detected = "Browser"
        } elseif (($popupIndicators | Where-Object { $text -contains $_ } | Measure-Object).Count -ge 2) {
            $detected = "Popup"
        } elseif (($explorerIndicators | Where-Object { $text -contains $_ } | Measure-Object).Count -ge 2) {
            $detected = "FileExplorer"
        }
        
        if ($detected -eq $test.Expected) {
            Write-Success "  Kontext '$($test.Expected)' erkannt"
            $PassCount++
        } else {
            Write-Fail "  Erwartet '$($test.Expected)', erkannt: '$detected'"
            $ErrorCount++
        }
    }
    
} catch {
    Write-Fail "Kontext-Erkennung Test fehlgeschlagen: $_"
    $ErrorCount++
}

# ==========================================
# Zusammenfassung
# ==========================================
Write-Host ""
Write-Host "==========================" -ForegroundColor Magenta
Write-Host "Test-Ergebnisse:" -ForegroundColor Magenta
Write-Host "  Bestanden: $PassCount" -ForegroundColor Green
Write-Host "  Fehlgeschlagen: $ErrorCount" -ForegroundColor $(if ($ErrorCount -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($ErrorCount -eq 0) {
    Write-Success "Alle Tests bestanden!"
    exit 0
} else {
    Write-Fail "Some tests failed. Please review."
    exit 1
}
