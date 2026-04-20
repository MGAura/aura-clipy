using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AuraClipy
{
    /// <summary>
    /// Lokales Logging-System für PC-Begleiter
    /// Speichert Sicherheitsanalysen, Warnungen und Benutzeraktionen
    /// </summary>
    public class SecurityLogger
    {
        private readonly string _logPath;
        private readonly int _maxAgeDays;
        private readonly long _maxFileSize;
        private readonly ReaderWriterLockSlim _lock;
        private readonly LocalConfiguration _config;
        
        public SecurityLogger(LocalConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logPath = _config.Logging.LogPath ?? GetDefaultLogPath();
            _maxAgeDays = _config.Logging.KeepDays;
            _maxFileSize = _config.Logging.MaxFileSize;
            _lock = new ReaderWriterLockSlim();
            
            EnsureLogDirectory();
            CleanupOldLogs();
        }
        
        /// <summary>
        /// Loggt Sicherheitsanalyse-Ergebnis
        /// </summary>
        public void LogAnalysis(ScreenAnalysisResult result, string context = "")
        {
            if (!ShouldLog()) return;
            
            var entry = new LogEntry
            {
                Timestamp = result.Timestamp,
                Type = LogEntryType.Analysis,
                Level = result.RiskLevel == RiskLevel.High ? LogLevel.Warning : LogLevel.Info,
                Title = $"Analyse: {result.RiskLevel}",
                Message = $"Risiko-Score: {result.RiskScore}/10, Kontext: {result.Context}",
                Details = BuildAnalysisDetails(result),
                RiskScore = result.RiskScore,
                RiskLevel = result.RiskLevel,
                Context = context
            };
            
            WriteLog(entry);
        }
        
        /// <summary>
        /// Loggt Warnung
        /// </summary>
        public void LogWarning(string title, string message, int riskScore = 0)
        {
            if (!ShouldLog()) return;
            
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Type = LogEntryType.Warning,
                Level = LogLevel.Warning,
                Title = title,
                Message = message,
                RiskScore = riskScore,
                RiskLevel = GetRiskLevelFromScore(riskScore)
            };
            
            WriteLog(entry);
        }
        
        /// <summary>
        /// Loggt Benutzeraktion
        /// </summary>
        public void LogUserAction(string action, string details, RiskLevel? riskLevel = null)
        {
            if (!ShouldLog()) return;
            
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Type = LogEntryType.UserAction,
                Level = LogLevel.Info,
                Title = $"Benutzeraktion: {action}",
                Message = details,
                RiskLevel = riskLevel ?? RiskLevel.None
            };
            
            WriteLog(entry);
        }
        
        /// <summary>
        /// Loggt OpenClaw Handoff
        /// </summary>
        public void LogOpenClawHandoff(HandoffResult result)
        {
            if (!ShouldLog()) return;
            
            var entry = new LogEntry
            {
                Timestamp = result.Timestamp,
                Type = LogEntryType.OpenClawHandoff,
                Level = result.RiskLevel == RiskLevel.High ? LogLevel.Warning : LogLevel.Info,
                Title = "OpenClaw Handoff",
                Message = $"Risk: {result.RiskScore}/10, Handoff: {(result.HandoffTriggered ? "Ja" : "Nein")}",
                Details = result.OpenClawResponse?.Explanation,
                RiskScore = result.RiskScore,
                RiskLevel = result.RiskLevel
            };
            
            WriteLog(entry);
        }
        
        /// <summary>
        /// Loggt generischen Info-Eintrag
        /// </summary>
        public void LogInfo(string title, string message)
        {
            if (!ShouldLog()) return;
            
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Type = LogEntryType.System,
                Level = LogLevel.Info,
                Title = title,
                Message = message
            };
            
            WriteLog(entry);
        }
        
        /// <summary>
        /// Lädt Analyse-Historie
        /// </summary>
        public List<LogEntry> GetAnalysisHistory(int days = 7, int maxEntries = 100)
        {
            var history = new List<LogEntry>();
            var cutoff = DateTime.Now.AddDays(-days);
            
            _lock.EnterReadLock();
            try
            {
                var logFile = GetCurrentLogFile();
                if (!File.Exists(logFile)) return history;
                
                var lines = File.ReadAllLines(logFile);
                foreach (var line in lines.Reverse().Take(maxEntries * 5))
                {
                    var entry = ParseLogLine(line);
                    if (entry != null && entry.Timestamp > cutoff)
                    {
                        history.Add(entry);
                    }
                }
            }
            catch
            {
                // Ignore errors in history loading
            }
            finally
            {
                _lock.ExitReadLock();
            }
            
            return history.OrderByDescending(e => e.Timestamp).Take(maxEntries).ToList();
        }
        
        /// <summary>
        /// Holt Statistiken
        /// </summary>
        public LogStatistics GetStatistics(int days = 30)
        {
            var stats = new LogStatistics();
            var cutoff = DateTime.Now.AddDays(-days);
            var entries = GetAnalysisHistory(days, 1000);
            
            stats.TotalAnalyses = entries.Count(e => e.Type == LogEntryType.Analysis);
            stats.TotalWarnings = entries.Count(e => e.Level == LogLevel.Warning);
            stats.TotalHighRisk = entries.Count(e => e.RiskLevel == RiskLevel.High);
            stats.TotalMediumRisk = entries.Count(e => e.RiskLevel == RiskLevel.Medium);
            stats.TotalLowRisk = entries.Count(e => e.RiskLevel == RiskLevel.Low);
            
            if (entries.Any(e => e.RiskScore > 0))
            {
                stats.AverageRiskScore = entries.Where(e => e.RiskScore > 0).Average(e => e.RiskScore);
            }
            
            // Detected categories
            stats.DetectedCategories = entries
                .SelectMany(e => e.DetectedCategories ?? new List<string>())
                .GroupBy(c => c)
                .Select(g => new CategoryCount { Category = g.Key, Count = g.Count() })
                .OrderByDescending(c => c.Count)
                .ToList();
            
            return stats;
        }
        
        /// <summary>
        /// Bereinigt alte Logs
        /// </summary>
        public void CleanupOldLogs()
        {
            try
            {
                if (!Directory.Exists(_logPath)) return;
                
                var cutoff = DateTime.Now.AddDays(-_maxAgeDays);
                var files = Directory.GetFiles(_logPath, "*.log");
                
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    if (info.LastWriteTime < cutoff)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
        
        private void WriteLog(LogEntry entry)
        {
            _lock.EnterWriteLock();
            try
            {
                var logFile = GetCurrentLogFile();
                var logLine = FormatLogEntry(entry);
                
                // Check file size and rotate if needed
                if (File.Exists(logFile))
                {
                    var info = new FileInfo(logFile);
                    if (info.Length > _maxFileSize)
                    {
                        RotateLog(logFile);
                    }
                }
                
                File.AppendAllText(logFile, logLine + Environment.NewLine);
            }
            catch
            {
                // Ignore write errors
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        
        private string FormatLogEntry(LogEntry entry)
        {
            var parts = new List<string>
            {
                entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                entry.Level.ToString().ToUpper(),
                entry.Type.ToString(),
                $"\"{Escape(entry.Title)}\"",
                $"\"{Escape(entry.Message)}\"",
                entry.RiskScore.ToString(),
                entry.RiskLevel.ToString(),
                entry.Context ?? ""
            };
            
            return string.Join("|", parts);
        }
        
        private string Escape(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("|", "\\|").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        }
        
        private LogEntry ParseLogLine(string line)
        {
            try
            {
                var parts = line.Split('|');
                if (parts.Length < 7) return null;
                
                return new LogEntry
                {
                    Timestamp = DateTime.Parse(parts[0]),
                    Level = Enum.Parse<LogLevel>(parts[1]),
                    Type = Enum.Parse<LogEntryType>(parts[2]),
                    Title = parts[3].Trim('\"'),
                    Message = parts[4].Trim('\"'),
                    RiskScore = int.Parse(parts[5]),
                    RiskLevel = Enum.Parse<RiskLevel>(parts[6]),
                    Context = parts.Length > 7 ? parts[7] : ""
                };
            }
            catch
            {
                return null;
            }
        }
        
        private string BuildAnalysisDetails(ScreenAnalysisResult result)
        {
            if (result.SafetyAnalysis?.DetectedRules == null) return "";
            
            var details = new List<string>();
            foreach (var rule in result.SafetyAnalysis.DetectedRules.Take(5))
            {
                details.Add($"[{rule.Rule.Category}] {rule.Rule.Name}");
            }
            
            return string.Join("; ", details);
        }
        
        private RiskLevel GetRiskLevelFromScore(int score)
        {
            if (score >= 8) return RiskLevel.High;
            if (score >= 5) return RiskLevel.Medium;
            if (score >= 2) return RiskLevel.Low;
            return RiskLevel.None;
        }
        
        private bool ShouldLog()
        {
            return _config.Logging.Enabled;
        }
        
        private void EnsureLogDirectory()
        {
            try
            {
                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }
            }
            catch
            {
                // Ignore creation errors
            }
        }
        
        private string GetCurrentLogFile()
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            return Path.Combine(_logPath, $"win-assistent-{today}.log");
        }
        
        private void RotateLog(string currentFile)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                var newFile = currentFile.Replace(".log", $"-{timestamp}.log");
                File.Move(currentFile, newFile);
            }
            catch
            {
                // Ignore rotation errors
            }
        }
        
        private string GetDefaultLogPath()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "WinAssistent", "Logs");
        }
    }
    
    public enum LogEntryType
    {
        Analysis,
        Warning,
        UserAction,
        OpenClawHandoff,
        System
    }
    
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
    
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogEntryType Type { get; set; }
        public LogLevel Level { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public int RiskScore { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public string Context { get; set; }
        public List<string> DetectedCategories { get; set; }
    }
    
    public class LogStatistics
    {
        public int TotalAnalyses { get; set; }
        public int TotalWarnings { get; set; }
        public int TotalHighRisk { get; set; }
        public int TotalMediumRisk { get; set; }
        public int TotalLowRisk { get; set; }
        public double AverageRiskScore { get; set; }
        public List<CategoryCount> DetectedCategories { get; set; }
    }
    
    public class CategoryCount
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
}
