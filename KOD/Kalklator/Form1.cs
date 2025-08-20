using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kalklator
{
    public class Form1 : Form
    {
        private TextBox display;
        private string currentInput = "";
        private double result = 0;
        private string lastOp = "";
        private bool darkMode = false;

        private Button settingsButton;
        private Button openKalkButton;

        public Form1()
        {
            this.Text = "Kalklator";
            this.ClientSize = new Size(300, 420);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            SetupUI();
        }

        private void SetupUI()
        {
            display = new TextBox();
            display.Font = new Font("Segoe UI", 20);
            display.Location = new Point(10, 10);
            display.Size = new Size(260, 40);
            display.ReadOnly = true;
            display.TextAlign = HorizontalAlignment.Right;
            this.Controls.Add(display);

            string[] buttons = {
                "7", "8", "9", "/",
                "4", "5", "6", "*",
                "1", "2", "3", "-",
                "0", "C", "=", "+"
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                var btn = new Button();
                btn.Text = buttons[i];
                btn.Font = new Font("Segoe UI", 14);
                btn.Size = new Size(60, 50);
                btn.Location = new Point(10 + (i % 4) * 70, 60 + (i / 4) * 60);
                btn.Click += Button_Click;
                this.Controls.Add(btn);
            }

            // Przycisk zębatki na dole po prawej
            settingsButton = new Button();
            settingsButton.Text = "⚙";
            settingsButton.Font = new Font("Segoe UI", 14);
            settingsButton.Size = new Size(40, 40);
            settingsButton.Location = new Point(this.ClientSize.Width - 50, this.ClientSize.Height - 50);
            settingsButton.Click += SettingsButton_Click;
            this.Controls.Add(settingsButton);

            openKalkButton = new Button();
            openKalkButton.Text = "📂";
            openKalkButton.Font = new Font("Segoe UI", 14);
            openKalkButton.Size = new Size(40, 40);
            openKalkButton.Location = new Point(this.ClientSize.Width - 100, this.ClientSize.Height - 50);
            openKalkButton.Click += OpenKalkButton_Click;
            this.Controls.Add(openKalkButton);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            string text = btn.Text;

            if ("0123456789".IndexOf(text) >= 0)
            {
                currentInput += text;
                display.Text = currentInput;
                return;
            }

            if (text == "C")
            {
                currentInput = "";
                lastOp = "";
                result = 0;
                display.Text = "";
                return;
            }

            if (text == "=")
            {
                double parsed;
                if (!double.TryParse(currentInput, out parsed))
                    parsed = 0;

                switch (lastOp)
                {
                    case "+":
                        result += parsed;
                        break;
                    case "-":
                        result -= parsed;
                        break;
                    case "*":
                        result *= parsed;
                        break;
                    case "/":
                        result = (parsed != 0) ? result / parsed : 0;
                        break;
                    default:
                        result = parsed;
                        break;
                }

                var rand = new Random();
                var fail = rand.NextDouble() * 5 - 2.5; // -2.5 do +2.5
                result += fail;

                display.Text = Math.Round(result, 2).ToString();
                currentInput = "";
                lastOp = "";
                return;
            }

            if (currentInput != "")
            {
                double parsed;
                if (!double.TryParse(currentInput, out parsed))
                    parsed = 0;

                if (lastOp == "")
                    result = parsed;
                else
                {
                    switch (lastOp)
                    {
                        case "+":
                            result += parsed;
                            break;
                        case "-":
                            result -= parsed;
                            break;
                        case "*":
                            result *= parsed;
                            break;
                        case "/":
                            result = (parsed != 0) ? result / parsed : 0;
                            break;
                    }
                }
            }

            lastOp = text;
            currentInput = "";
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            // Tworzymy i pokazujemy okno ustawień
            var settingsForm = new SettingsForm(darkMode);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                darkMode = settingsForm.DarkModeEnabled;
                ApplyTheme();
            }
        }

        private void ApplyTheme()
        {
            this.BackColor = darkMode ? Color.FromArgb(30, 30, 30) : SystemColors.Control;

            foreach (Control c in this.Controls)
            {
                var btn = c as Button;
                if (btn != null)
                {
                    btn.BackColor = darkMode ? Color.DimGray : SystemColors.ControlLight;
                    btn.ForeColor = darkMode ? Color.White : Color.Black;
                }

                var tb = c as TextBox;
                if (tb != null)
                {
                    tb.BackColor = darkMode ? Color.Black : Color.White;
                    tb.ForeColor = darkMode ? Color.Lime : Color.Black;
                }
            }
        }

        private void OpenKalkButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Pliki kalk (*.kalk)|*.kalk|Wszystkie pliki (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string contents = System.IO.File.ReadAllText(ofd.FileName).Trim();

                    // Resetuj stan
                    currentInput = "";
                    lastOp = "";
                    result = 0;

                    // Spróbuj obliczyć działanie
                    double evalResult = ObliczDzialanie(contents);

                    // Dodaj przekłamanie jak w "="
                    var rand = new Random();
                    var fail = rand.NextDouble() * 5 - 2.5; // -2.5 do +2.5
                    evalResult += fail;

                    display.Text = Math.Round(evalResult, 2).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas wczytywania pliku:\n" + ex.Message);
                }
            }
        }
        private double ObliczDzialanie(string expression)
        {
            try
            {
                // Użyj DataTable do prostego eval
                var dt = new System.Data.DataTable();
                var resultObj = dt.Compute(expression, "");
                return Convert.ToDouble(resultObj);
            }
            catch
            {
                return 0;
            }
        }
        public Form1(string importedExpression) : this()
        {
            if (!string.IsNullOrWhiteSpace(importedExpression))
            {
                try
                {
                    double evalResult = ObliczDzialanie(importedExpression);

                    // Dodaj losowe przekłamanie
                    var rand = new Random();
                    var fail = rand.NextDouble() * 5 - 2.5;
                    evalResult += fail;

                    display.Text = Math.Round(evalResult, 2).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd przy ładowaniu pliku .kalk:\n" + ex.Message);
                }
            }
        }
    }
}