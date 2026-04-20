namespace AuraClipy;

public partial class Form1 : Form
{
    private SimpleRuleEngine _ruleEngine;
    private SafetyUIManager _safetyUiManager;
    private Button _testSafetyButton;
    private TextBox _testInputBox;
    private ComboBox _contextComboBox;
    
    public Form1()
    {
        InitializeComponent();
        InitializeSafetyComponents();
        InitializeTestUI();
    }
    
    private void InitializeSafetyComponents()
    {
        _ruleEngine = new SimpleRuleEngine();
        _safetyUiManager = new SafetyUIManager(this, _ruleEngine);
        
        // Handle form resize for safety UI
        this.Resize += (sender, e) => _safetyUiManager?.UpdateLayout();
    }
    
    private void InitializeTestUI()
    {
        this.Text = "PC-Begleiter Safety Test";
        this.Size = new Size(600, 400);
        
        // Test input box
        _testInputBox = new TextBox
        {
            Location = new Point(20, 150),
            Size = new Size(400, 100),
            Multiline = true,
            Text = "Geben Sie Text zur Sicherheitsanalyse ein...\n\nBeispiel für Risiko: \"Ihr Account wird sofort gesperrt wenn Sie nicht jetzt bezahlen! Klicken Sie hier: bit.ly/fake-link\""
        };
        
        // Context selector
        _contextComboBox = new ComboBox
        {
            Location = new Point(430, 150),
            Size = new Size(150, 25),
            Items = { "E-Mail", "Browser", "Popup", "Allgemein" },
            SelectedIndex = 0
        };
        
        // Test safety button
        _testSafetyButton = new Button
        {
            Location = new Point(20, 260),
            Size = new Size(150, 30),
            Text = "Sicherheit analysieren"
        };
        _testSafetyButton.Click += TestSafetyButton_Click;
        
        // Local processing info button
        var localInfoButton = new Button
        {
            Location = new Point(180, 260),
            Size = new Size(150, 30),
            Text = "Lokal vs. Cloud Info"
        };
        localInfoButton.Click += LocalInfoButton_Click;
        
        // Add controls
        this.Controls.AddRange(new Control[] { 
            _testInputBox, 
            _contextComboBox, 
            _testSafetyButton,
            localInfoButton
        });
        
        // Add label
        var label = new Label
        {
            Location = new Point(20, 120),
            Size = new Size(400, 20),
            Text = "PC-Begleiter Safety-Layer Test (Prototyp)",
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        this.Controls.Add(label);
    }
    
    private void TestSafetyButton_Click(object sender, EventArgs e)
    {
        string text = _testInputBox.Text;
        string context = _contextComboBox.SelectedItem?.ToString() ?? "Allgemein";
        
        if (string.IsNullOrWhiteSpace(text))
        {
            MessageBox.Show("Bitte Text eingeben zur Analyse.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        _safetyUiManager.QuickSafetyCheck(text, context);
    }
    
    private void LocalInfoButton_Click(object sender, EventArgs e)
    {
        // Show different transparency modes
        _safetyUiManager.ShowTransparencyInfo("Sicherheitsanalyse", true);
        
        // After delay, show cloud mode
        var timer = new Timer { Interval = 2000 };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            timer.Dispose();
            _safetyUiManager.ShowTransparencyInfo("Bildanalyse", false);
        };
        timer.Start();
    }
}

