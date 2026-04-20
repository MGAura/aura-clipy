using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AuraClipy
{
    /// <summary>
    /// Local-First Konfigurationssystem für PC-Begleiter
    /// Speichert alle Einstellungen lokal in JSON
    /// </summary>
    public class LocalConfiguration
    {
        private readonly string _configPath;
        private readonly Action<string> _logCallback;
        private ConfigurationData _config;
        
        public LocalConfiguration(string configPath = null, Action<string> logCallback = null)
        {
            _configPath = configPath ?? GetDefaultConfigPath();
            _logCallback = logCallback;
            _config = new ConfigurationData();
        }
        
        /// <summary>
        /// Lädt Konfiguration aus Datei
        /// </summary>
        public bool Load()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    _Log($"Config nicht gefunden, erstelle Standard-Config: {_configPath}");
                    CreateDefaultConfig();
                    Save();
                    return true;
                }
                
                var json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<ConfigurationData>(json);
                _Log("Config geladen");
                return true;
            }
            catch (Exception ex)
            {
                _Log($"Fehler beim Laden: {ex.Message}");
                CreateDefaultConfig();
                return false;
            }
        }
        
        /// <summary>
        /// Speichert Konfiguration
        /// </summary>
        public bool Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_configPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_config, options);
                File.WriteAllText(_configPath, json);
                _Log("Config gespeichert");
                return true;
            }
            catch (Exception ex)
            {
                _Log($"Fehler beim Speichern: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Erstellt Standard-Konfiguration
        /// </summary>
        private void CreateDefaultConfig()
        {
            _config = new ConfigurationData
            {
                Version = "0.2.0",
                Created = DateTime.Now,
                LastModified = DateTime.Now,
                
                // General Settings
                General = new GeneralSettings
                {
                    Enabled = true,
                    AutoStart = true,
                    MinimizeToTray = true,
                    Language = "de-DE",
                    Theme = "System"
                },
                
                // Safety Layer Settings
                SafetyLayer = new SafetyLayerSettings
                {
                    Enabled = true,
                    RiskThreshold = 5,
                    AutoAnalyze = true,
                    ShowOverlay = true,
                    PlaySound = true,
                    NotificationTimeout = 10,
                    BlockHighRisk = false,
                    AskForConfirmation = true
                },
                
                // Analysis Settings
                Analysis = new AnalysisSettings
                {
                    AutoAnalyzeClipboard = false,
                    AutoAnalyzeBrowser = true,
                    AutoAnalyzeEmail = true,
                    AutoAnalyzePopups = true,
                    ContextDetection = true,
                    ScreenCaptureInterval = 30,
                    VisionAnalysisEnabled = false,
                    VisionModel = "llava:7b",
                    OllamaUrl = "http://127.0.0.1:11434"
                },
                
                // OpenClaw Settings
                OpenClaw = new OpenClawSettings
                {
                    Enabled = true,
                    Url = "http://127.0.0.1:3000",
                    AutoHandoff = true,
                    HandoffThreshold = 5,
                    Timeout = 60,
                    UseScreenshot = true
                },
                
                // Rule Configuration
                Rules = new RuleSettings
                {
                    DefaultEnabled = true,
                    CustomRiskScores = new Dictionary<string, int>(),
                    DisabledRules = new List<string>(),
                    DisabledCategories = new List<string>()
                },
                
                // Logging Settings
                Logging = new LoggingSettings
                {
                    Enabled = true,
                    LogLevel = "INFO",
                    KeepDays = 30,
                    MaxFileSize = 10485760, // 10MB
                    LogPath = GetDefaultLogPath()
                },
                
                // Privacy Settings
                Privacy = new PrivacySettings
                {
                    StoreScreenshots = false,
                    StoreText = true,
                    AnonymizeData = true,
                    CloudEnabled = false,
                    CloudUrl = ""
                },
                
                // UI Settings
                UI = new UISettings
                {
                    OverlayPosition = "TopRight",
                    OverlayOpacity = 0.9f,
                    ShowTransparencyIndicator = true,
                    RiskColorCoding = true,
                    CompactMode = false
                }
            };
            
            // Standard Custom Risk Scores
            _config.Rules.CustomRiskScores["PHISH_EMAIL_URGENCY"] = 8;
            _config.Rules.CustomRiskScores["FINANCIAL_BANK_FAKE"] = 10;
            _config.Rules.CustomRiskScores["CREDENTIAL_PASSWORD_REQUEST"] = 9;
        }
        
        // Accessors
        public ConfigurationData Config => _config;
        public GeneralSettings General => _config.General;
        public SafetyLayerSettings Safety => _config.SafetyLayer;
        public AnalysisSettings Analysis => _config.Analysis;
        public OpenClawSettings OpenClaw => _config.OpenClaw;
        public RuleSettings Rules => _config.Rules;
        public LoggingSettings Logging => _config.Logging;
        public PrivacySettings Privacy => _config.Privacy;
        public UISettings UI => _config.UI;
        
        /// <summary>
        /// Konfiguriert Rule-Engine basierend auf gespeicherten Regeln
        /// </summary>
        public void ConfigureRuleEngine(SimpleRuleEngine engine)
        {
            if (engine == null) return;
            
            // Deaktivierte Regeln
            foreach (var ruleId in _config.Rules.DisabledRules)
            {
                engine.SetRuleEnabled(ruleId, false);
            }
            
            // Deaktivierte Kategorien
            foreach (var categoryStr in _config.Rules.DisabledCategories)
            {
                if (Enum.TryParse<RiskCategory>(categoryStr, out var category))
                {
                    engine.SetCategoryEnabled(category, false);
                }
            }
            
            // Custom Risk Scores
            foreach (var kvp in _config.Rules.CustomRiskScores)
            {
                engine.SetCustomRiskScore(kvp.Key, kvp.Value);
            }
        }
        
        /// <summary>
        /// Speichert Rule-Engine Konfiguration
        /// </summary>
        public void SaveRuleEngineConfig(SimpleRuleEngine engine)
        {
            if (engine == null) return;
            
            _config.Rules.CustomRiskScores = engine.GetAllCustomRiskScores();
            // Weitere Speicherung basierend auf engine-Status
            Save();
        }
        
        /// <summary>
        /// Pfad zur Konfigurationsdatei
        /// </summary>
        public string ConfigPath => _configPath;
        
        private string GetDefaultConfigPath()
        {
            // Local Application Data (roaming für mehrere PCs)
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configDir = Path.Combine(appData, "WinAssistent");
            return Path.Combine(configDir, "config.json");
        }
        
        private string GetDefaultLogPath()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "WinAssistent", "Logs");
        }
        
        private void _Log(string message)
        {
            _logCallback?.Invoke($"[Config] {message}");
        }
    }
    
    // Konfigurations-Datenstruktur
    public class ConfigurationData
    {
        public string Version { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public GeneralSettings General { get; set; }
        public SafetyLayerSettings SafetyLayer { get; set; }
        public AnalysisSettings Analysis { get; set; }
        public OpenClawSettings OpenClaw { get; set; }
        public RuleSettings Rules { get; set; }
        public LoggingSettings Logging { get; set; }
        public PrivacySettings Privacy { get; set; }
        public UISettings UI { get; set; }
    }
    
    public class GeneralSettings
    {
        public bool Enabled { get; set; }
        public bool AutoStart { get; set; }
        public bool MinimizeToTray { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
    }
    
    public class SafetyLayerSettings
    {
        public bool Enabled { get; set; }
        public int RiskThreshold { get; set; }
        public bool AutoAnalyze { get; set; }
        public bool ShowOverlay { get; set; }
        public bool PlaySound { get; set; }
        public int NotificationTimeout { get; set; }
        public bool BlockHighRisk { get; set; }
        public bool AskForConfirmation { get; set; }
    }
    
    public class AnalysisSettings
    {
        public bool AutoAnalyzeClipboard { get; set; }
        public bool AutoAnalyzeBrowser { get; set; }
        public bool AutoAnalyzeEmail { get; set; }
        public bool AutoAnalyzePopups { get; set; }
        public bool ContextDetection { get; set; }
        public int ScreenCaptureInterval { get; set; }
        public bool VisionAnalysisEnabled { get; set; }
        public string VisionModel { get; set; }
        public string OllamaUrl { get; set; }
    }
    
    public class OpenClawSettings
    {
        public bool Enabled { get; set; }
        public string Url { get; set; }
        public bool AutoHandoff { get; set; }
        public int HandoffThreshold { get; set; }
        public int Timeout { get; set; }
        public bool UseScreenshot { get; set; }
    }
    
    public class RuleSettings
    {
        public bool DefaultEnabled { get; set; }
        public Dictionary<string, int> CustomRiskScores { get; set; }
        public List<string> DisabledRules { get; set; }
        public List<string> DisabledCategories { get; set; }
    }
    
    public class LoggingSettings
    {
        public bool Enabled { get; set; }
        public string LogLevel { get; set; }
        public int KeepDays { get; set; }
        public long MaxFileSize { get; set; }
        public string LogPath { get; set; }
    }
    
    public class PrivacySettings
    {
        public bool StoreScreenshots { get; set; }
        public bool StoreText { get; set; }
        public bool AnonymizeData { get; set; }
        public bool CloudEnabled { get; set; }
        public string CloudUrl { get; set; }
    }
    
    public class UISettings
    {
        public string OverlayPosition { get; set; }
        public float OverlayOpacity { get; set; }
        public bool ShowTransparencyIndicator { get; set; }
        public bool RiskColorCoding { get; set; }
        public bool CompactMode { get; set; }
    }
}
