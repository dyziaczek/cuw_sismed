using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public sealed class PatientEditDialog : Form
    {
        private readonly Patient sourcePatient;
        private readonly TextBox txtFirstName;
        private readonly TextBox txtLastName;
        private readonly TextBox txtPesel;
        private readonly TextBox txtBirthDate;
        private readonly TextBox txtPhone;
        private readonly TextBox txtEmail;
        private readonly TextBox txtCity;
        private readonly TextBox txtPostalCode;
        private readonly TextBox txtStreet;
        private readonly TextBox txtHouseNumber;
        private readonly TextBox txtApartmentNumber;
        private readonly Button btnSave;
        private readonly Button btnCancel;

        public Patient Patient { get; private set; }

        public PatientEditDialog(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException("patient");
            }

            sourcePatient = patient;

            Text = "Edycja danych pacjenta";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(540, 680);
            MinimumSize = new Size(480, 560);
            BackColor = SismedTheme.Surface;
            Shown += (sender, args) => SismedTheme.FitFormToWorkingArea(this);

            var title = new Label
            {
                Text = "Dane pacjenta",
                Location = new Point(24, 18),
                Size = new Size(460, 28),
                Font = SismedTheme.Font(13f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };

            txtFirstName = CreateLabeledTextBox("Imię", 24, 62, 230);
            txtLastName = CreateLabeledTextBox("Nazwisko", 282, 62, 230);
            txtPesel = CreateLabeledTextBox("PESEL", 24, 132, 230);
            txtBirthDate = CreateLabeledTextBox("Data urodzenia dd-mm-yyyy", 282, 132, 230);
            txtPhone = CreateLabeledTextBox("Numer telefonu", 24, 202, 230);
            txtEmail = CreateLabeledTextBox("Adres e-mail", 282, 202, 230);

            var addressTitle = new Label
            {
                Text = "Adres pacjenta",
                Location = new Point(24, 282),
                Size = new Size(460, 24),
                Font = SismedTheme.Font(11f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };

            txtCity = CreateLabeledTextBox("Miasto", 24, 324, 230);
            txtPostalCode = CreateLabeledTextBox("Kod pocztowy", 282, 324, 230);
            txtStreet = CreateLabeledTextBox("Ulica", 24, 394, 488);
            txtHouseNumber = CreateLabeledTextBox("Numer domu", 24, 464, 230);
            txtApartmentNumber = CreateLabeledTextBox("Numer lokalu", 282, 464, 230);

            btnSave = new Button
            {
                Text = "Zapisz",
                Location = new Point(320, 610),
                Size = new Size(92, 36),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            SismedTheme.ApplyPrimaryButton(btnSave);
            btnSave.Click += btnSave_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(420, 610),
                Size = new Size(92, 36),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            SismedTheme.ApplySecondaryButton(btnCancel);
            btnCancel.Click += (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            ConfigureInputLimits();
            LoadPatient();

            Controls.AddRange(new Control[]
            {
                title,
                txtFirstName, txtLastName, txtPesel, txtBirthDate, txtPhone, txtEmail,
                addressTitle,
                txtCity, txtPostalCode, txtStreet, txtHouseNumber, txtApartmentNumber,
                btnSave, btnCancel
            });
        }

        private TextBox CreateLabeledTextBox(string label, int left, int top, int width)
        {
            var caption = new Label
            {
                Text = label,
                Location = new Point(left, top),
                Size = new Size(width, 18),
                Font = SismedTheme.Font(8.5f, FontStyle.Bold),
                ForeColor = SismedTheme.Muted
            };
            Controls.Add(caption);

            var textBox = new TextBox
            {
                Location = new Point(left, top + 22),
                Size = new Size(width, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            SismedTheme.ApplyTextBox(textBox, string.Empty);
            return textBox;
        }

        private void ConfigureInputLimits()
        {
            txtPesel.MaxLength = 11;
            txtPhone.MaxLength = 9;
            txtBirthDate.MaxLength = 10;
            txtPostalCode.MaxLength = 6;
            txtPesel.KeyPress += DigitsOnly_KeyPress;
            txtPhone.KeyPress += DigitsOnly_KeyPress;
            txtBirthDate.KeyPress += Date_KeyPress;
            txtBirthDate.TextChanged += DateTextBox_TextChanged;
            txtPostalCode.KeyPress += PostalCode_KeyPress;
            txtPostalCode.TextChanged += PostalCodeTextBox_TextChanged;
        }

        private void LoadPatient()
        {
            string[] address = SplitAddress(sourcePatient.Address);
            txtFirstName.Text = sourcePatient.FirstName;
            txtLastName.Text = sourcePatient.LastName;
            txtPesel.Text = sourcePatient.Pesel;
            txtBirthDate.Text = sourcePatient.BirthDate.HasValue
                ? sourcePatient.BirthDate.Value.ToString("dd-MM-yyyy")
                : string.Empty;
            txtPhone.Text = sourcePatient.Phone;
            txtEmail.Text = sourcePatient.Email;
            txtCity.Text = address[0];
            txtPostalCode.Text = address[1];
            txtStreet.Text = address[2];
            txtHouseNumber.Text = address[3];
            txtApartmentNumber.Text = address[4];
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string city = txtCity.Text.Trim();
            string street = txtStreet.Text.Trim();
            string houseNumber = txtHouseNumber.Text.Trim();
            string postalCode = txtPostalCode.Text.Trim();

            if (!InputValidation.IsName(firstName) || !InputValidation.IsName(lastName))
            {
                ShowValidation("Imię i nazwisko są wymagane i mogą zawierać tylko litery, spacje i myślnik.");
                return;
            }

            if (!InputValidation.IsFullPesel(txtPesel.Text))
            {
                ShowValidation("PESEL musi składać się dokładnie z 11 cyfr.");
                return;
            }

            DateTime birthDate;
            if (!DateTime.TryParseExact(
                txtBirthDate.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out birthDate))
            {
                ShowValidation("Data urodzenia musi mieć format dd-mm-yyyy.");
                return;
            }

            if (!InputValidation.IsPhone(txtPhone.Text))
            {
                ShowValidation("Telefon może zawierać tylko cyfry i maksymalnie 9 znaków.");
                return;
            }

            if (!InputValidation.IsOptionalEmail(txtEmail.Text))
            {
                ShowValidation("Podaj poprawny adres e-mail.");
                return;
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                ShowValidation("Miasto jest wymagane.");
                return;
            }

            if (!InputValidation.IsPostalCode(postalCode))
            {
                ShowValidation("Kod pocztowy musi mieć format XX-XXX.");
                return;
            }

            if (string.IsNullOrWhiteSpace(street))
            {
                ShowValidation("Ulica jest wymagana.");
                return;
            }

            if (string.IsNullOrWhiteSpace(houseNumber))
            {
                ShowValidation("Numer domu jest wymagany.");
                return;
            }

            Patient = new Patient
            {
                Id = sourcePatient.Id,
                FirstName = firstName,
                LastName = lastName,
                Pesel = txtPesel.Text.Trim(),
                BirthDate = birthDate.Date,
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = ComposeAddress(),
                Notes = sourcePatient.Notes,
                WarningCount = sourcePatient.WarningCount,
                BlockedUntil = sourcePatient.BlockedUntil
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private string ComposeAddress()
        {
            return string.Join(", ", new[]
            {
                txtCity.Text.Trim(),
                txtPostalCode.Text.Trim(),
                txtStreet.Text.Trim(),
                txtHouseNumber.Text.Trim(),
                txtApartmentNumber.Text.Trim()
            });
        }

        private static string[] SplitAddress(string address)
        {
            string[] result = new string[5];
            string[] parts = (address ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.None);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = i < parts.Length ? parts[i].Trim() : string.Empty;
            }

            return result;
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

        private static void PostalCode_KeyPress(object sender, KeyPressEventArgs e)
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

        private static void PostalCodeTextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
            {
                return;
            }

            string digits = new string(textBox.Text.Where(char.IsDigit).Take(5).ToArray());
            string formatted = digits.Length <= 2
                ? digits
                : digits.Substring(0, 2) + "-" + digits.Substring(2);
            if (textBox.Text == formatted)
            {
                return;
            }

            textBox.Text = formatted;
            textBox.SelectionStart = formatted.Length;
        }

        private static void ShowValidation(string message)
        {
            MessageBox.Show(message, "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
