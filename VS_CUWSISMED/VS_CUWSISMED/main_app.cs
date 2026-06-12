using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public partial class main_app : Form
    {
        private readonly IClinicDataStore dataStore;
        private readonly Employee currentEmployee;
        private Patient selectedPatient;
        private Patient swapPatient;
        private Employee selectedEmployee;
        private string lastStatus;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public main_app()
            : this(null)
        {
        }

        public main_app(Employee employee)
        {
            dataStore = AppServices.DataStore;
            currentEmployee = employee;
            InitializeComponent();
            MouseDown += MoveForm;

            ConfigureCurrentUser();
            LoadDoctorLists();
            RefreshPatientCard(null);
            LoadCalendar();
            RefreshReservedAppointments();
            LoadEmployeeList(string.Empty);
            RefreshReceptionStats();
            ShowReceptionScreen();
            SetStatus("Gotowy");
        }

        private bool IsCurrentUserAdministrator
        {
            get
            {
                return currentEmployee != null && currentEmployee.IsAdministrator;
            }
        }

        private void ConfigureCurrentUser()
        {
            btnAddEmployee.Visible = IsCurrentUserAdministrator;
            btnDeactivateEmployee.Visible = IsCurrentUserAdministrator;
            lblPersonnelAccess.Text = IsCurrentUserAdministrator
                ? string.Empty
                : "Tryb podgladu: tylko administrator moze dodawac i dezaktywowac konta.";
            UpdateCurrentUserLabel();
        }

        private void MoveForm(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        }

        private void btnNavReception_Click(object sender, EventArgs e)
        {
            ShowReceptionScreen();
        }

        private void btnNavCalendar_Click(object sender, EventArgs e)
        {
            ShowScreen(pnlCalendarScreen, "KALENDARZ WIZYT");
            LoadCalendar();
        }

        private void btnNavDocuments_Click(object sender, EventArgs e)
        {
            ShowScreen(pnlDocumentsScreen, "DOKUMENTY");
        }

        private void btnNavPersonnel_Click(object sender, EventArgs e)
        {
            ShowScreen(pnlPersonnelScreen, "PERSONEL");
            LoadEmployeeList(txtEmployeeSearch.Text);
        }

        private void ShowReceptionScreen()
        {
            ShowScreen(pnlReceptionScreen, "RECEPCJA");
        }

        private void ShowScreen(Panel panel, string title)
        {
            pnlReceptionScreen.Visible = false;
            pnlCalendarScreen.Visible = false;
            pnlDocumentsScreen.Visible = false;
            pnlPersonnelScreen.Visible = false;

            panel.Visible = true;
            panel.BringToFront();
            lblScreenTitle.Text = title;
            SetActiveNav(title);
        }

        private void SetActiveNav(string title)
        {
            btnNavCalendar.FillColor = title == "KALENDARZ WIZYT" ? SismedTheme.Magenta : SismedTheme.NavyDark;
            btnNavReception.FillColor = title == "RECEPCJA" ? SismedTheme.Magenta : SismedTheme.NavyDark;
            btnNavDocuments.FillColor = title == "DOKUMENTY" ? SismedTheme.Magenta : SismedTheme.NavyDark;
            btnNavPersonnel.FillColor = title == "PERSONEL" ? SismedTheme.Magenta : SismedTheme.NavyDark;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PatientSearchCriteria criteria;
            if (!TryBuildPatientSearchCriteria(out criteria))
            {
                return;
            }

            IReadOnlyList<Patient> patients = dataStore.SearchPatients(criteria);
            selectedPatient = patients.FirstOrDefault();
            swapPatient = null;
            lblSwapResult.Text = string.Empty;
            btnSwap.Enabled = false;

            if (selectedPatient == null)
            {
                RefreshPatientCard(null);
                RefreshReservedAppointments();
                SetStatus("Nie znaleziono pacjenta.");
                return;
            }

            RefreshPatientCard(selectedPatient);
            RefreshReservedAppointments();
            SetStatus(patients.Count > 1
                ? "Znaleziono kilku pacjentow, pokazano pierwszy wynik: " + selectedPatient.DisplayName
                : "Wybrano pacjenta: " + selectedPatient.DisplayName);
        }

        private void btnClearPatientSearch_Click(object sender, EventArgs e)
        {
            txtPatientPesel.Text = string.Empty;
            txtPatientFirstName.Text = string.Empty;
            txtPatientLastName.Text = string.Empty;
            txtPatientBirthDate.Text = string.Empty;
            txtPatientPhone.Text = string.Empty;
            txtPatientEmail.Text = string.Empty;
            selectedPatient = null;
            swapPatient = null;
            lblSwapResult.Text = string.Empty;
            btnSwap.Enabled = false;
            RefreshPatientCard(null);
            RefreshReservedAppointments();
            SetStatus("Wyczyszczono wyszukiwanie pacjenta.");
        }

        private void btnAddPatient_Click(object sender, EventArgs e)
        {
            using (var dialog = new AddPatientDialog())
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    selectedPatient = dataStore.AddPatient(dialog.Patient);
                    FillPatientSearchFields(selectedPatient);
                    RefreshPatientCard(selectedPatient);
                    RefreshReservedAppointments();
                    RefreshReceptionStats();
                    SetStatus("Dodano pacjenta: " + selectedPatient.DisplayName);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private bool TryBuildPatientSearchCriteria(out PatientSearchCriteria criteria)
        {
            criteria = null;
            DateTime? birthDate;

            if (!InputValidation.IsPeselPrefix(txtPatientPesel.Text))
            {
                ShowError("PESEL moze zawierac tylko cyfry i maksymalnie 11 znakow.");
                return false;
            }

            if (!InputValidation.IsOptionalName(txtPatientFirstName.Text)
                || !InputValidation.IsOptionalName(txtPatientLastName.Text))
            {
                ShowError("Imie i nazwisko moga zawierac tylko litery, spacje i myslnik.");
                return false;
            }

            if (!InputValidation.TryParseBirthDate(txtPatientBirthDate.Text, out birthDate))
            {
                ShowError("Data urodzenia musi miec format dd.MM.yyyy albo dd-MM-yyyy.");
                return false;
            }

            if (!InputValidation.IsPhone(txtPatientPhone.Text))
            {
                ShowError("Telefon moze zawierac tylko cyfry i maksymalnie 9 znakow.");
                return false;
            }

            if (!InputValidation.IsOptionalEmail(txtPatientEmail.Text))
            {
                ShowError("Podaj poprawny adres e-mail.");
                return false;
            }

            criteria = new PatientSearchCriteria
            {
                Pesel = txtPatientPesel.Text.Trim(),
                FirstName = txtPatientFirstName.Text.Trim(),
                LastName = txtPatientLastName.Text.Trim(),
                BirthDate = birthDate,
                Phone = txtPatientPhone.Text.Trim(),
                Email = txtPatientEmail.Text.Trim()
            };

            if (criteria.IsEmpty)
            {
                ShowError("Podaj przynajmniej jedno kryterium wyszukiwania pacjenta.");
                return false;
            }

            return true;
        }

        private void FillPatientSearchFields(Patient patient)
        {
            if (patient == null)
            {
                return;
            }

            txtPatientPesel.Text = patient.Pesel ?? string.Empty;
            txtPatientFirstName.Text = patient.FirstName ?? string.Empty;
            txtPatientLastName.Text = patient.LastName ?? string.Empty;
            txtPatientBirthDate.Text = patient.BirthDate.HasValue
                ? patient.BirthDate.Value.ToString("dd.MM.yyyy")
                : string.Empty;
            txtPatientPhone.Text = patient.Phone ?? string.Empty;
            txtPatientEmail.Text = patient.Email ?? string.Empty;
        }

        private void btnOpenCalendar_Click(object sender, EventArgs e)
        {
            btnNavCalendar_Click(sender, e);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLoadSlots_Click(object sender, EventArgs e)
        {
            LoadSlots();
        }

        private void btnReserve_Click(object sender, EventArgs e)
        {
            if (selectedPatient == null)
            {
                ShowError("Najpierw wyszukaj albo dodaj pacjenta.");
                return;
            }

            AvailableSlot slot = GetSelectedSlot();
            if (slot == null)
            {
                ShowError("Wybierz wolny termin z listy.");
                return;
            }

            try
            {
                dataStore.ReserveAppointment(slot.Doctor.Id, selectedPatient.Id, slot.StartAt);
                SetStatus("Zarezerwowano wizyte: " + slot.StartAt.ToString("dd.MM.yyyy HH:mm"));
                LoadSlots();
                LoadCalendar();
                RefreshReservedAppointments();
                RefreshReceptionStats();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnLoadCal_Click(object sender, EventArgs e)
        {
            LoadCalendar();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Appointment appointment = GetSelectedReservedAppointment();
            if (appointment == null)
            {
                ShowError("Wybierz zarezerwowana wizyte.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Czy anulowac wybrana wizyte?",
                "SISMED",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            dataStore.CancelAppointment(appointment.Id, "Anulowano przez pracownika");
            SetStatus("Wizyta zostala anulowana.");
            LoadCalendar();
            LoadSlots();
            RefreshPatientCard(selectedPatient);
            RefreshReservedAppointments();
            RefreshReceptionStats();
        }

        private void btnSwapFind_Click(object sender, EventArgs e)
        {
            swapPatient = dataStore.FindPatient(txtSwapSearch.Text);
            if (swapPatient == null)
            {
                lblSwapResult.ForeColor = SismedTheme.Danger;
                lblSwapResult.Text = "Nie znaleziono pacjenta";
                btnSwap.Enabled = false;
                return;
            }

            lblSwapResult.ForeColor = SismedTheme.Success;
            lblSwapResult.Text = swapPatient.DisplayName;
            btnSwap.Enabled = GetSelectedReservedAppointment() != null;
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            Appointment appointment = GetSelectedReservedAppointment();
            if (appointment == null)
            {
                ShowError("Wybierz wizyte do zamiany.");
                return;
            }

            if (swapPatient == null)
            {
                ShowError("Najpierw wyszukaj pacjenta do zamiany.");
                return;
            }

            try
            {
                dataStore.SwapAppointmentPatient(appointment.Id, swapPatient.Id);
                selectedPatient = swapPatient;
                FillPatientSearchFields(selectedPatient);
                RefreshPatientCard(selectedPatient);
                RefreshReservedAppointments();
                LoadCalendar();
                RefreshReceptionStats();
                SetStatus("Zamieniono pacjenta na wizycie.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnEmployeeSearch_Click(object sender, EventArgs e)
        {
            LoadEmployeeList(txtEmployeeSearch.Text);
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            if (!RequireAdministrator())
            {
                return;
            }

            using (var dialog = new RegisterEmployeeDialog(true))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                LoadEmployeeList(txtEmployeeSearch.Text);
                SelectEmployee(dialog.RegisteredEmployee);
                SetStatus("Dodano pracownika: " + dialog.RegisteredEmployee.FullName);
            }
        }

        private void btnDeactivateEmployee_Click(object sender, EventArgs e)
        {
            if (!RequireAdministrator())
            {
                return;
            }

            Employee employee = GetSelectedEmployee();
            if (employee == null)
            {
                ShowError("Wybierz pracownika.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Czy dezaktywowac konto pracownika " + employee.FullName + "?",
                "SISMED",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                AppServices.AuthService.DeactivateEmployee(currentEmployee, employee.Id);
                LoadEmployeeList(txtEmployeeSearch.Text);
                SetStatus("Konto pracownika zostalo dezaktywowane.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            selectedEmployee = GetSelectedEmployee();
            RefreshEmployeeDetails(selectedEmployee);
        }

        private bool RequireAdministrator()
        {
            if (IsCurrentUserAdministrator)
            {
                return true;
            }

            ShowError("Brak uprawnien administratora do tej funkcji.");
            return false;
        }

        private void LoadDoctorLists()
        {
            IReadOnlyList<Doctor> doctors = dataStore.GetDoctors();

            cmbDoctor.DataSource = new List<Doctor>(doctors);
            cmbDoctor.DisplayMember = "DisplayName";
            cmbDoctor.ValueMember = "Id";

            cmbCalDoctor.DataSource = new List<Doctor>(doctors);
            cmbCalDoctor.DisplayMember = "DisplayName";
            cmbCalDoctor.ValueMember = "Id";
        }

        private void LoadSlots()
        {
            Doctor doctor = cmbDoctor.SelectedItem as Doctor;
            dgvSlots.Rows.Clear();

            if (doctor == null)
            {
                return;
            }

            foreach (AvailableSlot slot in dataStore.GetAvailableSlots(doctor.Id, dtpBook.Value.Date))
            {
                int rowIndex = dgvSlots.Rows.Add(
                    slot.StartAt.ToString("HH:mm"),
                    slot.Doctor.DisplayName,
                    slot.Doctor.Specialization,
                    slot.StartAt.ToString("dd.MM.yyyy"));
                dgvSlots.Rows[rowIndex].Tag = slot;
            }

            SetStatus("Wczytano wolne terminy: " + dgvSlots.Rows.Count);
        }

        private void LoadCalendar()
        {
            Doctor doctor = cmbCalDoctor.SelectedItem as Doctor;
            dgvCal.Rows.Clear();

            if (doctor == null)
            {
                return;
            }

            foreach (Appointment appointment in dataStore.GetAppointmentsForDoctor(doctor.Id, dtpCal.Value.Date))
            {
                Patient patient = appointment.PatientId.HasValue
                    ? dataStore.GetPatient(appointment.PatientId.Value)
                    : null;

                int rowIndex = dgvCal.Rows.Add(
                    appointment.StartAt.ToString("HH:mm"),
                    patient == null ? string.Empty : patient.DisplayName,
                    appointment.StatusText);
                dgvCal.Rows[rowIndex].Tag = appointment;
            }
        }

        private void RefreshReservedAppointments()
        {
            dgvReserved.Rows.Clear();
            if (selectedPatient == null)
            {
                return;
            }

            foreach (Appointment appointment in dataStore.GetAppointmentsForPatient(selectedPatient.Id))
            {
                Doctor doctor = dataStore.GetDoctor(appointment.DoctorId);
                int rowIndex = dgvReserved.Rows.Add(
                    appointment.StartAt.ToString("dd.MM.yyyy"),
                    appointment.StartAt.ToString("HH:mm"),
                    doctor == null ? string.Empty : doctor.DisplayName,
                    doctor == null ? string.Empty : doctor.Specialization,
                    appointment.StatusText,
                    appointment.Notes ?? string.Empty);
                dgvReserved.Rows[rowIndex].Tag = appointment;
            }
        }

        private void RefreshReceptionStats()
        {
            int todayVisits = 0;
            int plannedVisits = 0;

            foreach (Doctor doctor in dataStore.GetDoctors())
            {
                foreach (Appointment appointment in dataStore.GetAppointmentsForDoctor(doctor.Id, DateTime.Today))
                {
                    if (appointment.Status == AppointmentStatus.Reserved)
                    {
                        todayVisits++;
                    }
                }

                for (int dayOffset = 0; dayOffset < 30; dayOffset++)
                {
                    DateTime day = DateTime.Today.AddDays(dayOffset);
                    foreach (Appointment appointment in dataStore.GetAppointmentsForDoctor(doctor.Id, day))
                    {
                        if (appointment.Status == AppointmentStatus.Reserved && appointment.StartAt >= DateTime.Now)
                        {
                            plannedVisits++;
                        }
                    }
                }
            }

            lblTodayVisitsValue.Text = todayVisits.ToString();
            lblPlannedVisitsValue.Text = plannedVisits.ToString();
            lblPatientsValue.Text = dataStore.GetPatientCount().ToString();
        }

        private void LoadEmployeeList(string query)
        {
            dgvEmployees.Rows.Clear();

            foreach (Employee employee in dataStore.SearchEmployees(query))
            {
                int rowIndex = dgvEmployees.Rows.Add(
                    employee.FullName,
                    employee.Login,
                    employee.Role,
                    employee.StatusText,
                    employee.IsDoctor ? "Tak" : "Nie");
                dgvEmployees.Rows[rowIndex].Tag = employee;
            }

            if (dgvEmployees.Rows.Count > 0)
            {
                dgvEmployees.Rows[0].Selected = true;
                selectedEmployee = dgvEmployees.Rows[0].Tag as Employee;
            }
            else
            {
                selectedEmployee = null;
            }

            RefreshEmployeeDetails(selectedEmployee);
        }

        private void SelectEmployee(Employee employee)
        {
            if (employee == null)
            {
                return;
            }

            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                Employee rowEmployee = row.Tag as Employee;
                if (rowEmployee != null && rowEmployee.Id == employee.Id)
                {
                    row.Selected = true;
                    dgvEmployees.CurrentCell = row.Cells[0];
                    selectedEmployee = rowEmployee;
                    RefreshEmployeeDetails(rowEmployee);
                    return;
                }
            }
        }

        private void RefreshPatientCard(Patient patient)
        {
            if (patient == null)
            {
                lblPatientName.Text = "- Brak wybranego pacjenta -";
                lblPatientPesel.Text = "PESEL: -";
                lblPatientBirthDate.Text = "Data ur.: -";
                lblPatientPhone.Text = "Tel: -";
                lblPatientEmail.Text = "E-mail: -";
                lblPatientWarnings.Text = "Ostrzezenia: 0/3";
                lblPatientNotes.Text = "Notatka: -";
                lblPatientStatus.Text = string.Empty;
                return;
            }

            Patient current = dataStore.GetPatient(patient.Id) ?? patient;
            selectedPatient = current;

            lblPatientName.Text = current.DisplayName;
            lblPatientPesel.Text = "PESEL: " + current.Pesel;
            lblPatientBirthDate.Text = "Data ur.: "
                + (current.BirthDate.HasValue ? current.BirthDate.Value.ToString("dd.MM.yyyy") : "-");
            lblPatientPhone.Text = "Tel: " + (string.IsNullOrWhiteSpace(current.Phone) ? "-" : current.Phone);
            lblPatientEmail.Text = "E-mail: " + (string.IsNullOrWhiteSpace(current.Email) ? "-" : current.Email);

            IReadOnlyList<PatientWarning> warnings = dataStore.GetPatientWarnings(current.Id);
            IReadOnlyList<PatientNote> notes = dataStore.GetPatientNotes(current.Id);
            lblPatientWarnings.Text = "Ostrzezenia: " + Math.Max(current.WarningCount, warnings.Count) + "/3";
            lblPatientNotes.Text = "Notatka: " + (notes.Count > 0 ? Truncate(notes[0].Text, 44) : "-");
            lblPatientStatus.Text = current.IsBlocked
                ? "Blokada do " + current.BlockedUntil.Value.ToString("dd.MM.yyyy")
                : string.Empty;
        }

        private void RefreshEmployeeDetails(Employee employee)
        {
            if (employee == null)
            {
                lblEmployeeName.Text = "Imię i nazwisko: -";
                lblEmployeePesel.Text = "PESEL: -";
                lblEmployeeBirthDate.Text = "Data urodzenia: -";
                lblEmployeeLogin.Text = "Login: -";
                lblEmployeeRole.Text = "Rola: -";
                lblEmployeeStatus.Text = "Status: -";
                lblEmployeeDoctor.Text = "Lekarz: -";
                lblEmployeeSpecialization.Text = "Specjalizacja: -";
                return;
            }

            lblEmployeeName.Text = "Imię i nazwisko: " + employee.FullName;
            lblEmployeePesel.Text = "PESEL: " + Safe(employee.Pesel);
            lblEmployeeBirthDate.Text = "Data urodzenia: "
                + (employee.BirthDate.HasValue ? employee.BirthDate.Value.ToString("dd.MM.yyyy") : "-");
            lblEmployeeLogin.Text = "Login: " + Safe(employee.Login);
            lblEmployeeRole.Text = "Rola: " + employee.Role;
            lblEmployeeStatus.Text = "Status: " + employee.StatusText;
            lblEmployeeDoctor.Text = "Lekarz: " + (employee.IsDoctor ? "Tak" : "Nie");
            lblEmployeeSpecialization.Text = "Specjalizacja: "
                + (employee.IsDoctor ? Safe(employee.Specialization) : "-");
        }

        private AvailableSlot GetSelectedSlot()
        {
            if (dgvSlots.CurrentRow == null)
            {
                return null;
            }

            return dgvSlots.CurrentRow.Tag as AvailableSlot;
        }

        private Appointment GetSelectedReservedAppointment()
        {
            if (dgvReserved.CurrentRow == null)
            {
                return null;
            }

            return dgvReserved.CurrentRow.Tag as Appointment;
        }

        private Employee GetSelectedEmployee()
        {
            if (dgvEmployees.CurrentRow == null)
            {
                return selectedEmployee;
            }

            return dgvEmployees.CurrentRow.Tag as Employee;
        }

        private void SetStatus(string message)
        {
            lastStatus = message;
            UpdateCurrentUserLabel();
        }

        private void UpdateCurrentUserLabel()
        {
            string user = currentEmployee == null
                ? "pracownik"
                : currentEmployee.FullName + " - " + currentEmployee.Role;

            lblCurrentUser.Text = user + " | " + AppServices.StorageInfo
                + (string.IsNullOrWhiteSpace(lastStatus) ? string.Empty : " | " + lastStatus);
        }

        private static string Safe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static string Truncate(string value, int maxLength)
        {
            value = value ?? string.Empty;
            if (value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength - 3) + "...";
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
