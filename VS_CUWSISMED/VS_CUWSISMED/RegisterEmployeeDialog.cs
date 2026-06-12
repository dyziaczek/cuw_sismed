using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace VS_CUWSISMED
{
    public sealed class RegisterEmployeeDialog : Form
    {
        private readonly Guna2TextBox txtLogin;
        private readonly Guna2TextBox txtDisplayName;
        private readonly Guna2TextBox txtRole;
        private readonly Guna2TextBox txtPassword;
        private readonly Guna2TextBox txtRepeatPassword;
        private readonly Guna2Button btnRegister;
        private readonly Guna2Button btnCancel;

        public Employee RegisteredEmployee { get; private set; }

        public RegisterEmployeeDialog()
        {
            Text = "Rejestracja pracownika";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 360);
            BackColor = Color.FromArgb(10, 12, 35);

            txtLogin = CreateTextBox("Login", 24);
            txtDisplayName = CreateTextBox("Imie i nazwisko", 72);
            txtRole = CreateTextBox("Rola", 120);
            txtPassword = CreateTextBox("Haslo", 168);
            txtRepeatPassword = CreateTextBox("Powtorz haslo", 216);

            txtRole.Text = "Rejestracja";
            txtPassword.PasswordChar = '●';
            txtRepeatPassword.PasswordChar = '●';

            btnRegister = new Guna2Button
            {
                Text = "Utworz konto",
                Location = new Point(190, 286),
                Size = new Size(112, 34),
                BorderRadius = 8,
                FillColor = Color.FromArgb(0, 130, 110),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnRegister.Click += btnRegister_Click;

            btnCancel = new Guna2Button
            {
                Text = "Anuluj",
                Location = new Point(310, 286),
                Size = new Size(78, 34),
                BorderRadius = 8,
                FillColor = Color.FromArgb(50, 55, 85),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnCancel.Click += (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.AddRange(new Control[]
            {
                txtLogin, txtDisplayName, txtRole, txtPassword, txtRepeatPassword,
                btnRegister, btnCancel
            });
        }

        private Guna2TextBox CreateTextBox(string placeholder, int top)
        {
            return new Guna2TextBox
            {
                Location = new Point(24, top),
                Size = new Size(364, 36),
                PlaceholderText = placeholder,
                Font = new Font("Segoe UI", 9f),
                BorderColor = Color.FromArgb(40, 50, 120),
                FocusedState = { BorderColor = Color.FromArgb(220, 0, 150) },
                BorderThickness = 2
            };
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegistrationResult result = AppServices.AuthService.RegisterEmployee(
                txtLogin.Text,
                txtDisplayName.Text,
                txtRole.Text,
                txtPassword.Text,
                txtRepeatPassword.Text);

            if (!result.Success)
            {
                MessageBox.Show(result.Message, "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RegisteredEmployee = result.Employee;
            MessageBox.Show(result.Message, "SISMED",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
