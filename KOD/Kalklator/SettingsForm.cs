using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kalklator
{
    public partial class SettingsForm : Form
    {
        private CheckBox chkDarkMode;
        public bool DarkModeEnabled { get; private set; }

        public SettingsForm(bool currentDarkMode)
        {
            this.Text = "Ustawienia";
            this.ClientSize = new Size(250, 120);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            chkDarkMode = new CheckBox
            {
                Text = "Tryb ciemny",
                Checked = currentDarkMode,
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(chkDarkMode);

            var btnOk = new Button
            {
                Text = "OK",
                Location = new Point(40, 60),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };
            this.Controls.Add(btnOk);

            var btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(130, 60),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            this.FormClosing += SettingsForm_FormClosing;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                DarkModeEnabled = chkDarkMode.Checked;
            }
        }
    }
}