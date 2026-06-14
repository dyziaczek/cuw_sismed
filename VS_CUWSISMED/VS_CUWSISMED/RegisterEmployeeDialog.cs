using System;
using System.Drawing;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public sealed class RegisterEmployeeDialog : Form
    {
        private readonly bool allowRoleSelection;
        private readonly TextBox txtLogin;
        private readonly TextBox txtFirstName;
        private readonly TextBox txtLastName;
        private readonly TextBox txtPesel;
        private readonly DateTimePicker dtpBirthDate;
        private readonly ComboBox cmbRole;
        private readonly CheckBox chkDoctor;
        private readonly TextBox txtSpecialization;
        private readonly TextBox txtPassword;
        private readonly TextBox txtRepeatPassword;
        private readonly Button btnRegister;
        private readonly Button btnCancel;

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
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(460, 570);
            MinimumSize = new Size(400, 500);
            BackColor = SismedTheme.Surface;
            Shown += (sender, args) => SismedTheme.FitFormToWorkingArea(this);

            txtLogin = CreateTextBox("Login", 24);
            txtFirstName = CreateTextBox("Imie", 72);
            txtLastName = CreateTextBox("Nazwisko", 120);
            txtPesel = CreateTextBox("PESEL", 168);

            dtpBirthDate = new DateTimePicker
            {
                Location = new Point(24, 216),
                Size = new Size(392, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                CustomFormat = "dd.MM.yyyy",
                Format = DateTimePickerFormat.Custom,
                Value = DateTime.Today.AddYears(-30),
                Font = SismedTheme.Font(9f)
            };

            cmbRole = new ComboBox
            {
                Location = new Point(24, 264),
                Size = new Size(392, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = SismedTheme.Font(9f)
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
                ForeColor = SismedTheme.Text,
                Font = SismedTheme.Font(9f),
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

            btnRegister = new Button
            {
                Text = allowRoleSelection ? "Dodaj pracownika" : "Utworz konto",
                Location = new Point(190, 500),
                Size = new Size(142, 34),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            SismedTheme.ApplySuccessButton(btnRegister);
            btnRegister.Click += btnRegister_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(340, 500),
                Size = new Size(76, 34),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            SismedTheme.ApplySecondaryButton(btnCancel);
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

        private TextBox CreateTextBox(string placeholder, int top)
        {
            var textBox = new TextBox
            {
                Location = new Point(24, top),
                Size = new Size(392, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            SismedTheme.ApplyTextBox(textBox, placeholder);
            return textBox;
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
