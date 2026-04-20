using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace AuraClipy
{
    /// <summary>
    /// OpenClaw Handoff Service für komplexe Sicherheitsfragen
    /// Delegiert an OpenClaw wenn Rule-Engine unsicher ist
    /// </summary>
    public class OpenClawHandoffService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openclawUrl;
        private readonly SimpleRuleEngine _ruleEngine;
        private readonly Action<string, string> _logCallback;
        
        private const int HIGH_CONFIDENCE_THRESHOLD = 8;  // Rule-Engine entscheidet selbst
        private const int MEDIUM_CONFIDENCE_THRESHOLD = 5; // Optional Handoff
        
        public OpenClawHandoffService(
            string openclawUrl, 
            SimpleRuleEngine ruleEngine,
            Action<string, string> logCallback = null)
        {
            _openclawUrl = openclawUrl?.TrimEnd('/') ?? "http://127.0.0.1:3000";
            _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
            _logCallback = logCallback;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
        }
        
        /// <summary>
        /// Prüft ob OpenClaw-Handoff empfohlen ist
        /// </summary>
        public bool ShouldHandoffToOpenClaw(SecurityAnalysisResult result, int maxRiskScore = 5)
        {
            // Handoff bei:
            // 1. Unklarer Risiko-Level (mittleres Risiko mit mehreren Regeln)
            // 2. Hohe Risk-Scores
            // 3. Mehrere konfliktierende Regeln
            // 4. Benutzer explizit gefragt
            
            if (result.OverallRiskScore >= maxRiskScore)
                return true;
            
            if (result.DetectedRules.Count >= 3)
                return true;
            
            // Bei Social Engineering oder Financial Fraud immer fragen
            var sensitiveCategories = new[] { 
                RiskCategory.SocialEngineering, 
                RiskCategory.FinancialFraud,
                RiskCategory.CredentialTheft 
            };
            
            foreach (var rule in result.DetectedRules)
            {
                if (Array.Exists(sensitiveCategories, cat => cat == rule.Rule.Category))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Führt Sicherheitsanalyse durch, mit optionalem OpenClaw Handoff
        /// </summary>
        public async Task<HandoffResult> AnalyzeWithOptionalHandoff(
            string text, 
            byte[] screenshot = null,
            string context = "unknown",
            bool forceHandoff = false)
        {
            _logCallback?.Invoke("INFO", "Starte Analyse mit OpenClaw-Option...");
            
            var result = new HandoffResult
            {
                Timestamp = DateTime.Now,
                Context = context
            };
            
            // 1. Rule-Engine Analyse
            var ruleResult = _ruleEngine.AnalyzeText(text, context);
            result.RuleBasedResult = ruleResult;
            result.RiskScore = ruleResult.OverallRiskScore;
            result.RiskLevel = ruleResult.RiskLevel;
            
            // 2. Prüfe Handoff Notwendigkeit
            if (!forceHandoff && !ShouldHandoffToOpenClaw(ruleResult))
            {
                _logCallback?.Invoke("INFO", "Rule-Engine entscheidet selbstständig");
                result.Recommendation = _ruleEngine.GetRecommendation(ruleResult);
                result.HandoffTriggered = false;
                return result;
            }
            
            // 3. OpenClaw Handoff
            _logCallback?.Invoke("INFO", "Delegiere an OpenClaw...");
            result.HandoffTriggered = true;
            
            var openClawResult = await SendToOpenClaw(text, screenshot, ruleResult, context);
            result.OpenClawResponse = openClawResult;
            result.Recommendation = ConvertOpenClawResponse(openClawResult);
            
            if (openClawResult.Success)
            {
                _logCallback?.Invoke("INFO", "OpenClaw hat entschieden");
            }
            else
            {
                _logCallback?.Invoke("WARN", "OpenClaw nicht erreichbar, nutze Rule-Engine");
                result.Recommendation = _ruleEngine.GetRecommendation(ruleResult);
            }
            
            return result;
        }
        
        /// <summary>
        /// Sendet Analyse-Daten an OpenClaw
        /// </summary>
        private async Task<OpenClawResponse> SendToOpenClaw(
            string text, 
            byte[] screenshot,
            SecurityAnalysisResult ruleResult,
            string context)
        {
            var response = new OpenClawResponse();
            
            try
            {
                var request = new OpenClawRequest
                {
                    Text = text,
                    Context = context,
                    DetectedRules = ruleResult.DetectedRules,
                    RiskScore = ruleResult.OverallRiskScore,
                    RiskLevel = ruleResult.RiskLevel.ToString(),
                    Timestamp = DateTime.Now
                };
                
                // Screenshot optional
                if (screenshot != null && screenshot.Length > 0)
                {
                    request.ScreenshotBase64 = Convert.ToBase64String(screenshot);
                }
                
                // Frage formulieren
                request.Question = BuildOpenClawQuestion(ruleResult, context);
                
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logCallback?.Invoke("INFO", "Sende an OpenClaw...");
                
                var httpResponse = await _httpClient.PostAsync($"{_openclawUrl}/api/analyze", content);
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                
                if (!httpResponse.IsSuccessStatusCode)
                {
                    response.Error = $"HTTP {httpResponse.StatusCode}: {responseContent}";
                    _logCallback?.Invoke("ERROR", response.Error);
                    return response;
                }
                
                var parsedResponse = JsonSerializer.Deserialize<OpenClawApiResponse>(responseContent);
                response.Success = true;
                response.RiskAssessment = parsedResponse?.RiskAssessment ?? "Unknown";
                response.Explanation = parsedResponse?.Explanation ?? "No explanation";
                response.Recommendations = parsedResponse?.Recommendations ?? new List<string>();
                response.ShouldBlock = parsedResponse?.ShouldBlock ?? (ruleResult.RiskLevel == RiskLevel.High);
                
            }
            catch (TaskCanceledException)
            {
                response.Error = "Timeout - OpenClaw antwortet nicht";
                _logCallback?.Invoke("ERROR", response.Error);
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                _logCallback?.Invoke("ERROR", $"OpenClaw Fehler: {ex.Message}");
            }
            
            return response;
        }
        
        /// <summary>
        /// Baut strukturierte Frage für OpenClaw
        /// </summary>
        private string BuildOpenClawQuestion(SecurityAnalysisResult result, string context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("SICHERHEITSANALYSE - Bitte bewerte:");
            sb.AppendLine();
            sb.AppendLine($"Kontext: {context}");
            sb.AppendLine($"Risiko-Score: {result.OverallRiskScore}/10 ({result.RiskLevel})");
            sb.AppendLine();
            
            sb.AppendLine("Erkannte Regeln:");
            foreach (var rule in result.DetectedRules)
            {
                sb.AppendLine($"  - [{rule.Rule.Category}] {rule.Rule.Name} (Confidence: {rule.Confidence:P0})");
                if (!string.IsNullOrEmpty(rule.MatchedSnippet))
                {
                    sb.AppendLine($"    Textausschnitt: \"{rule.MatchedSnippet.Substring(0, Math.Min(100, rule.MatchedSnippet.Length))}\"");
                }
            }
            
            sb.AppendLine();
            sb.AppendLine("Fragen:");
            sb.AppendLine("1. Ist dies ein echtes Sicherheitsrisiko oder ein False Positive?");
            sb.AppendLine("2. Soll der Benutzer blockiert werden?");
            sb.AppendLine("3. Was ist die beste Handlungsempfehlung?");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Konvertiert OpenClaw Antwort zu SafetyRecommendation
        /// </summary>
        private SafetyRecommendation ConvertOpenClawResponse(OpenClawResponse response)
        {
            var recommendation = new SafetyRecommendation
            {
                Title = "Sicherheitsanalyse (OpenClaw)",
                RiskLevel = ParseRiskLevel(response.RiskAssessment),
                Explanation = response.Explanation
            };
            
            // Sichere Alternativen
            if (response.ShouldBlock)
            {
                recommendation.SafeAlternatives.Add(new SafeAlternative
                {
                    Action = "Abbrechen",
                    Description = "Vorgang abbrechen und Seite verlassen",
                    Reason = "OpenClaw empfiehlt Blockierung"
                });
            }
            
            if (response.Recommendations != null)
            {
                foreach (var rec in response.Recommendations)
                {
                    recommendation.SafeAlternatives.Add(new SafeAlternative
                    {
                        Action = "Empfehlung",
                        Description = rec,
                        Reason = "OpenClaw Empfehlung"
                    });
                }
            }
            
            return recommendation;
        }
        
        private RiskLevel ParseRiskLevel(string riskAssessment)
        {
            if (string.IsNullOrEmpty(riskAssessment))
                return RiskLevel.None;
            
            var lower = riskAssessment.ToLower();
            if (lower.Contains("high") || lower.Contains("kritisch") || lower.Contains("danger"))
                return RiskLevel.High;
            if (lower.Contains("medium") || lower.Contains("mittel"))
                return RiskLevel.Medium;
            if (lower.Contains("low") || lower.Contains("niedrig"))
                return RiskLevel.Low;
            
            return RiskLevel.None;
        }
        
        /// <summary>
        /// Sicherheits-Report für Benutzer erstellen
        /// </summary>
        public string GenerateUserReport(HandoffResult result)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("╔════════════════════════════════════════════════════════╗");
            sb.AppendLine("║     PC-BEGLEITER SICHERHEITSBERICHT                    ║");
            sb.AppendLine("╚════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            sb.AppendLine($" Zeit: {result.Timestamp:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($" Kontext: {result.Context}");
            sb.AppendLine();
            
            // Risk Level
            sb.AppendLine($" Risiko-Level: {result.RiskLevel}");
            sb.AppendLine($" Risk-Score: {result.RiskScore}/10");
            sb.AppendLine();
            
            // Detected Rules
            if (result.RuleBasedResult?.DetectedRules?.Count > 0)
            {
                sb.AppendLine(" Erkannte Risiken:");
                foreach (var rule in result.RuleBasedResult.DetectedRules)
                {
                    sb.AppendLine($"   • [{rule.Rule.Category}] {rule.Rule.Name}");
                }
                sb.AppendLine();
            }
            
            // Handoff Info
            if (result.HandoffTriggered)
            {
                sb.AppendLine(" OpenClaw-Konsultation:");
                if (result.OpenClawResponse?.Success == true)
                {
                    sb.AppendLine($"   Status: ✓ Berücksichtigt");
                    sb.AppendLine($"   Bewertung: {result.OpenClawResponse.RiskAssessment}");
                }
                else
                {
                    sb.AppendLine($"   Status: ✗ Nicht verfügbar");
                }
                sb.AppendLine();
            }
            
            // Recommendation
            if (result.Recommendation != null)
            {
                sb.AppendLine(" Empfehlung:");
                sb.AppendLine($"   {result.Recommendation.Title}");
                sb.AppendLine($"   {result.Recommendation.Explanation}");
            }
            
            return sb.ToString();
        }
    }
    
    // Request/Response DTOs
    internal class OpenClawRequest
    {
        public string Text { get; set; }
        public string ScreenshotBase64 { get; set; }
        public string Context { get; set; }
        public List<DetectedRule> DetectedRules { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public DateTime Timestamp { get; set; }
        public string Question { get; set; }
    }
    
    internal class OpenClawApiResponse
    {
        public string RiskAssessment { get; set; }
        public string Explanation { get; set; }
        public List<string> Recommendations { get; set; }
        public bool ShouldBlock { get; set; }
    }
    
    /// <summary>
    /// OpenClaw Antwort
    /// </summary>
    public class OpenClawResponse
    {
        public bool Success { get; set; }
        public string RiskAssessment { get; set; }
        public string Explanation { get; set; }
        public List<string> Recommendations { get; set; }
        public bool ShouldBlock { get; set; }
        public string Error { get; set; }
    }
    
    /// <summary>
    /// Komplettes Handoff-Ergebnis
    /// </summary>
    public class HandoffResult
    {
        public DateTime Timestamp { get; set; }
        public string Context { get; set; }
        public SecurityAnalysisResult RuleBasedResult { get; set; }
        public OpenClawResponse OpenClawResponse { get; set; }
        public bool HandoffTriggered { get; set; }
        public int RiskScore { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public SafetyRecommendation Recommendation { get; set; }
    }
}
