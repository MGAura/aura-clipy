using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuraClipy
{
    /// <summary>
    /// Vision-Analyzer für OCR und Bildanalyse via Llava/Ollama
    /// Integration für fortgeschrittene Screen-Analyse
    /// </summary>
    public class VisionAnalyzer
    {
        private readonly string _ollamaUrl;
        private readonly string _modelName;
        private readonly HttpClient _httpClient;
        private readonly Action<string> _logCallback;
        
        /// <summary>
        /// Erstellt Vision-Analyzer
        /// </summary>
        /// <param name="ollamaUrl">Ollama Server URL (default: http://127.0.0.1:11434)</param>
        /// <param name="modelName">Modelname für Vision (default: llava:7b)</param>
        /// <param name="logCallback">Optionaler Logger Callback</param>
        public VisionAnalyzer(string ollamaUrl = "http://127.0.0.1:11434", 
                            string modelName = "llava:7b",
                            Action<string> logCallback = null)
        {
            _ollamaUrl = ollamaUrl.TrimEnd('/');
            _modelName = modelName;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
            _logCallback = logCallback ?? (_ => { });
        }
        
        /// <summary>
        /// Prüft ob Ollama mit Vision-Modell verfügbar ist
        /// </summary>
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                // Check if Ollama is running
                var response = await _httpClient.GetAsync($"{_ollamaUrl}/api/tags");
                if (!response.IsSuccessStatusCode)
                {
                    _logCallback($"Ollama nicht erreichbar: {response.StatusCode}");
                    return false;
                }
                
                // Check if model is available
                var content = await response.Content.ReadAsStringAsync();
                var models = JsonSerializer.Deserialize<OllamaTagsResponse>(content);
                
                bool modelAvailable = models?.Models?.Any(m => m.Name.Contains("llava")) ?? false;
                
                if (!modelAvailable)
                {
                    _logCallback($"Model '{_modelName}' nicht in Ollama gefunden");
                }
                else
                {
                    _logCallback($"Vision-Analyzer bereit mit {_modelName}");
                }
                
                return modelAvailable;
            }
            catch (Exception ex)
            {
                _logCallback($"Vision-Analyzer Fehler: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Analysiert Bild auf sicherheitsrelevante Inhalte
        /// </summary>
        public async Task<VisionAnalysisResult> AnalyzeImageAsync(byte[] imageBytes, string prompt = null)
        {
            var result = new VisionAnalysisResult();
            
            if (prompt == null)
            {
                prompt = @"Analysiere dieses Bild auf Sicherheitsrisiken. 
Prüfe speziell auf:
1. Phishing-Elemente (gefälschte Login-Felder, verdächtige Links)
2. System-Warnungen (Fake-Fehlermeldungen, Betrugs-Dialoge)
3. Verdächtige E-Mail-Inhalte
4. Fake Software-Popups
5. Kreditkarten/Zahlungsformulare

Antworte im Format:
- RISIKO: [Hoch/Mittel/Niedrig/Unbekannt]
- ERKENNUNGEN: [Liste der erkannten Probleme]
- EMPFEHLUNG: [Was der Nutzer tun sollte]";
            }
            
            try
            {
                // Convert image to base64
                string base64Image = Convert.ToBase64String(imageBytes);
                
                // Build request
                var requestBody = new
                {
                    model = _modelName,
                    prompt = prompt,
                    images = new[] { base64Image },
                    stream = false
                };
                
                var json = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logCallback($"Sende Bild an {_modelName}...");
                
                var response = await _httpClient.PostAsync($"{_ollamaUrl}/api/generate", httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    result.Error = $"Ollama Fehler: {response.StatusCode}";
                    result.Response = responseContent;
                    _logCallback(result.Error);
                    return result;
                }
                
                var ollamaResponse = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseContent);
                result.Response = ollamaResponse?.Response ?? "";
                result.Success = true;
                
                // Parse simple risk level from response
                result.ParsedRiskLevel = ParseRiskLevel(result.Response);
                
                _logCallback($"Analyse abgeschlossen: {result.ParsedRiskLevel}");
            }
            catch (TaskCanceledException)
            {
                result.Error = "Timeout - Ollama antwortet nicht";
                _logCallback(result.Error);
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                _logCallback($"Vision-Analyse Fehler: {ex.Message}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Extrahiert Text aus Bild (vereinfachtes OCR)
        /// </summary>
        public async Task<string> ExtractTextAsync(byte[] imageBytes)
        {
            var prompt = @"Extrahiere ALLEN lesbaren Text aus diesem Bild. 
Gib den Text genau so wieder wie er erscheint, ohne Zusammenfassung.
Wenn kein Text erkennbar ist, antworte mit: [KEIN TEXT GEFUNDEN]";
            
            var result = await AnalyzeImageAsync(imageBytes, prompt);
            return result.Response ?? "";
        }
        
        /// <summary>
        /// Prüft ob Bild verdächtige UI-Elemente enthält
        /// </summary>
        public async Task<SuspiciousUIElements> DetectSuspiciousUIElementsAsync(byte[] imageBytes)
        {
            var prompt = @"Analysiere dieses Bild auf SUSPEKTE UI-ELEMENTE.
Prüfe auf:
1. Login-Formulare die wie bekannte Marken aussehen (Microsoft, Google, Apple, Amazon, PayPal, Banken)
2. System-Dialoge die echt wirken aber nicht von Windows stammen
3. Fake-Fehlermeldungen oder Warnungen
4. Eingabefelder für: Passwörter, Kreditkartennummern, Bankdaten
5. Telefonnummern für "Tech Support" oder "Kundendienst"
6. Popups die zum Anrufen auffordern
7. Download-Buttons oder -Links

Antworte im Format:
UI-ELEMENTE: [Liste der erkannten Elemente mit kurzer Beschreibung]
RISIKO: [Hoch/Mittel/Niedrig]
HANDLUNG: [Empfohlene Aktion]";

            var result = await AnalyzeImageAsync(imageBytes, prompt);
            
            return new SuspiciousUIElements
            {
                RawResponse = result.Response,
                DetectedElements = ParseUIElements(result.Response),
                RiskLevel = result.ParsedRiskLevel,
                Success = result.Success
            };
        }
        
        private RiskLevel ParseRiskLevel(string response)
        {
            var lower = response.ToLower();
            
            if (lower.Contains("hoch") || lower.Contains("high") || lower.Contains("risk"))
                return RiskLevel.High;
            if (lower.Contains("mittel") || lower.Contains("medium"))
                return RiskLevel.Medium;
            if (lower.Contains("niedrig") || lower.Contains("low") || lower.Contains("gering"))
                return RiskLevel.Low;
            
            return RiskLevel.None;
        }
        
        private List<string> ParseUIElements(string response)
        {
            var elements = new List<string>();
            
            // Simple parsing - look for lines with specific keywords
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var lower = line.ToLower();
                if (lower.Contains("login") || lower.Contains("password") || 
                    lower.Contains("konto") || lower.Contains("email") ||
                    lower.Contains("popup") || lower.Contains("warnung") ||
                    lower.Contains("download") || lower.Contains("telefon") ||
                    lower.Contains("support") || lower.Contains("kredit"))
                {
                    elements.Add(line.Trim());
                }
            }
            
            return elements;
        }
        
        /// <summary>
        /// Kombinierte Analyse: Safety Rules + Vision
        /// </summary>
        public async Task<CombinedSafetyResult> CombinedAnalysisAsync(
            string extractedText, 
            byte[] screenshotBytes,
            SimpleRuleEngine ruleEngine)
        {
            var result = new CombinedSafetyResult
            {
                Timestamp = DateTime.Now
            };
            
            // 1. Rule-based analysis
            _logCallback("Starte Rule-basierte Analyse...");
            var ruleResult = ruleEngine.AnalyzeText(extractedText);
            result.RuleBasedAnalysis = ruleResult;
            result.RiskScore = ruleResult.OverallRiskScore;
            result.RiskLevel = ruleResult.RiskLevel;
            _logCallback($"Rule-Analyse: {result.RiskScore}/10");
            
            // 2. Vision-based analysis (if available)
            if (screenshotBytes != null && screenshotBytes.Length > 0)
            {
                _logCallback("Starte Vision-Analyse...");
                var visionResult = await AnalyzeImageAsync(screenshotBytes);
                result.VisionAnalysis = visionResult;
                
                if (visionResult.Success)
                {
                    result.VisionAvailable = true;
                    
                    // Take higher risk if Vision detects something
                    if (visionResult.ParsedRiskLevel > result.RiskLevel)
                    {
                        result.RiskLevel = visionResult.ParsedRiskLevel;
                    }
                    
                    result.CombinedResponse = $"=== RULE-BASED ===\n{ruleResult.RiskLevel} Risk\n\n=== VISION-BASED ===\n{visionResult.Response}";
                }
                else
                {
                    result.VisionAvailable = false;
                    result.CombinedResponse = $"=== RULE-BASED ===\n{ruleResult.RiskLevel} Risk\n\n=== VISION ===\n{visionResult.Error ?? "Nicht verfügbar"}";
                }
            }
            else
            {
                result.VisionAvailable = false;
                result.CombinedResponse = $"=== RULE-BASED ===\n{ruleResult.RiskLevel} Risk";
            }
            
            // 3. Generate combined recommendation
            result.Recommendation = ruleEngine.GetRecommendation(ruleResult);
            
            return result;
        }
    }
    
    // Response DTOs
    internal class OllamaTagsResponse
    {
        public OllamaModel[] Models { get; set; }
    }
    
    internal class OllamaModel
    {
        public string Name { get; set; }
    }
    
    internal class OllamaGenerateResponse
    {
        public string Response { get; set; }
        public bool Done { get; set; }
    }
    
    /// <summary>
    /// Ergebnis der Vision-Analyse
    /// </summary>
    public class VisionAnalysisResult
    {
        public bool Success { get; set; }
        public string Response { get; set; }
        public RiskLevel ParsedRiskLevel { get; set; }
        public string Error { get; set; }
    }
    
    /// <summary>
    /// Erkannte verdächtige UI-Elemente
    /// </summary>
    public class SuspiciousUIElements
    {
        public string RawResponse { get; set; }
        public List<string> DetectedElements { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public bool Success { get; set; }
    }
    
    /// <summary>
    /// Kombiniertes Analyse-Ergebnis
    /// </summary>
    public class CombinedSafetyResult
    {
        public DateTime Timestamp { get; set; }
        public SecurityAnalysisResult RuleBasedAnalysis { get; set; }
        public VisionAnalysisResult VisionAnalysis { get; set; }
        public bool VisionAvailable { get; set; }
        public int RiskScore { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public string CombinedResponse { get; set; }
        public SafetyRecommendation Recommendation { get; set; }
    }
}
