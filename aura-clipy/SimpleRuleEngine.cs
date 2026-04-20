using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuraClipy
{
    /// <summary>
    /// Simple Rule Engine für Sicherheitsbewertungen im PC-Begleiter
    /// Analysiert Screenshots/Kontext auf Risikomuster
    /// </summary>
    public class SimpleRuleEngine
    {
        private readonly List<SecurityRule> _rules;
        private readonly Dictionary<string, bool> _enabledRules;
        private readonly Dictionary<string, int> _customRiskScores;
        
        /// <summary>
        /// Erstellt Rule Engine mit Standardregeln
        /// </summary>
        public SimpleRuleEngine()
        {
            _rules = new List<SecurityRule>();
            _enabledRules = new Dictionary<string, bool>();
            _customRiskScores = new Dictionary<string, int>();
            InitializeDefaultRules();
        }
        
        /// <summary>
        /// Gibt Liste aller verfügbaren Regeln zurück
        /// </summary>
        public IReadOnlyList<SecurityRule> GetAllRules() => _rules.AsReadOnly();
        
        /// <summary>
        /// Aktiviert oder deaktiviert eine Regel
        /// </summary>
        public void SetRuleEnabled(string ruleId, bool enabled)
        {
            _enabledRules[ruleId] = enabled;
        }
        
        /// <summary>
        /// Setzt benutzerdefinierten RiskScore für eine Regel
        /// </summary>
        public void SetCustomRiskScore(string ruleId, int score)
        {
            _customRiskScores[ruleId] = Math.Clamp(score, 0, 10);
        }
        
        /// <summary>
        /// Setzt alle Regeln einer Kategorie aktiv/inaktiv
        /// </summary>
        public void SetCategoryEnabled(RiskCategory category, bool enabled)
        {
            foreach (var rule in _rules.Where(r => r.Category == category))
            {
                _enabledRules[rule.Id] = enabled;
            }
        }
        
        /// <summary>
        /// Setzt alle Regeln zurück auf Standard
        /// </summary>
        public void ResetToDefaults()
        {
            _enabledRules.Clear();
            _customRiskScores.Clear();
        }
        
        private void InitializeDefaultRules()
        {
            _rules = new List<SecurityRule>
            {
                // E-Mail Phishing Patterns
                new SecurityRule
                {
                    Id = "PHISH_EMAIL_URGENCY",
                    Name = "E-Mail mit Dringlichkeits-Druck",
                    Description = "E-Mails die künstlichen Zeitdruck erzeugen",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "sofort", "dringend", "jetzt", "unverzüglich", "letzte Chance", "Frist", "abgelaufen", "Account gesperrt" },
                    RiskScore = 7,
                    Category = RiskCategory.Phishing
                },
                new SecurityRule
                {
                    Id = "PHISH_EMAIL_THREAT",
                    Name = "E-Mail mit Drohkulisse",
                    Description = "E-Mails die mit Konsequenzen drohen",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "gesperrt", "gelöscht", "strafrechtlich", "Anzeige", "Rechnung", "Zahlung", "Mahnung" },
                    RiskScore = 8,
                    Category = RiskCategory.Phishing
                },
                new SecurityRule
                {
                    Id = "PHISH_EMAIL_PERSONAL",
                    Name = "E-Mail mit persönlicher Ansprache",
                    Description = "E-Mails die vertrauenswürdig wirken sollen",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Sehr geehrte", "Liebe", "Hallo", "Herr", "Frau", "Kunde", "Nutzer" },
                    RiskScore = 3,
                    Category = RiskCategory.Phishing
                },
                new SecurityRule
                {
                    Id = "PHISH_EMAIL_LINK",
                    Name = "E-Mail mit verdächtigen Links",
                    Description = "E-Mails die zu unbekannten/verdächtigen URLs verlinken",
                    PatternType = PatternType.Regex,
                    PatternData = new[] { @"https?://[^\s/]*\.(xyz|top|club|gq|ml|tk|cf|ga)", @"bit\.ly|tinyurl\.com|shorte\.st" },
                    RiskScore = 9,
                    Category = RiskCategory.Phishing
                },
                
                // Popup/Dialog Patterns
                new SecurityRule
                {
                    Id = "POPUP_FAKE_WARNING",
                    Name = "Fake-Systemwarnung",
                    Description = "Popup das wie System-Dialog aussieht aber nicht ist",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Virus gefunden", "Malware erkannt", "System gefährdet", "Computer infiziert", "sofort handeln", "Anrufnummer", "Techniker" },
                    RiskScore = 9,
                    Category = RiskCategory.Malware
                },
                new SecurityRule
                {
                    Id = "POPUP_UNEXPECTED",
                    Name = "Unerwarteter Dialog",
                    Description = "Popup das ohne Nutzeraktion erscheint",
                    PatternType = PatternType.Contextual,
                    PatternData = new[] { "unexpected", "unsolicited" },
                    RiskScore = 6,
                    Category = RiskCategory.Suspicious
                },
                
                // Browser/Web Patterns
                new SecurityRule
                {
                    Id = "WEB_FAKE_LOGIN",
                    Name = "Fake-Login-Seite",
                    Description = "Webseite die Login-Daten abfragt aber nicht vertrauenswürdig ist",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Passwort", "Login", "Anmelden", "Benutzername", "Konto", "Zugangsdaten", "ändern", "aktualisieren" },
                    RiskScore = 8,
                    Category = RiskCategory.CredentialTheft
                },
                new SecurityRule
                {
                    Id = "WEB_SSL_MISSING",
                    Name = "Fehlende SSL-Verschlüsselung",
                    Description = "Webseite ohne HTTPS (unsichere Verbindung)",
                    PatternType = PatternType.Contextual,
                    PatternData = new[] { "http://" },
                    RiskScore = 5,
                    Category = RiskCategory.InsecureConnection
                },
                
                // Social Engineering Patterns
                new SecurityRule
                {
                    Id = "SOCIAL_IMPERSONATION",
                    Name = "Imitation von bekannten Marken/Personen",
                    Description = "Inhalte die bekannte Marken oder Personen imitieren",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Microsoft", "Apple", "Google", "Amazon", "PayPal", "Netflix", "Sparkasse", "Deutsche Bank", "Telekom" },
                    RiskScore = 6,
                    Category = RiskCategory.SocialEngineering
                },
                new SecurityRule
                {
                    Id = "SOCIAL_FAKE_PRIZE",
                    Name = "Fake Gewinnspiel/Preis",
                    Description = "Meldungen über nicht existierende Gewinne oder Preise",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Sie haben gewonnen", "Gratulation", "Jackpot", "Preis gewonnen", "Kostenlos", "absolut kostenlos", "keine Kosten" },
                    RiskScore = 8,
                    Category = RiskCategory.SocialEngineering
                },
                new SecurityRule
                {
                    Id = "SOCIAL_FAKE_SUPPORT",
                    Name = "Fake Tech-Support",
                    Description = "Angebote für technischen Support die echt wirken sollen",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Tech-Support", "Microsoft Support", "Windows Support", "Apple Care", "technischer Kundendienst", "0800", "0180", "Rufnummer" },
                    RiskScore = 8,
                    Category = RiskCategory.SocialEngineering
                },
                
                // Financial Fraud Patterns
                new SecurityRule
                {
                    Id = "FINANCIAL_PAYMENT_URGENT",
                    Name = "Dringende Zahlungsaufforderung",
                    Description = "Aufforderungen zu sofortiger Zahlung mit Drohkulisse",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "sofort überweisen", "Zahlung jetzt", "letzte Frist", "Mahnbescheid", "Inkasso", "Gerichtsvollzieher" },
                    RiskScore = 9,
                    Category = RiskCategory.FinancialFraud
                },
                new SecurityRule
                {
                    Id = "FINANCIAL_BANK_FAKE",
                    Name = "Fake Bank-Nachricht",
                    Description = "E-Mails die vorgeben von einer Bank zu sein",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Konto gesperrt", "Konto verifizieren", "Online-Banking", "PIN ändern", "TAN eingeben", "Kreditkarte bestätigen" },
                    RiskScore = 9,
                    Category = RiskCategory.FinancialFraud
                },
                new SecurityRule
                {
                    Id = "FINANCIAL_CRYPTOCURRENCY",
                    Name = "Kryptowährung/Investment Betrug",
                    Description = "Angebote für Kryptowährungen oder Investitionen mit unrealistischen Versprechen",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Bitcoin", "Krypto", "investieren", "100% Rendite", "verdoppeln", "garantierte Gewinne", "ETH", "BTC", "Wallet" },
                    RiskScore = 8,
                    Category = RiskCategory.FinancialFraud
                },
                
                // Download/File Patterns
                new SecurityRule
                {
                    Id = "DOWNLOAD_SUSPICIOUS_EXT",
                    Name = "Verdächtige Dateiendung",
                    Description = "Download-Links für potenziell gefährliche Dateitypen",
                    PatternType = PatternType.Regex,
                    PatternData = new[] { @"\.(exe|bat|cmd|ps1|vbs|js|jse|wsf|wsh|msi|scr|pif)$", @"\.(zip|rar|7z).*\.(exe|bat|cmd)" },
                    RiskScore = 8,
                    Category = RiskCategory.Malware
                },
                new SecurityRule
                {
                    Id = "DOWNLOAD_CRACK_PIRACY",
                    Name = "Crack/Piracy Download",
                    Description = "Links zu Raubkopien oder Cracks",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "crack", "keygen", "patch", "pirated", "raubkopie", "serial key", "license key", "vollversion kostenlos" },
                    RiskScore = 9,
                    Category = RiskCategory.Malware
                },
                
                // Credential Theft Patterns
                new SecurityRule
                {
                    Id = "CREDENTIAL_PASSWORD_REQUEST",
                    Name = "Passwort-Anfrage",
                    Description = "Aufforderung Passwörter oder Zugangsdaten einzugeben",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "Passwort eingeben", "Passwort ändern", "Zugangsdaten", "login", "sign in", "anmelden" },
                    RiskScore = 7,
                    Category = RiskCategory.CredentialTheft
                },
                new SecurityRule
                {
                    Id = "CREDENTIAL_2FA_FAKE",
                    Name = "Fake 2-Faktor-Authentifizierung",
                    Description = "Aufforderung 2FA Codes einzugeben",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "2FA", "zweifaktor", "Authentifizierungscode", "Bestätigungscode", "SMS Code", "Verifizierungscode" },
                    RiskScore = 9,
                    Category = RiskCategory.CredentialTheft
                },
                
                // Additional Urgency Patterns
                new SecurityRule
                {
                    Id = "URGENCY_FAKE_DEADLINE",
                    Name = "Falsche Frist/Termin",
                    Description = "Künstliche Zeitlimit-Situationen",
                    PatternType = PatternType.TextContains,
                    PatternData = new[] { "24 Stunden", "48 Stunden", " heute noch", "Frist endet", "Angebot läuft ab", "nur noch begrenzt" },
                    RiskScore = 6,
                    Category = RiskCategory.Phishing
                }
            };
        }
        
        /// <summary>
        /// Analysiert Text auf Sicherheitsrisiken
        /// </summary>
        public SecurityAnalysisResult AnalyzeText(string text, string context = "unknown")
        {
            var result = new SecurityAnalysisResult
            {
                Context = context,
                DetectedRules = new List<DetectedRule>(),
                OverallRiskScore = 0
            };
            
            if (string.IsNullOrWhiteSpace(text))
                return result;
            
            // Normalize text for analysis
            string normalizedText = text.ToLower();
            
            foreach (var rule in _rules)
            {
                // Check if rule is enabled (default true if not in dictionary)
                if (_enabledRules.TryGetValue(rule.Id, out bool enabled) && !enabled)
                    continue;
                
                bool matches = false;
                
                switch (rule.PatternType)
                {
                    case PatternType.TextContains:
                        foreach (var pattern in rule.PatternData)
                        {
                            if (normalizedText.Contains(pattern.ToLower()))
                            {
                                matches = true;
                                break;
                            }
                        }
                        break;
                        
                    case PatternType.Regex:
                        foreach (var pattern in rule.PatternData)
                        {
                            if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
                            {
                                matches = true;
                                break;
                            }
                        }
                        break;
                        
                    case PatternType.Contextual:
                        // Contextual patterns require additional context analysis
                        // For now, we'll treat them as text contains
                        foreach (var pattern in rule.PatternData)
                        {
                            if (normalizedText.Contains(pattern.ToLower()))
                            {
                                matches = true;
                                break;
                            }
                        }
                        break;
                }
                
                if (matches)
                {
                    // Get effective risk score (custom or default)
                    int effectiveScore = _customRiskScores.TryGetValue(rule.Id, out int customScore) 
                        ? customScore 
                        : rule.RiskScore;
                    
                    result.DetectedRules.Add(new DetectedRule
                    {
                        Rule = rule,
                        MatchedText = GetMatchedSnippet(text, rule),
                        Confidence = 0.8f // Default confidence
                    });
                    
                    // Accumulate risk score (capped at 10)
                    result.OverallRiskScore = Math.Min(10, result.OverallRiskScore + effectiveScore);
                }
            }
            
            // Determine risk level based on score
            result.RiskLevel = DetermineRiskLevel(result.OverallRiskScore);
            
            return result;
        }
        
        /// <summary>
        /// Analysiert Screenshot-Kontext (kann später mit Vision-Modell erweitert werden)
        /// </summary>
        public SecurityAnalysisResult AnalyzeContext(ScreenshotContext context)
        {
            var result = new SecurityAnalysisResult
            {
                Context = context.SourceType.ToString(),
                DetectedRules = new List<DetectedRule>(),
                OverallRiskScore = 0
            };
            
            // For now, combine text analysis from context
            if (!string.IsNullOrWhiteSpace(context.DetectedText))
            {
                var textResult = AnalyzeText(context.DetectedText, context.SourceType.ToString());
                result.DetectedRules.AddRange(textResult.DetectedRules);
                result.OverallRiskScore = textResult.OverallRiskScore;
                result.RiskLevel = textResult.RiskLevel;
            }
            
            // Additional context-based rules
            if (context.SourceType == ContextSourceType.EmailClient)
            {
                // Email-specific additional risk
                result.OverallRiskScore = Math.Min(10, result.OverallRiskScore + 1);
            }
            else if (context.SourceType == ContextSourceType.Browser)
            {
                // Browser-specific additional rules
                if (context.Url?.Contains("http://") == true && !context.Url.Contains("https://"))
                {
                    result.DetectedRules.Add(new DetectedRule
                    {
                        Rule = _rules.First(r => r.Id == "WEB_SSL_MISSING"),
                        MatchedText = "HTTP ohne SSL erkannt",
                        Confidence = 0.9f
                    });
                    result.OverallRiskScore = Math.Min(10, result.OverallRiskScore + 5);
                }
            }
            
            result.RiskLevel = DetermineRiskLevel(result.OverallRiskScore);
            return result;
        }
        
        private string GetMatchedSnippet(string text, SecurityRule rule)
        {
            // Extract a small snippet around matched pattern
            // Simplified implementation
            foreach (var pattern in rule.PatternData)
            {
                int index = text.ToLower().IndexOf(pattern.ToLower());
                if (index >= 0)
                {
                    int start = Math.Max(0, index - 20);
                    int length = Math.Min(text.Length - start, pattern.Length + 40);
                    return text.Substring(start, length) + "...";
                }
            }
            return "Pattern matched";
        }
        
        private RiskLevel DetermineRiskLevel(int score)
        {
            if (score >= 8) return RiskLevel.High;
            if (score >= 5) return RiskLevel.Medium;
            if (score >= 2) return RiskLevel.Low;
            return RiskLevel.None;
        }
        
        /// <summary>
        /// Gibt Sicherheitsempfehlung basierend auf Analyse zurück
        /// </summary>
        public SafetyRecommendation GetRecommendation(SecurityAnalysisResult analysis)
        {
            var recommendation = new SafetyRecommendation
            {
                RiskLevel = analysis.RiskLevel,
                OverallScore = analysis.OverallRiskScore,
                DetectedThreats = analysis.DetectedRules.Select(r => r.Rule.Name).ToList(),
                Timestamp = DateTime.Now
            };
            
            // Generate recommendation text based on risk level
            recommendation.RecommendationText = GenerateRecommendationText(analysis);
            recommendation.SafeAlternatives = GenerateSafeAlternatives(analysis);
            
            return recommendation;
        }
        
        private string GenerateRecommendationText(SecurityAnalysisResult analysis)
        {
            if (analysis.RiskLevel == RiskLevel.None)
                return "✅ Diese Situation scheint sicher. Keine bekannten Risikomuster erkannt.";
            
            if (analysis.RiskLevel == RiskLevel.Low)
                return "⚠️ Leicht erhöhtes Risiko erkannt. Einige verdächtige Muster gefunden, aber wahrscheinlich harmlos.";
            
            if (analysis.RiskLevel == RiskLevel.Medium)
                return "⚠️⚠️ Mittleres Risiko erkannt. Enthält mehrere verdächtige Muster. Vorsicht empfohlen.";
            
            // High risk
            var threats = string.Join(", ", analysis.DetectedRules.Select(r => r.Rule.Name));
            return $"🚨 HOHE RISIKO erkannt! Enthält: {threats}. Empfehle NICHT zu interagieren. Sichere Alternative vorschlagen.";
        }
        
        private List<string> GenerateSafeAlternatives(SecurityAnalysisResult analysis)
        {
            var alternatives = new List<string>();
            
            if (analysis.RiskLevel >= RiskLevel.Medium)
            {
                alternatives.Add("❌ NICHT auf Links klicken");
                alternatives.Add("❌ NICHT Anhänge öffnen");
                alternatives.Add("❌ NICHT persönliche Daten eingeben");
                
                // Check detected categories for specific advice
                bool hasFinancialFraud = analysis.DetectedRules.Any(r => r.Rule.Category == RiskCategory.FinancialFraud);
                bool hasSocialEngineering = analysis.DetectedRules.Any(r => r.Rule.Category == RiskCategory.SocialEngineering);
                bool hasCredentialTheft = analysis.DetectedRules.Any(r => r.Rule.Category == RiskCategory.CredentialTheft);
                bool hasMalware = analysis.DetectedRules.Any(r => r.Rule.Category == RiskCategory.Malware);
                
                if (analysis.Context.Contains("email", StringComparison.OrdinalIgnoreCase))
                {
                    alternatives.Add("✅ Absender direkt überprüfen");
                    alternatives.Add("✅ Offizielle Website manuell aufrufen");
                    alternatives.Add("✅ Bei Unsicherheit: E-Mail löschen");
                }
                
                if (analysis.Context.Contains("browser", StringComparison.OrdinalIgnoreCase))
                {
                    alternatives.Add("✅ Browser schließen und neu starten");
                    alternatives.Add("✅ Bekannte, vertrauenswürdige Seite direkt aufrufen");
                    alternatives.Add("✅ Suchmaschine für legitime Seite nutzen");
                }
                
                if (hasFinancialFraud)
                {
                    alternatives.Add("✅ Niemals Geld überweisen ohne unabhängige Verifizierung");
                    alternatives.Add("✅ Bank direkt kontaktieren (nicht über E-Mail)");
                    alternatives.Add("✅ Niemals TANs oder Passwörter eingeben");
                }
                
                if (hasSocialEngineering)
                {
                    alternatives.Add("✅ Misstraue unerwarteten Gewinnmitteilungen");
                    alternatives.Add("✅ Keine persönlichen Daten am Telefon geben");
                    alternatives.Add("✅ Seriöse Unternehmen fragen nicht so");
                }
                
                if (hasCredentialTheft)
                {
                    alternatives.Add("✅ Niemals Passwörter eingeben nach E-Mail-Link");
                    alternatives.Add("✅ Direkt auf echter Website einloggen");
                    alternatives.Add("✅ 2FA nur auf offiziellen Seiten");
                }
                
                if (hasMalware)
                {
                    alternatives.Add("✅ Keine Software von unbekannten Quellen");
                    alternatives.Add("✅ Windows Defender / Antivirus aktuell halten");
                    alternatives.Add("✅ Bei Verdacht: System-Scan durchführen");
                }
            }
            
            if (alternatives.Count == 0)
            {
                alternatives.Add("✅ Normal weiterarbeiten");
                alternatives.Add("✅ Bei Fragen: PC-Begleiter fragen");
            }
            
            return alternatives;
        }
    }
    
    // Supporting Types
    public enum PatternType { TextContains, Regex, Contextual }
    public enum RiskCategory { Phishing, Malware, Suspicious, CredentialTheft, InsecureConnection, SocialEngineering, FinancialFraud }
    public enum RiskLevel { None, Low, Medium, High }
    public enum ContextSourceType { Unknown, EmailClient, Browser, Popup, SystemDialog, FileExplorer }
    
    public class SecurityRule
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PatternType PatternType { get; set; }
        public string[] PatternData { get; set; }
        public int RiskScore { get; set; } // 0-10
        public RiskCategory Category { get; set; }
    }
    
    public class DetectedRule
    {
        public SecurityRule Rule { get; set; }
        public string MatchedText { get; set; }
        public float Confidence { get; set; } // 0.0 - 1.0
    }
    
    public class SecurityAnalysisResult
    {
        public string Context { get; set; }
        public List<DetectedRule> DetectedRules { get; set; }
        public int OverallRiskScore { get; set; } // 0-10
        public RiskLevel RiskLevel { get; set; }
    }
    
    public class ScreenshotContext
    {
        public ContextSourceType SourceType { get; set; }
        public string DetectedText { get; set; }
        public string Url { get; set; }
        public Bitmap Screenshot { get; set; }
        public DateTime CaptureTime { get; set; }
    }
    
    public class SafetyRecommendation
    {
        public RiskLevel RiskLevel { get; set; }
        public int OverallScore { get; set; }
        public List<string> DetectedThreats { get; set; }
        public string RecommendationText { get; set; }
        public List<string> SafeAlternatives { get; set; }
        public DateTime Timestamp { get; set; }
    }
}