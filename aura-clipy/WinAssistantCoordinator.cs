using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuraClipy
{
    /// <summary>
    /// Haupt-Koordinator für PC-Begleiter
    /// Verbindet alle Services: Safety-Layer, Vision, OpenClaw, Config, Logging
    /// </summary>
    public class WinAssistantCoordinator
    {
        // Services
        public LocalConfiguration Config { get; private set; }
        public SimpleRuleEngine RuleEngine { get; private set; }
        public ScreenAnalysisService ScreenAnalysis { get; private set; }
        public VisionAnalyzer VisionAnalyzer { get; private set; }
        public ContextDetector ContextDetector { get; private set; }
        public OpenClawHandoffService OpenClawHandoff { get; private set; }
        public SecurityLogger Logger { get; private set; }
        
        // Events
        public event EventHandler<SecurityEventArgs> OnSecurityWarning;
        public event EventHandler<AnalysisEventArgs> OnAnalysisComplete;
        public event EventHandler<LogEventArgs> OnLogMessage;
        
        private bool _initialized = false;
        
        /// <summary>
        /// Erstellt und initialisiert den Koordinator
        /// </summary>
        public WinAssistantCoordinator(string configPath = null)
        {
            Config = new LocalConfiguration(configPath, msg => Log("CONFIG", msg));
        }
        
        /// <summary>
        /// Initialisiert alle Services
        /// </summary>
        public async Task<bool> InitializeAsync()
        {
            try
            {
                Log("INIT", "Starte PC-Begleiter Initialisierung...");
                
                // 1. Config laden
                if (!Config.Load())
                {
                    Log("WARN", "Konnte Config nicht laden, nutze Standard");
                }
                
                // 2. Rule Engine erstellen und konfigurieren
                RuleEngine = new SimpleRuleEngine();
                Config.ConfigureRuleEngine(RuleEngine);
                Log("INIT", "Rule-Engine initialisiert");
                
                // 3. Screen Analysis
                ScreenAnalysis = new ScreenAnalysisService(
                    RuleEngine, 
                    (level, msg) => Log($"SCREEN:{level}", msg)
                );
                Log("INIT", "Screen-Analysis initialisiert");
                
                // 4. Vision Analyzer (wenn aktiviert)
                if (Config.Analysis.VisionAnalysisEnabled)
                {
                    VisionAnalyzer = new VisionAnalyzer(
                        Config.Analysis.OllamaUrl,
                        Config.Analysis.VisionModel,
                        msg => Log("VISION", msg)
                    );
                    
                    var visionAvailable = await VisionAnalyzer.IsAvailableAsync();
                    if (!visionAvailable)
                    {
                        Log("WARN", "Vision-Analyzer nicht verfügbar");
                    }
                    else
                    {
                        Log("INIT", "Vision-Analyzer bereit");
                    }
                }
                else
                {
                    Log("INFO", "Vision-Analyzer deaktiviert in Config");
                }
                
                // 5. Context Detector
                ContextDetector = new ContextDetector();
                Log("INIT", "Context-Detector initialisiert");
                
                // 6. OpenClaw Handoff (wenn aktiviert)
                if (Config.OpenClaw.Enabled)
                {
                    OpenClawHandoff = new OpenClawHandoffService(
                        Config.OpenClaw.Url,
                        RuleEngine,
                        (level, msg) => Log($"OPENCLAW:{level}", msg)
                    );
                    Log("INIT", "OpenClaw-Handoff initialisiert");
                }
                
                // 7. Logger
                Logger = new SecurityLogger(Config);
                Log("INIT", "Security-Logger initialisiert");
                
                _initialized = true;
                Log("INIT", "PC-Begleiter bereit!");
                
                return true;
            }
            catch (Exception ex)
            {
                Log("ERROR", $"Initialisierung fehlgeschlagen: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Führt vollständige Sicherheitsanalyse durch
        /// </summary>
        public async Task<AnalysisResult> PerformAnalysisAsync(string text = null, byte[] screenshot = null)
        {
            if (!_initialized)
            {
                Log("ERROR", "Koordinator nicht initialisiert");
                return new AnalysisResult { Error = "Nicht initialisiert" };
            }
            
            try
            {
                Log("ANALYSIS", "Starte Sicherheitsanalyse...");
                
                // 1. Kontext erkennen
                var contextResult = ContextDetector.DetectCurrentContext();
                var context = contextResult.DetectedType.ToString();
                
                if (!string.IsNullOrEmpty(text))
                {
                    // Text-basierte Analyse
                    var textContext = ContextDetector.DetectFromText(text);
                    if (textContext != ContextSourceType.Unknown)
                    {
                        context = textContext.ToString();
                    }
                }
                
                Log("ANALYSIS", $"Kontext: {context}");
                
                // 2. Rule-basierte Analyse
                var ruleResult = RuleEngine.AnalyzeText(text ?? "", context);
                
                // 3. Optional: Vision + OpenClaw Handoff
                SafetyRecommendation recommendation;
                OpenClawResponse openClawResponse = null;
                
                if (Config.OpenClaw.Enabled && OpenClawHandoff != null)
                {
                    var handoffResult = await OpenClawHandoff.AnalyzeWithOptionalHandoff(
                        text ?? "", screenshot, context
                    );
                    
                    recommendation = handoffResult.Recommendation;
                    openClawResponse = handoffResult.OpenClawResponse;
                    
                    // Loggen
                    Logger.LogOpenClawHandoff(handoffResult);
                }
                else
                {
                    recommendation = RuleEngine.GetRecommendation(ruleResult);
                }
                
                // 4. Ergebnis erstellen
                var result = new AnalysisResult
                {
                    Timestamp = DateTime.Now,
                    Context = context,
                    RiskScore = ruleResult.OverallRiskScore,
                    RiskLevel = ruleResult.RiskLevel,
                    DetectedRules = ruleResult.DetectedRules,
                    Recommendation = recommendation,
                    OpenClawResponse = openClawResponse,
                    TextAnalyzed = text ?? "",
                    ScreenshotAnalyzed = screenshot != null
                };
                
                // 5. Loggen
                var screenResult = new ScreenAnalysisResult
                {
                    Timestamp = result.Timestamp,
                    Context = context,
                    RiskScore = result.RiskScore,
                    RiskLevel = result.RiskLevel,
                    SafetyAnalysis = ruleResult
                };
                Logger.LogAnalysis(screenResult, context);
                
                // 6. Event auslösen
                OnAnalysisComplete?.Invoke(this, new AnalysisEventArgs(result));
                
                // 7. Bei hohem Risiko: Warnung
                if (result.RiskLevel == RiskLevel.High)
                {
                    OnSecurityWarning?.Invoke(this, new SecurityEventArgs(result));
                }
                
                Log("ANALYSIS", $"Analyse abgeschlossen: {result.RiskLevel} ({result.RiskScore}/10)");
                
                return result;
            }
            catch (Exception ex)
            {
                Log("ERROR", $"Analyse fehlgeschlagen: {ex.Message}");
                return new AnalysisResult { Error = ex.Message };
            }
        }
        
        /// <summary>
        /// Holt Statistiken
        /// </summary>
        public LogStatistics GetStatistics(int days = 30)
        {
            return Logger?.GetStatistics(days) ?? new LogStatistics();
        }
        
        /// <summary>
        /// Aktualisiert Konfiguration
        /// </summary>
        public void UpdateConfig(Action<LocalConfiguration> configAction)
        {
            configAction?.Invoke(Config);
            Config.Save();
            Config.ConfigureRuleEngine(RuleEngine);
        }
        
        /// <summary>
        /// Stoppt alle Services
        /// </summary>
        public void Shutdown()
        {
            Log("SHUTDOWN", "Beende PC-Begleiter...");
            
            try
            {
                // Config speichern
                Config?.Save();
                
                _initialized = false;
                Log("SHUTDOWN", "PC-Begleiter beendet");
            }
            catch (Exception ex)
            {
                Log("ERROR", $"Shutdown Fehler: {ex.Message}");
            }
        }
        
        private void Log(string level, string message)
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] [{level}] {message}";
            OnLogMessage?.Invoke(this, new LogEventArgs(logMessage));
        }
    }
    
    // Event Args
    public class SecurityEventArgs : EventArgs
    {
        public AnalysisResult Result { get; }
        
        public SecurityEventArgs(AnalysisResult result)
        {
            Result = result;
        }
    }
    
    public class AnalysisEventArgs : EventArgs
    {
        public AnalysisResult Result { get; }
        
        public AnalysisEventArgs(AnalysisResult result)
        {
            Result = result;
        }
    }
    
    public class LogEventArgs : EventArgs
    {
        public string Message { get; }
        
        public LogEventArgs(string message)
        {
            Message = message;
        }
    }
    
    // Analysis Result
    public class AnalysisResult
    {
        public DateTime Timestamp { get; set; }
        public string Context { get; set; }
        public int RiskScore { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public List<DetectedRule> DetectedRules { get; set; }
        public SafetyRecommendation Recommendation { get; set; }
        public OpenClawResponse OpenClawResponse { get; set; }
        public string TextAnalyzed { get; set; }
        public bool ScreenshotAnalyzed { get; set; }
        public string Error { get; set; }
    }
}
