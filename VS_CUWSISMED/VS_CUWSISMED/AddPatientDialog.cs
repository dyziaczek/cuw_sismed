using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace VS_CUWSISMED
{
    public sealed class AddPatientDialog : Form
    {
        private readonly Guna2TextBox txtFirstName;
        private readonly Guna2TextBox txtLastName;
        private readonly Guna2TextBox txtPesel;
        private readonly Guna2TextBox txtPhone;
        private readonly Guna2TextBox txtEmail;
        private readonly Guna2TextBox txtAddress;
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
            ClientSize = new Size(420, 360);
            BackColor = Color.FromArgb(10, 12, 35);

            txtFirstName = CreateTextBox("Imie", 24);
            txtLastName = CreateTextBox("Nazwisko", 72);
            txtPesel = CreateTextBox("PESEL", 120);
            txtPhone = CreateTextBox("Telefon", 168);
            txtEmail = CreateTextBox("E-mail", 216);
            txtAddress = CreateTextBox("Adres", 264);

            btnSave = new Guna2Button
            {
                Text = "Zapisz",
                Location = new Point(210, 314),
                Size = new Size(84, 32),
                BorderRadius = 8,
                FillColor = Color.FromArgb(0, 130, 110),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnSave.Click += btnSave_Click;

            btnCancel = new Guna2Button
            {
                Text = "Anuluj",
                Location = new Point(304, 314),
                Size = new Size(84, 32),
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
                txtFirstName, txtLastName, txtPesel, txtPhone, txtEmail, txtAddress,
                btnSave, btnCancel
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string pesel = txtPesel.Text.Trim();

            if (firstName.Length == 0 || lastName.Length == 0)
            {
                MessageBox.Show("Podaj imie i nazwisko pacjenta.", "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                MessageBox.Show("PESEL musi skladac sie z 11 cyfr.", "SISMED",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Patient = new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                Pesel = pesel,
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                WarningCount = 0
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
