using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AuraClipy
{
    /// <summary>
    /// Service für automatisierte Screen-Analyse mit Safety-Layer Integration
    /// Erfasst Bildschirm und leitet Text zur Sicherheitsanalyse weiter
    /// </summary>
    public class ScreenAnalysisService
    {
        private readonly SimpleRuleEngine _ruleEngine;
        private readonly Action<string, string> _logCallback;
        
        public ScreenAnalysisService(SimpleRuleEngine ruleEngine, Action<string, string> logCallback = null)
        {
            _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
            _logCallback = logCallback;
        }
        
        /// <summary>
        /// Führt vollständige Screen-Sicherheitsanalyse durch
        /// </summary>
        public ScreenAnalysisResult AnalyzeScreen(string contextHint = "unknown")
        {
            _logCallback?.Invoke("INFO", "Starte Screen-Analyse...");
            
            var result = new ScreenAnalysisResult
            {
                Timestamp = DateTime.Now,
                Context = contextHint
            };
            
            try
            {
                // Screen capture
                var screens = CaptureScreen();
                result.ScreensCaptured = screens.Count;
                
                _Log("INFO", $" {screens.Count} Bildschirm(e) erfasst");
                
                // Text extrahieren
                var extractedText = new StringBuilder();
                foreach (var screen in screens)
                {
                    var screenText = ExtractTextFromScreen(screen);
                    extractedText.AppendLine(screenText);
                    result.TextExtracted += screenText.Length;
                }
                
                result.ExtractedText = extractedText.ToString();
                
                // Kontext erkennen
                result.DetectedContext = DetectContext(result.ExtractedText);
                if (string.IsNullOrEmpty(contextHint) || contextHint == "unknown")
                {
                    result.Context = result.DetectedContext;
                }
                
                _Log("INFO", $" Kontext erkannt: {result.DetectedContext}");
                _Log("INFO", $" Textlänge: {result.TextExtracted} Zeichen");
                
                // Sicherheitsanalyse durchführen
                var safetyResult = _ruleEngine.AnalyzeText(result.ExtractedText, result.Context);
                result.SafetyAnalysis = safetyResult;
                result.RiskScore = safetyResult.OverallRiskScore;
                result.RiskLevel = safetyResult.RiskLevel;
                
                // Empfehlungen abrufen
                result.Recommendation = _ruleEngine.GetRecommendation(safetyResult);
                
                _Log("INFO", $" Risiko: {result.RiskScore}/10 ({result.RiskLevel})");
                
                if (safetyResult.DetectedRules.Count > 0)
                {
                    _Log("WARN", $" {safetyResult.DetectedRules.Count} verdächtige Muster gefunden:");
                    foreach (var rule in safetyResult.DetectedRules.Take(5))
                    {
                        _Log("WARN", $"   - {rule.Rule.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                _Log("ERROR", $" Screen-Analyse fehlgeschlagen: {ex.Message}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Erfasst Bildschirm (vereinfachte Version ohne externe Dependencies)
        /// </summary>
        private List<CapturedScreen> CaptureScreen()
        {
            var screens = new List<CapturedScreen>();
            
            try
            {
                // Using System.Windows.Forms.Screen
                var allScreens = Screen.AllScreens;
                var cursorPos = Cursor.Position;
                
                // Order screens: cursor screen first
                var orderedScreens = allScreens
                    .OrderByDescending(s => s.Bounds.Contains(cursorPos))
                    .ToList();
                
                int screenNum = 1;
                foreach (var screen in orderedScreens)
                {
                    try
                    {
                        var bounds = screen.Bounds;
                        
                        using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                            
                            // Scale to max 800px for text extraction
                            var scaled = ScaleBitmap(bitmap, 800);
                            
                            screens.Add(new CapturedScreen
                            {
                                ScreenNumber = screenNum++,
                                Bounds = bounds,
                                IsCursorOnScreen = bounds.Contains(cursorPos),
                                Bitmap = scaled
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _Log("WARN", $" Fehler bei Screen {screenNum}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _Log("ERROR", $" Screen-Capture fehlgeschlagen: {ex.Message}");
            }
            
            return screens;
        }
        
        /// <summary>
        /// Extrahiert "lesbaren" Text aus Bildschirm (Simuliert - echte OCR wäre Llava/Vision)
        /// Für Prototyp: simuliert Text-Extraktion basierend auf bekannten Mustern
        /// </summary>
        private string ExtractTextFromScreen(CapturedScreen screen)
        {
            // Für echte OCR würde hier Llava/Vision integriert werden
            // Aktuell: Dummy-Text für Demonstrationszwecke
            var sb = new StringBuilder();
            
            sb.AppendLine($"[Screen {screen.ScreenNumber}]");
            
            // Simuliere einige häufige Textmuster
            // In echtem Einsatz: Llava für OCR
            if (screen.IsCursorOnScreen)
            {
                sb.AppendLine("PRIMÄRER BILDSCHIRM (Cursor hier)");
            }
            
            // Placeholder für zukünftige OCR-Integration
            sb.AppendLine("[OCR-Text würde hier erscheinen - Llava Integration geplant]");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Erkennt Kontext basierend auf extrahiertem Text
        /// </summary>
        public string DetectContext(string text)
        {
            var lowerText = text.ToLower();
            
            // E-Mail Indikatoren
            string[] emailIndicators = { "betreff", "an:", "von:", "Cc:", "smtp", "mail", "inbox", 
                                         " Posteingang", "Gesendet", "Entwürfe", "@" };
            if (emailIndicators.Any(ind => lowerText.Contains(ind.ToLower())))
                return "EmailClient";
            
            // Browser Indikatoren
            string[] browserIndicators = { "http://", "https://", "www.", ".com", ".de", ".org",
                                          "browser", "tab", "suchen", "google", "bing" };
            if (browserIndicators.Any(ind => lowerText.Contains(ind.ToLower())))
                return "Browser";
            
            // Popup/System Dialog Indikatoren
            string[] popupIndicators = { "warnung", "fehler", "ok", "abbrechen", "ja", "nein",
                                        "system", "dialog", "popup", "meldung" };
            if (popupIndicators.Any(ind => lowerText.Contains(ind.ToLower())))
                return "Popup";
            
            // File Explorer Indikatoren
            string[] explorerIndicators = { "desktop", "dokumente", "downloads", "bilder",
                                           "dateien", "ordner", "verzeichnis", "c:\\", "d:\\" };
            if (explorerIndicators.Any(ind => lowerText.Contains(ind.ToLower())))
                return "FileExplorer";
            
            return "Unknown";
        }
        
        /// <summary>
        /// Analysiert spezifischen Text direkt (ohne Screen-Capture)
        /// </summary>
        public ScreenAnalysisResult AnalyzeText(string text, string context = "unknown")
        {
            var result = new ScreenAnalysisResult
            {
                Timestamp = DateTime.Now,
                Context = context,
                ExtractedText = text,
                TextExtracted = text?.Length ?? 0,
                DetectedContext = DetectContext(text ?? ""),
                ScreensCaptured = 0
            };
            
            if (string.IsNullOrWhiteSpace(text))
            {
                result.SafetyAnalysis = new SecurityAnalysisResult();
                result.Recommendation = _ruleEngine.GetRecommendation(result.SafetyAnalysis);
                return result;
            }
            
            // Sicherheitsanalyse
            var safetyResult = _ruleEngine.AnalyzeText(text, result.DetectedContext);
            result.SafetyAnalysis = safetyResult;
            result.RiskScore = safetyResult.OverallRiskScore;
            result.RiskLevel = safetyResult.RiskLevel;
            result.Recommendation = _ruleEngine.GetRecommendation(safetyResult);
            
            return result;
        }
        
        private Bitmap ScaleBitmap(Bitmap original, int maxDimension)
        {
            if (original == null) return null;
            
            double ratio = Math.Min((double)maxDimension / original.Width, 
                                   (double)maxDimension / original.Height);
            
            if (ratio >= 1.0) return original;
            
            int newWidth = (int)(original.Width * ratio);
            int newHeight = (int)(original.Height * ratio);
            
            var scaled = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(scaled))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(original, 0, 0, newWidth, newHeight);
            }
            
            return scaled;
        }
        
        private void _Log(string level, string message)
        {
            _logCallback?.Invoke(level, message);
        }
    }
    
    /// <summary>
    /// Ergebnis einer Screen-Analyse
    /// </summary>
    public class ScreenAnalysisResult
    {
        public DateTime Timestamp { get; set; }
        public string Context { get; set; }
        public string DetectedContext { get; set; }
        public string ExtractedText { get; set; }
        public int TextExtracted { get; set; }
        public int ScreensCaptured { get; set; }
        public int RiskScore { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public SecurityAnalysisResult SafetyAnalysis { get; set; }
        public SafetyRecommendation Recommendation { get; set; }
        public string Error { get; set; }
    }
    
    /// <summary>
    /// Interner Datentyp für Screen-Capture
    /// </summary>
    internal class CapturedScreen
    {
        public int ScreenNumber { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsCursorOnScreen { get; set; }
        public Bitmap Bitmap { get; set; }
    }
}
