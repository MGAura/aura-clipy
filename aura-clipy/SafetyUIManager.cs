using System;
using System.Drawing;
using System.Windows.Forms;

namespace AuraClipy
{
    /// <summary>
    /// UI Manager für Sicherheitshinweise im PC-Begleiter
    /// Handelt Risk Color Coding, Safety Recommendations und Transparenz-Anzeige
    /// </summary>
    public class SafetyUIManager
    {
        private readonly SimpleRuleEngine _ruleEngine;
        private Form _mainForm;
        private Label _safetyStatusLabel;
        private Panel _safetyPanel;
        private RichTextBox _recommendationBox;
        private Button _detailsButton;
        
        // Colors for risk levels
        private readonly Color _colorNone = Color.FromArgb(240, 240, 240); // Light gray
        private readonly Color _colorLow = Color.FromArgb(255, 255, 200); // Light yellow
        private readonly Color _colorMedium = Color.FromArgb(255, 200, 100); // Orange
        private readonly Color _colorHigh = Color.FromArgb(255, 150, 150); // Light red
        
        public SafetyUIManager(Form mainForm, SimpleRuleEngine ruleEngine)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
            
            InitializeSafetyUI();
        }
        
        private void InitializeSafetyUI()
        {
            // Create safety panel
            _safetyPanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(_mainForm.ClientSize.Width - 20, 120),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = _colorNone,
                Visible = false // Initially hidden
            };
            
            // Safety status label
            _safetyStatusLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(_safetyPanel.Width - 20, 30),
                Text = "Sicherheitsanalyse: Bereit",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            // Recommendation text box
            _recommendationBox = new RichTextBox
            {
                Location = new Point(10, 45),
                Size = new Size(_safetyPanel.Width - 20, 50),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = _safetyPanel.BackColor,
                Font = new Font("Segoe UI", 9),
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.None
            };
            
            // Details button
            _detailsButton = new Button
            {
                Location = new Point(_safetyPanel.Width - 110, 100),
                Size = new Size(100, 20),
                Text = "Details anzeigen",
                Visible = false
            };
            _detailsButton.Click += DetailsButton_Click;
            
            // Add controls to panel
            _safetyPanel.Controls.Add(_safetyStatusLabel);
            _safetyPanel.Controls.Add(_recommendationBox);
            _safetyPanel.Controls.Add(_detailsButton);
            
