using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public sealed class PatientSwapDialog : Form
    {
        private readonly IClinicDataStore dataStore;
        private readonly int currentPatientId;
        private readonly TextBox txtPesel;
        private readonly TextBox txtFirstName;
        private readonly TextBox txtLastName;
        private readonly TextBox txtBirthDate;
        private readonly TextBox txtPhone;
        private readonly TextBox txtEmail;
        private readonly Button btnSearch;
        private readonly Button btnChoose;
        private readonly Button btnCancel;
        private readonly DataGridView dgvResults;

        public Patient SelectedPatient { get; private set; }

        public PatientSwapDialog(IClinicDataStore dataStore, int currentPatientId)
        {
            this.dataStore = dataStore;
            this.currentPatientId = currentPatientId;

            Text = "Zamień wizytę na innego pacjenta";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(760, 520);
            MinimumSize = new Size(620, 460);
            BackColor = SismedTheme.Surface;
            Shown += (sender, args) => SismedTheme.FitFormToWorkingArea(this);

            var title = new Label
            {
                Text = "Wyszukaj nowego pacjenta",
                Location = new Point(24, 18),
                Size = new Size(420, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = SismedTheme.Font(15f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };

            txtPesel = CreateTextBox("PESEL", 24, 64, 210);
            txtFirstName = CreateTextBox("Imię", 248, 64, 210);
            txtLastName = CreateTextBox("Nazwisko", 472, 64, 210);
            txtBirthDate = CreateTextBox("Data urodzenia dd.MM.yyyy", 24, 112, 210);
            txtPhone = CreateTextBox("Telefon", 248, 112, 210);
            txtEmail = CreateTextBox("E-mail", 472, 112, 210);

            btnSearch = new Button
            {
                Text = "Szukaj",
                Location = new Point(24, 164),
                Size = new Size(110, 34)
            };
            SismedTheme.ApplyPrimaryButton(btnSearch);
            btnSearch.Click += btnSearch_Click;

            dgvResults = new DataGridView
            {
                Location = new Point(24, 214),
                Size = new Size(708, 224),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            SismedTheme.ApplyGrid(dgvResults);
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "firstName", HeaderText = "Imię", Width = 110 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "lastName", HeaderText = "Nazwisko", Width = 130 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "pesel", HeaderText = "PESEL", Width = 120 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "birthDate", HeaderText = "Data ur.", Width = 100 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "phone", HeaderText = "Telefon", Width = 100 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "email", HeaderText = "E-mail", Width = 170 });
            dgvResults.CellDoubleClick += (sender, args) => ChooseSelectedPatient();

            btnChoose = new Button
            {
                Text = "Wybierz pacjenta",
                Location = new Point(470, 462),
                Size = new Size(140, 34),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            SismedTheme.ApplySuccessButton(btnChoose);
            btnChoose.Click += (sender, args) => ChooseSelectedPatient();

            btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(622, 462),
                Size = new Size(110, 34),
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
                title,
                txtPesel,
                txtFirstName,
                txtLastName,
                txtBirthDate,
                txtPhone,
                txtEmail,
                btnSearch,
                dgvResults,
                btnChoose,
                btnCancel
            });
        }

        private TextBox CreateTextBox(string placeholder, int left, int top, int width)
        {
            var textBox = new TextBox
            {
                Location = new Point(left, top),
                Size = new Size(width, 28)
            };
            SismedTheme.ApplyTextBox(textBox, placeholder);
            return textBox;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PatientSearchCriteria criteria;
            if (!TryBuildCriteria(out criteria))
            {
                return;
            }

            LoadResults(dataStore.SearchPatients(criteria));
        }

        private bool TryBuildCriteria(out PatientSearchCriteria criteria)
        {
            criteria = null;
            DateTime? birthDate;

            if (!InputValidation.IsPeselPrefix(txtPesel.Text))
            {
                ShowWarning("PESEL może zawierać tylko cyfry i maksymalnie 11 znaków.");
                return false;
            }

            if (!InputValidation.IsOptionalName(txtFirstName.Text)
                || !InputValidation.IsOptionalName(txtLastName.Text))
            {
                ShowWarning("Imię i nazwisko mogą zawierać tylko litery, spacje i myślnik.");
                return false;
            }

            if (!InputValidation.TryParseBirthDate(txtBirthDate.Text, out birthDate))
            {
                ShowWarning("Data urodzenia musi mieć format dd.MM.yyyy albo dd-MM-yyyy.");
                return false;
            }

            if (!InputValidation.IsPhone(txtPhone.Text))
            {
                ShowWarning("Telefon może zawierać tylko cyfry i maksymalnie 9 znaków.");
                return false;
            }

            if (!InputValidation.IsOptionalEmail(txtEmail.Text))
            {
                ShowWarning("Podaj poprawny adres e-mail.");
                return false;
            }

            criteria = new PatientSearchCriteria
            {
                Pesel = txtPesel.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                BirthDate = birthDate,
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            if (criteria.IsEmpty)
            {
                ShowWarning("Podaj przynajmniej jedno kryterium wyszukiwania pacjenta.");
                return false;
            }

            return true;
        }

        private void LoadResults(IReadOnlyList<Patient> patients)
        {
            dgvResults.Rows.Clear();
            foreach (Patient patient in patients)
            {
                int rowIndex = dgvResults.Rows.Add(
                    Safe(patient.FirstName),
                    Safe(patient.LastName),
                    Safe(patient.Pesel),
                    patient.BirthDate.HasValue ? patient.BirthDate.Value.ToString("dd.MM.yyyy") : "-",
                    Safe(patient.Phone),
                    Safe(patient.Email));
                dgvResults.Rows[rowIndex].Tag = patient;
            }

            dgvResults.ClearSelection();
            if (patients.Count == 0)
            {
                ShowWarning("Nie znaleziono pacjenta.");
            }
        }

        private void ChooseSelectedPatient()
        {
            if (dgvResults.CurrentRow == null)
            {
                ShowWarning("Wybierz pacjenta z listy.");
                return;
            }

            Patient patient = dgvResults.CurrentRow.Tag as Patient;
            if (patient == null)
            {
                ShowWarning("Wybierz pacjenta z listy.");
                return;
            }

            patient = dataStore.GetPatient(patient.Id) ?? patient;

            if (patient.Id == currentPatientId)
            {
                ShowWarning("Nie można zamienić wizyty na tego samego pacjenta.");
                return;
            }

            if (patient.IsBlocked)
            {
                ShowWarning("Pacjent ma aktywną blokadę rezerwacji do "
                    + patient.BlockedUntil.Value.ToString("dd.MM.yyyy") + ".");
                return;
            }

            SelectedPatient = patient;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static string Safe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static void ShowWarning(string message)
        {
            MessageBox.Show(message, "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
