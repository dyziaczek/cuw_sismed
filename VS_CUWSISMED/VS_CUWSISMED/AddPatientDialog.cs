using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public sealed class AddPatientDialog : Form
    {
        private readonly TextBox txtFirstName;
        private readonly TextBox txtLastName;
        private readonly TextBox txtPesel;
        private readonly TextBox txtBirthDate;
        private readonly TextBox txtPhone;
        private readonly TextBox txtEmail;
        private readonly Button btnSave;
        private readonly Button btnCancel;

        public Patient Patient { get; private set; }

        public AddPatientDialog()
        {
            Text = "Dodaj pacjenta";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(420, 360);
            MinimumSize = new Size(380, 340);
            BackColor = SismedTheme.Surface;
            Shown += (sender, args) => SismedTheme.FitFormToWorkingArea(this);

            txtFirstName = CreateTextBox("Imie", 24);
            txtLastName = CreateTextBox("Nazwisko", 72);
            txtPesel = CreateTextBox("PESEL", 120);
            txtBirthDate = CreateTextBox("Data urodzenia dd-MM-yyyy", 168);
            txtPhone = CreateTextBox("Telefon", 216);
            txtEmail = CreateTextBox("E-mail", 264);
            ConfigureInputLimits();

            btnSave = new Button
            {
                Text = "Zapisz",
                Location = new Point(210, 314),
                Size = new Size(84, 32),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            SismedTheme.ApplySuccessButton(btnSave);
            btnSave.Click += btnSave_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(304, 314),
                Size = new Size(84, 32),
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
                txtFirstName, txtLastName, txtPesel, txtBirthDate, txtPhone, txtEmail,
                btnSave, btnCancel
            });
        }

        private void ConfigureInputLimits()
        {
            txtPesel.MaxLength = 11;
            txtPhone.MaxLength = 9;
            txtBirthDate.MaxLength = 10;
            txtPesel.KeyPress += DigitsOnly_KeyPress;
            txtPhone.KeyPress += DigitsOnly_KeyPress;
            txtBirthDate.KeyPress += Date_KeyPress;
            txtBirthDate.TextChanged += DateTextBox_TextChanged;
        }

        private TextBox CreateTextBox(string placeholder, int top)
        {
            var textBox = new TextBox
            {
                Location = new Point(24, top),
                Size = new Size(364, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            SismedTheme.ApplyTextBox(textBox, placeholder);
            return textBox;
        }

        private static void DigitsOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private static void Date_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private static void DateTextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            string digits = new string(textBox.Text.Where(char.IsDigit).Take(8).ToArray());
            string formatted = FormatBirthDateInput(digits);
            if (textBox.Text == formatted)
            {
                return;
            }

            textBox.Text = formatted;
            textBox.SelectionStart = formatted.Length;
        }

        private static string FormatBirthDateInput(string digits)
        {
            if (digits.Length <= 2)
            {
                return digits;
            }

            if (digits.Length <= 4)
            {
                return digits.Substring(0, 2) + "-" + digits.Substring(2);
            }

            return digits.Substring(0, 2) + "-"
                + digits.Substring(2, 2) + "-"
                + digits.Substring(4);
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
                Address = string.Empty,
                Notes = string.Empty,
                WarningCount = 0
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