            // Add panel to main form
            _mainForm.Controls.Add(_safetyPanel);
            _safetyPanel.BringToFront();
        }
        
        /// <summary>
        /// Zeigt Sicherheitsanalyse-Ergebnis in der UI an
        /// </summary>
        public void ShowSafetyAnalysis(SecurityAnalysisResult analysis, SafetyRecommendation recommendation)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(() => ShowSafetyAnalysis(analysis, recommendation)));
                return;
            }
            
            _safetyPanel.Visible = true;
            
            // Update colors based on risk level
            UpdateRiskColors(analysis.RiskLevel);
            
            // Update status text
            _safetyStatusLabel.Text = $"Sicherheitsanalyse: {GetRiskLevelText(analysis.RiskLevel)} ({analysis.OverallRiskScore}/10)";
            
            // Update recommendation
            _recommendationBox.Text = recommendation.RecommendationText;
            
            // Show details button for medium/high risk
            _detailsButton.Visible = analysis.RiskLevel >= RiskLevel.Medium;
            
            // Auto-hide for low/none risk after delay
            if (analysis.RiskLevel <= RiskLevel.Low)
            {
                var timer = new Timer { Interval = 5000 };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    HideSafetyPanel();
                };
                timer.Start();
            }
        }
        
        /// <summary>
        /// Zeigt "Analysiere Sicherheit..." Status an
        /// </summary>
        public void ShowAnalyzingStatus(string context)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(() => ShowAnalyzingStatus(context)));
                return;
            }
            
            _safetyPanel.Visible = true;
            _safetyPanel.BackColor = Color.FromArgb(200, 200, 255); // Light blue for analysis
            _safetyStatusLabel.Text = $"🔍 Analysiere Sicherheit: {context}...";
            _recommendationBox.Text = "Bildschirm wird analysiert auf Risikomuster...";
            _detailsButton.Visible = false;
        }
        
        /// <summary>
        /// Versteckt das Safety-Panel
        /// </summary>
        public void HideSafetyPanel()
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(HideSafetyPanel));
                return;
            }
            
            _safetyPanel.Visible = false;
        }
        
        /// <summary>
        /// Zeigt Transparenz-Info an (lokal vs. cloud Verarbeitung)
        /// </summary>
        public void ShowTransparencyInfo(string processingType, bool isLocal)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(() => ShowTransparencyInfo(processingType, isLocal)));
                return;
            }
            
            // Create or update transparency label
            var transparencyLabel = _mainForm.Controls.Find("transparencyLabel", false);
            Label label;
            
            if (transparencyLabel.Length > 0)
            {
                label = (Label)transparencyLabel[0];
            }
            else
            {
                label = new Label
                {
                    Name = "transparencyLabel",
                    Location = new Point(10, _mainForm.ClientSize.Height - 30),
                    Size = new Size(200, 20),
                    Font = new Font("Segoe UI", 8),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _mainForm.Controls.Add(label);
                label.BringToFront();
            }
            
            string icon = isLocal ? "🔒" : "☁️";
            string text = isLocal ? "Lokal" : "Cloud";
            label.Text = $"{icon} {processingType}: {text}";
            label.ForeColor = isLocal ? Color.DarkGreen : Color.DarkBlue;
            label.Visible = true;
            
            // Auto-hide after 3 seconds
            var timer = new Timer { Interval = 3000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                label.Visible = false;
            };
            timer.Start();
        }
        
        private void UpdateRiskColors(RiskLevel riskLevel)
        {
            Color panelColor;
            Color textColor = Color.Black;
            
            switch (riskLevel)
            {
                case RiskLevel.None:
                    panelColor = _colorNone;
                    break;
                case RiskLevel.Low:
                    panelColor = _colorLow;
                    break;
                case RiskLevel.Medium:
                    panelColor = _colorMedium;
                    textColor = Color.White;
                    break;
                case RiskLevel.High:
                    panelColor = _colorHigh;
                    textColor = Color.White;
                    break;
                default:
                    panelColor = _colorNone;
                    break;
            }
            
            _safetyPanel.BackColor = panelColor;
            _recommendationBox.BackColor = panelColor;
            _recommendationBox.ForeColor = textColor;
            _safetyStatusLabel.ForeColor = textColor;
        }
        
        private string GetRiskLevelText(RiskLevel riskLevel)
        {
            switch (riskLevel)
            {
                case RiskLevel.None: return "Kein Risiko";
                case RiskLevel.Low: return "Geringes Risiko";
                case RiskLevel.Medium: return "Mittleres Risiko";
                case RiskLevel.High: return "Hohes Risiko";
                default: return "Unbekannt";
            }
        }
        
        private void DetailsButton_Click(object sender, EventArgs e)
        {
            ShowDetailedSafetyDialog();
        }
        
        private void ShowDetailedSafetyDialog()
        {
            // Create detailed dialog with all detected threats
            var dialog = new Form
            {
                Text = "Detaillierte Sicherheitsanalyse",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            
            var textBox = new RichTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(dialog.ClientSize.Width - 20, dialog.ClientSize.Height - 50),
                ReadOnly = true,
                Text = "Detaillierte Sicherheitsinformationen werden hier angezeigt.\n\n" +
                       "Diese Funktion wird in späteren Versionen erweitert."
            };
            
            var closeButton = new Button
            {
                Text = "Schließen",
                Location = new Point(dialog.ClientSize.Width - 90, dialog.ClientSize.Height - 35),
                Size = new Size(80, 25)
            };
            closeButton.Click += (s, e) => dialog.Close();
            
            dialog.Controls.Add(textBox);
            dialog.Controls.Add(closeButton);
            
            dialog.ShowDialog(_mainForm);
        }
        
        /// <summary>
        /// Passt UI-Größe an wenn MainForm resized wird
        /// </summary>
        public void UpdateLayout()
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(UpdateLayout));
                return;
            }
            
            if (_safetyPanel != null)
            {
                _safetyPanel.Width = _mainForm.ClientSize.Width - 20;
                _recommendationBox.Width = _safetyPanel.Width - 20;
                _detailsButton.Left = _safetyPanel.Width - 110;
            }
            
            // Update transparency label position
            var transparencyLabel = _mainForm.Controls.Find("transparencyLabel", false);
            if (transparencyLabel.Length > 0)
            {
                ((Label)transparencyLabel[0]).Top = _mainForm.ClientSize.Height - 30;
            }
        }
        
        /// <summary>
        /// Einfache Methode um Safety-Check für Text durchzuführen und anzuzeigen
        /// </summary>
        public void QuickSafetyCheck(string text, string context)
        {
            ShowAnalyzingStatus(context);
            
            // Simulate analysis delay
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                
                var analysis = _ruleEngine.AnalyzeText(text, context);
                var recommendation = _ruleEngine.GetRecommendation(analysis);
                
                ShowSafetyAnalysis(analysis, recommendation);
                
                // Show transparency info (simulate local processing)
                ShowTransparencyInfo("Sicherheitsanalyse", true);
            };
            timer.Start();
        }
    }
}