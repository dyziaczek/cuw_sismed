using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace VS_CUWSISMED
{
    public sealed class AddPatientDialog : Form
    {
        private readonly Guna2TextBox txtFirstName;
        private readonly Guna2TextBox txtLastName;
        private readonly Guna2TextBox txtPesel;
        private readonly Guna2TextBox txtBirthDate;
        private readonly Guna2TextBox txtPhone;
        private readonly Guna2TextBox txtEmail;
        private readonly Guna2TextBox txtAddress;
        private readonly Guna2TextBox txtNotes;
        private readonly Guna2Button btnSave;
        private readonly Guna2Button btnCancel;

        public Patient Patient { get; private set; }

        public AddPatientDialog()
        {
            Text = "Dodaj pacjenta";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(420, 460);
            BackColor = SismedTheme.Surface;

            txtFirstName = CreateTextBox("Imie", 24);
            txtLastName = CreateTextBox("Nazwisko", 72);
            txtPesel = CreateTextBox("PESEL", 120);
            txtBirthDate = CreateTextBox("Data urodzenia dd.MM.yyyy", 168);
            txtPhone = CreateTextBox("Telefon", 216);
            txtEmail = CreateTextBox("E-mail", 264);
            txtAddress = CreateTextBox("Adres", 312);
            txtNotes = CreateTextBox("Notatka pacjenta", 360);

            btnSave = new Guna2Button
            {
                Text = "Zapisz",
                Location = new Point(210, 414),
                Size = new Size(84, 32),
                BorderRadius = 8,
                ForeColor = Color.White,
            };
            SismedTheme.ApplySuccessButton(btnSave);
            btnSave.Click += btnSave_Click;

            btnCancel = new Guna2Button
            {
                Text = "Anuluj",
                Location = new Point(304, 414),
                Size = new Size(84, 32),
                BorderRadius = 8,
                ForeColor = Color.White,
            };
            SismedTheme.ApplySecondaryButton(btnCancel);
            btnCancel.Click += (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.AddRange(new Control[]
            {
                txtFirstName, txtLastName, txtPesel, txtBirthDate, txtPhone, txtEmail, txtAddress, txtNotes,
                btnSave, btnCancel
            });
        }

        private Guna2TextBox CreateTextBox(string placeholder, int top)
        {
            var textBox = new Guna2TextBox
            {
                Location = new Point(24, top),
                Size = new Size(364, 36),
                PlaceholderText = placeholder,
                BorderThickness = 2
            };

            SismedTheme.ApplyTextBox(textBox);
            return textBox;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string pesel = txtPesel.Text.Trim();
            DateTime? birthDate;

            if (!InputValidation.IsName(firstName) || !InputValidation.IsName(lastName))
            {
                MessageBox.Show("Imie i nazwisko moga zawierac tylko litery, spacje i myslnik.", "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!InputValidation.IsFullPesel(pesel))
            {
                MessageBox.Show("PESEL musi skladac sie z 11 cyfr.", "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!InputValidation.TryParseBirthDate(txtBirthDate.Text, out birthDate))
            {
                MessageBox.Show("Data urodzenia musi miec format dd.MM.yyyy albo dd-MM-yyyy.", "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!InputValidation.IsPhone(txtPhone.Text))
            {
                MessageBox.Show("Telefon moze zawierac tylko cyfry i maksymalnie 9 znakow.", "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!InputValidation.IsOptionalEmail(txtEmail.Text))
            {
                MessageBox.Show("Podaj poprawny adres e-mail.", "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Patient = new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                Pesel = pesel,
                BirthDate = birthDate,
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Notes = txtNotes.Text.Trim(),
                WarningCount = 0
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
