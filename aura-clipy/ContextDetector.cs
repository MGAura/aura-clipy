using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AuraClipy
{
    /// <summary>
    /// Service für automatische Kontext-Erkennung
    /// Erkennt ob Benutzer in E-Mail, Browser, Popup, etc. ist
    /// </summary>
    public class ContextDetector
    {
        // Windows API für Fenster-Informationen
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);
        
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        
        private readonly Dictionary<string, ContextSourceType> _windowPatterns;
        private readonly Dictionary<string, ContextSourceType> _processPatterns;
        
        public ContextDetector()
        {
            // Bekannte Fenster-Titel Muster
            _windowPatterns = new Dictionary<string, ContextSourceType>(StringComparer.OrdinalIgnoreCase)
            {
                // E-Mail Clients
                { "outlook", ContextSourceType.EmailClient },
                { "thunderbird", ContextSourceType.EmailClient },
                { "mail", ContextSourceType.EmailClient },
                { "posteo", ContextSourceType.EmailClient },
                { "web.de", ContextSourceType.EmailClient },
                { "gmx", ContextSourceType.EmailClient },
                { "gmail", ContextSourceType.EmailClient },
                { " Posteingang", ContextSourceType.EmailClient },
                { "Verfassen", ContextSourceType.EmailClient },
                { " - Nachricht", ContextSourceType.EmailClient },
                
                // Browser
                { "chrome", ContextSourceType.Browser },
                { "firefox", ContextSourceType.Browser },
                { "edge", ContextSourceType.Browser },
                { "opera", ContextSourceType.Browser },
                { "brave", ContextSourceType.Browser },
                { "browser", ContextSourceType.Browser },
                { "tabs", ContextSourceType.Browser },
                
                // Popups und Dialoge
                { "Popup", ContextSourceType.Popup },
                { "Dialog", ContextSourceType.Popup },
                { "Meldung", ContextSourceType.Popup },
                { "Bestätigung", ContextSourceType.Popup },
                { "Warnung", ContextSourceType.Popup },
                { "Fehler", ContextSourceType.Popup },
                { "Question", ContextSourceType.Popup },
                { "Confirm", ContextSourceType.Popup },
                
                // System Dialoge
                { "Einstellungen", ContextSourceType.SystemDialog },
                { "Settings", ContextSourceType.SystemDialog },
                { "Systemsteuerung", ContextSourceType.SystemDialog },
                { "Control Panel", ContextSourceType.SystemDialog },
                { "Task-Manager", ContextSourceType.SystemDialog },
                { "Task Manager", ContextSourceType.SystemDialog },
            };
            
            // Prozess-Namen
            _processPatterns = new Dictionary<string, ContextSourceType>(StringComparer.OrdinalIgnoreCase)
            {
                // E-Mail
                { "OUTLOOK", ContextSourceType.EmailClient },
                { "THUNDERBIRD", ContextSourceType.EmailClient },
                { "MAIL", ContextSourceType.EmailClient },
                
                // Browser
                { "CHROME", ContextSourceType.Browser },
                { "FIREFOX", ContextSourceType.Browser },
                { "MSEDGE", ContextSourceType.Browser },
                { "OPERA", ContextSourceType.Browser },
                { "BRAVE", ContextSourceType.Browser },
                
                // System
                { "EXPLORER", ContextSourceType.FileExplorer },
                { "NOTEPAD", ContextSourceType.Unknown },
                { "WINWORD", ContextSourceType.Unknown },
                { "EXCEL", ContextSourceType.Unknown },
                { "POWERPNT", ContextSourceType.Unknown },
            };
        }
        
        /// <summary>
        /// Erkennt aktuellen Kontext basierend auf Vordergrund-Fenster
        /// </summary>
        public ContextDetectionResult DetectCurrentContext()
        {
            var result = new ContextDetectionResult
            {
                Timestamp = DateTime.Now
            };
            
            try
            {
                // Get foreground window
                IntPtr hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero)
                {
                    result.DetectedType = ContextSourceType.Unknown;
                    result.Confidence = 0;
                    return result;
                }
                
                // Get window title
                var titleBuilder = new System.Text.StringBuilder(256);
                GetWindowText(hwnd, titleBuilder, titleBuilder.Capacity);
                string windowTitle = titleBuilder.ToString();
                result.WindowTitle = windowTitle;
                
                // Get process name
                GetWindowThreadProcessId(hwnd, out uint processId);
                try
                {
                    var process = Process.GetProcessById((int)processId);
                    result.ProcessName = process.ProcessName;
                }
                catch
                {
                    result.ProcessName = "Unknown";
                }
                
                // Detect context
                result.DetectedType = DetectFromWindowAndProcess(windowTitle, result.ProcessName);
                result.Confidence = CalculateConfidence(windowTitle, result.ProcessName, result.DetectedType);
                
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                result.DetectedType = ContextSourceType.Unknown;
            }
            
            return result;
        }
        
        /// <summary>
        /// Erkennt Kontext aus Text-Inhalten (z.B. aus Screen-OCR)
        /// </summary>
        public ContextSourceType DetectFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return ContextSourceType.Unknown;
            
            var lowerText = text.ToLower();
            
            // E-Mail Indikatoren
            string[] emailKeywords = { 
                "betreff", "an:", "von:", "Cc:", "Bcc:", 
                " Postein", "Gesendet", "Entwürfe", "Papierkorb",
                "答复", "发送", "收到", // Chinese
                "subject:", "to:", "from:", "cc:", 
                "inbox", "sent", "drafts", "trash"
            };
            
            int emailScore = emailKeywords.Count(kw => lowerText.Contains(kw.ToLower()));
            if (emailScore >= 2)
                return ContextSourceType.EmailClient;
            
            // Browser Indikatoren  
            string[] browserKeywords = {
                "http://", "https://", "www.", ".com", ".de", ".org", ".net",
                "suchen", "google", "bing", "suche", "search",
                "tab", "tabs", "browser", "lesezeichen", "favoriten",
                "chrome://", "about:", "file://"
            };
            
            int browserScore = browserKeywords.Count(kw => lowerText.Contains(kw.ToLower()));
            if (browserScore >= 2)
                return ContextSourceType.Browser;
            
            // Popup/Dialog Indikatoren
            string[] popupKeywords = {
                "ok", "abbrechen", "ja", "nein", "cancel", "close",
                "警告", "错误", "注意", // Warning/Error in Chinese
                "warning", "error", "confirm", "attention"
            };
            
            int popupScore = popupKeywords.Count(kw => lowerText.Contains(kw.ToLower()));
            if (popupScore >= 3 && text.Length < 500)
                return ContextSourceType.Popup;
            
            // File Explorer Indikatoren
            string[] explorerKeywords = {
                "desktop", "dokumente", "downloads", "bilder", "musik", "videos",
                "dateien", "ordner", "verzeichnis", "c:\\", "d:\\", "e:\\",
                "this pc", "computer", "documents", "downloads"
            };
            
            int explorerScore = explorerKeywords.Count(kw => lowerText.Contains(kw.ToLower()));
            if (explorerScore >= 2)
                return ContextSourceType.FileExplorer;
            
            return ContextSourceType.Unknown;
        }
        
        private ContextSourceType DetectFromWindowAndProcess(string windowTitle, string processName)
        {
            ContextSourceType result = ContextSourceType.Unknown;
            int bestScore = 0;
            
            // Check window title patterns
            foreach (var pattern in _windowPatterns)
            {
                if (windowTitle.Contains(pattern.Key, StringComparison.OrdinalIgnoreCase))
                {
                    int score = pattern.Key.Length; // Longer matches = higher score
                    if (score > bestScore)
                    {
                        bestScore = score;
                        result = pattern.Value;
                    }
                }
            }
            
            // Check process patterns (lower priority than window title)
            foreach (var pattern in _processPatterns)
            {
                if (processName.Contains(pattern.Key, StringComparison.OrdinalIgnoreCase))
                {
                    int score = pattern.Key.Length / 2;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        result = pattern.Value;
                    }
                }
            }
            
            return result;
        }
        
        private float CalculateConfidence(string windowTitle, string processName, ContextSourceType detectedType)
        {
            if (detectedType == ContextSourceType.Unknown)
                return 0.0f;
            
            float confidence = 0.5f; // Base confidence
            
            // Strong window title match
            foreach (var pattern in _windowPatterns.Where(p => p.Value == detectedType))
            {
                if (windowTitle.Contains(pattern.Key, StringComparison.OrdinalIgnoreCase))
                {
                    confidence = Math.Max(confidence, 0.8f + (pattern.Key.Length * 0.01f));
                }
            }
            
            // Matching process
            foreach (var pattern in _processPatterns.Where(p => p.Value == detectedType))
            {
                if (processName.Contains(pattern.Key, StringComparison.OrdinalIgnoreCase))
                {
                    confidence = Math.Max(confidence, 0.7f);
                }
            }
            
            return Math.Min(1.0f, confidence);
        }
        
        /// <summary>
        /// Gibt kontext-spezifische Empfehlungen für Safety-Checks
        /// </summary>
        public List<string> GetContextSpecificChecks(ContextSourceType context)
        {
            switch (context)
            {
                case ContextSourceType.EmailClient:
                    return new List<string>
                    {
                        "Prüfe Absender-Adresse sorgfältig",
                        "Misstraue unerwarteten Anhängen",
                        "Prüfe Links vor dem Klicken (Hover)",
                        "Bei Geldforderungen: Direkt bei Bank prüfen"
                    };
                    
                case ContextSourceType.Browser:
                    return new List<string>
                    {
                        "Prüfe URL auf HTTPS",
                        "Achte auf Fake-Login-Seiten",
                        "Keine sensiblen Daten auf HTTP-Seiten",
                        "Prüfe Zertifikats-Info (Vorhängeschloss)"
                    };
                    
                case ContextSourceType.Popup:
                    return new List<string>
                    {
                        "Prüfe ob Popup von echtem System kommt",
                        "System-Popups kommen NICHT von Browsern",
                        "Bei Fake-Warnungen: Browser schließen",
                        "Keine Telefonnummern anrufen"
                    };
                    
                case ContextSourceType.SystemDialog:
                    return new List<string>
                    {
                        "System-Dialoge sind vertrauenswürdig",
                        "Prüfe ob Handlung wirklich nötig",
                        "Bei Unsicherheit: Abbrechen wählen"
                    };
                    
                default:
                    return new List<string>
                    {
                        "Allgemeine Wachsamkeit",
                        "Bei Unsicherheit: Nicht klicken",
                        "PC-Begleiter fragen"
                    };
            }
        }
    }
    
    /// <summary>
    /// Ergebnis der Kontext-Erkennung
    /// </summary>
    public class ContextDetectionResult
    {
        public DateTime Timestamp { get; set; }
        public ContextSourceType DetectedType { get; set; }
        public float Confidence { get; set; }
        public string WindowTitle { get; set; }
        public string ProcessName { get; set; }
        public string Error { get; set; }
    }
}
