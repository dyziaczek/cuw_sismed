using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace VS_CUWSISMED
{
    public sealed class RegisterEmployeeDialog : Form
    {
        private readonly bool allowRoleSelection;
        private readonly Guna2TextBox txtLogin;
        private readonly Guna2TextBox txtFirstName;
        private readonly Guna2TextBox txtLastName;
        private readonly Guna2TextBox txtPesel;
        private readonly DateTimePicker dtpBirthDate;
        private readonly ComboBox cmbRole;
        private readonly CheckBox chkDoctor;
        private readonly Guna2TextBox txtSpecialization;
        private readonly Guna2TextBox txtPassword;
        private readonly Guna2TextBox txtRepeatPassword;
        private readonly Guna2Button btnRegister;
        private readonly Guna2Button btnCancel;

        public Employee RegisteredEmployee { get; private set; }

        public RegisterEmployeeDialog()
            : this(false)
        {
        }

        public RegisterEmployeeDialog(bool allowRoleSelection)
        {
            this.allowRoleSelection = allowRoleSelection;

            Text = "Rejestracja pracownika";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(460, 570);
            BackColor = Color.FromArgb(10, 12, 35);

            txtLogin = CreateTextBox("Login", 24);
            txtFirstName = CreateTextBox("Imie", 72);
            txtLastName = CreateTextBox("Nazwisko", 120);
            txtPesel = CreateTextBox("PESEL", 168);

            dtpBirthDate = new DateTimePicker
            {
                Location = new Point(24, 216),
                Size = new Size(392, 28),
                CustomFormat = "dd.MM.yyyy",
                Format = DateTimePickerFormat.Custom,
                Value = DateTime.Today.AddYears(-30)
            };

            cmbRole = new ComboBox
            {
                Location = new Point(24, 264),
                Size = new Size(392, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.Add(EmployeeRoles.Reception);
            cmbRole.Items.Add(EmployeeRoles.Administrator);
            cmbRole.SelectedItem = EmployeeRoles.Reception;
            cmbRole.Enabled = allowRoleSelection;

            chkDoctor = new CheckBox
            {
                Text = "Pracownik jest lekarzem",
                Location = new Point(24, 312),
                Size = new Size(220, 24),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            chkDoctor.CheckedChanged += (sender, args) =>
            {
                txtSpecialization.Enabled = chkDoctor.Checked;
            };

            txtSpecialization = CreateTextBox("Specjalizacja lekarza", 342);
            txtSpecialization.Enabled = false;
            txtPassword = CreateTextBox("Haslo", 390);
            txtRepeatPassword = CreateTextBox("Powtorz haslo", 438);

            txtPassword.PasswordChar = '●';
            txtRepeatPassword.PasswordChar = '●';

            btnRegister = new Guna2Button
            {
                Text = allowRoleSelection ? "Dodaj pracownika" : "Utworz konto",
                Location = new Point(190, 500),
                Size = new Size(142, 34),
                BorderRadius = 8,
                FillColor = Color.FromArgb(0, 130, 110),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnRegister.Click += btnRegister_Click;

            btnCancel = new Guna2Button
            {
                Text = "Anuluj",
                Location = new Point(340, 500),
                Size = new Size(76, 34),
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
                txtLogin, txtFirstName, txtLastName, txtPesel, dtpBirthDate,
                cmbRole, chkDoctor, txtSpecialization, txtPassword, txtRepeatPassword,
                btnRegister, btnCancel
            });
        }

        private Guna2TextBox CreateTextBox(string placeholder, int top)
        {
            return new Guna2TextBox
            {
                Location = new Point(24, top),
                Size = new Size(392, 36),
                PlaceholderText = placeholder,
                Font = new Font("Segoe UI", 9f),
                BorderColor = Color.FromArgb(40, 50, 120),
                FocusedState = { BorderColor = Color.FromArgb(220, 0, 150) },
                BorderThickness = 2
            };
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string role = allowRoleSelection
                ? Convert.ToString(cmbRole.SelectedItem)
                : EmployeeRoles.Reception;

            RegistrationResult result = AppServices.AuthService.RegisterEmployee(
                txtLogin.Text,
                txtFirstName.Text,
                txtLastName.Text,
                txtPesel.Text,
                dtpBirthDate.Value.Date,
                role,
                txtPassword.Text,
                txtRepeatPassword.Text,
                chkDoctor.Checked,
                txtSpecialization.Text);

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
