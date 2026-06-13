using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        private SismedDocument selectedDocument;
        private string lastStatus;
        private string activePatientSection;

        private const string PatientSectionMessages = "messages";
        private const string PatientSectionPlanned = "planned";
        private const string PatientSectionHistory = "history";
        private const string PatientSectionBooking = "booking";

        private sealed class BookingServiceOption
        {
            public MedicalService Service { get; set; }

            public string Text
            {
                get
                {
                    if (Service == null)
                    {
                        return string.Empty;
                    }

                    return Service.Name + " - " + Service.Specialization;
                }
            }
        }

        private sealed class BookingDoctorOption
        {
            public Doctor Doctor { get; set; }
            public string Specialization { get; set; }

            public bool IsAnyDoctor
            {
                get { return Doctor == null; }
            }

            public string Text
            {
                get
                {
                    return IsAnyDoctor
                        ? "Dowolny lekarz"
                        : Doctor.DisplayName + " - " + Doctor.Specialization;
                }
            }
        }

        private sealed class BookingRangeOption
        {
            public int Days { get; set; }

            public string Text
            {
                get { return Days + " dni"; }
            }
        }

        private sealed class PatientBookingSlot
        {
            public Doctor Doctor { get; set; }
            public MedicalService Service { get; set; }
            public DateTime StartAt { get; set; }
        }

        private sealed class CalendarDoctorOption
        {
            public Doctor Doctor { get; set; }

            public bool IsAll
            {
                get { return Doctor == null; }
            }

            public string Text
            {
                get { return IsAll ? "Wszyscy lekarze" : Doctor.DisplayName; }
            }
        }

        private sealed class CalendarServiceOption
        {
            public MedicalService Service { get; set; }

            public bool IsAll
            {
                get { return Service == null; }
            }

            public string Text
            {
                get { return IsAll ? "Wszystkie specjalizacje" : Service.Name + " - " + Service.Specialization; }
            }
        }

        private sealed class CalendarStatusOption
        {
            public string Text { get; set; }
            public string Key { get; set; }
        }

        private sealed class CalendarSlotRow
        {
            public DateTime StartAt { get; set; }
            public Doctor Doctor { get; set; }
            public MedicalService Service { get; set; }
            public Appointment Appointment { get; set; }
            public Patient Patient { get; set; }
            public string StatusText { get; set; }
        }

        private sealed class DocumentStatusFilterOption
        {
            public string Text { get; set; }
            public DocumentStatus? Status { get; set; }
        }

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
            LoadPatientBookingOptions();
            LoadCalendarFilters();
            LoadDocumentStatusFilter();
            LoadDocumentList();
            RefreshPatientCard(null);
            ShowPatientEmptyPanel("Wyszukaj pacjenta, aby zobaczyć kartę, notatki i wizyty.");
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
            btnArchiveDocument.Visible = IsCurrentUserAdministrator;
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
            LoadDocumentList();
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
            swapPatient = null;
            lblSwapResult.Text = string.Empty;
            btnSwap.Enabled = false;

            if (patients.Count == 0)
            {
                selectedPatient = null;
                RefreshPatientCard(null);
                RefreshReservedAppointments();
                ShowPatientEmptyPanel("Nie znaleziono pacjenta dla podanych kryteriów.");
                SetStatus("Nie znaleziono pacjenta.");
                return;
            }

            if (patients.Count == 1)
            {
                SelectPatient(patients[0], true);
                SetStatus("Wybrano pacjenta: " + selectedPatient.DisplayName);
                return;
            }

            selectedPatient = null;
            RefreshPatientCard(null);
            RefreshReservedAppointments();
            ShowPatientSearchResults(patients);
            SetStatus("Znaleziono " + patients.Count + " pacjentow. Wybierz pacjenta z listy wynikow.");
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
            ShowPatientEmptyPanel("Wyszukaj pacjenta, aby zobaczyć kartę, notatki i wizyty.");
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
                    Patient savedPatient = dataStore.AddPatient(dialog.Patient);
                    SelectPatient(savedPatient, true);
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
                RefreshActivePatientSection();
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

        private void dgvCal_SelectionChanged(object sender, EventArgs e)
        {
            CalendarSlotRow row = GetSelectedCalendarRow();
            ShowCalendarDetails(row);
        }

        private void btnCalendarOpenPatient_Click(object sender, EventArgs e)
        {
            CalendarSlotRow row = GetSelectedCalendarRow();
            if (row == null || row.Patient == null)
            {
                ShowError("Wybierz wizytę z pacjentem.");
                return;
            }

            ShowReceptionScreen();
            SelectPatient(row.Patient, true);
            ShowPatientPlannedPanel();
        }

        private void btnCalendarCancelAppointment_Click(object sender, EventArgs e)
        {
            CalendarSlotRow row = GetSelectedCalendarRow();
            if (row == null || row.Appointment == null || row.Appointment.Status != AppointmentStatus.Reserved)
            {
                ShowError("Wybierz zarezerwowaną wizytę.");
                return;
            }

            string reason = txtCalendarCancelReason.Text.Trim();
            if (string.IsNullOrWhiteSpace(reason))
            {
                ShowError("Podaj powód anulowania wizyty.");
                return;
            }

            bool lateCancellation = row.Appointment.StartAt > DateTime.Now
                && row.Appointment.StartAt.Subtract(DateTime.Now).TotalHours < 12;

            DialogResult result = MessageBox.Show(
                "Czy anulować wizytę pacjenta "
                + (row.Patient == null ? "-" : row.Patient.DisplayName)
                + "?"
                + Environment.NewLine
                + (lateCancellation
                    ? "Anulowanie mniej niż 12 godzin przed terminem doda ostrzeżenie."
                    : "Anulowanie bez ostrzeżenia."),
                "SISMED",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                dataStore.CancelAppointment(row.Appointment.Id, reason);
                if (selectedPatient != null)
                {
                    selectedPatient = dataStore.GetPatient(selectedPatient.Id) ?? selectedPatient;
                    RefreshPatientCard(selectedPatient);
                    RefreshReservedAppointments();
                    RefreshActivePatientSection();
                }

                txtCalendarCancelReason.Text = string.Empty;
                LoadCalendar();
                LoadSlots();
                RefreshReceptionStats();
                SetStatus("Anulowano wizytę z kalendarza.");
                MessageBox.Show("Wizyta została anulowana.", "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
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
            RefreshActivePatientSection();
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
                dataStore.SwapAppointmentPatient(appointment.Id, swapPatient.Id, GetCurrentEmployeeName());
                SelectPatient(swapPatient, true);
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

        private void btnDocumentSearch_Click(object sender, EventArgs e)
        {
            LoadDocumentList();
        }

        private void btnDocumentClear_Click(object sender, EventArgs e)
        {
            txtDocumentSearch.Text = string.Empty;
            if (cmbDocumentStatus.Items.Count > 0)
            {
                cmbDocumentStatus.SelectedIndex = 0;
            }

            LoadDocumentList();
        }

        private void btnAddDocument_Click(object sender, EventArgs e)
        {
            using (var dialog = new DocumentDialog(null, IsCurrentUserAdministrator))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    SismedDocument document = dialog.Document;
                    document.Author = GetCurrentEmployeeName();
                    document.LastEditedBy = GetCurrentEmployeeName();

                    SismedDocument saved = dataStore.AddDocument(document);
                    LoadDocumentList();
                    SelectDocument(saved);
                    SetStatus("Dodano dokument: " + saved.Title);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void btnEditDocument_Click(object sender, EventArgs e)
        {
            SismedDocument document = GetSelectedDocument();
            if (document == null)
            {
                ShowError("Wybierz dokument do edycji.");
                return;
            }

            using (var dialog = new DocumentDialog(document, IsCurrentUserAdministrator))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    SismedDocument saved = dataStore.UpdateDocument(dialog.Document, GetCurrentEmployeeName());
                    LoadDocumentList();
                    SelectDocument(saved);
                    SetStatus("Zapisano dokument: " + saved.Title);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void btnArchiveDocument_Click(object sender, EventArgs e)
        {
            if (!RequireAdministrator())
            {
                return;
            }

            SismedDocument document = GetSelectedDocument();
            if (document == null)
            {
                ShowError("Wybierz dokument do archiwizacji.");
                return;
            }

            if (document.Status == DocumentStatus.Archived)
            {
                ShowError("Dokument jest już archiwalny.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Czy przenieść dokument do archiwum?" + Environment.NewLine + document.Title,
                "SISMED",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                dataStore.ArchiveDocument(document.Id, GetCurrentEmployeeName());
                LoadDocumentList();
                SetStatus("Dokument został zarchiwizowany.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void dgvDocuments_SelectionChanged(object sender, EventArgs e)
        {
            selectedDocument = GetSelectedDocument();
            RefreshDocumentDetails(selectedDocument);
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
        }

        private void LoadCalendarFilters()
        {
            var doctorOptions = new List<CalendarDoctorOption>
            {
                new CalendarDoctorOption()
            };
            doctorOptions.AddRange(dataStore.GetDoctors().Select(d => new CalendarDoctorOption { Doctor = d }));
            cmbCalDoctor.DisplayMember = "Text";
            cmbCalDoctor.DataSource = doctorOptions;
            cmbCalDoctor.SelectedIndex = 0;

            var serviceOptions = new List<CalendarServiceOption>
            {
                new CalendarServiceOption()
            };
            serviceOptions.AddRange(dataStore.GetServices().Select(s => new CalendarServiceOption { Service = s }));
            cmbCalService.DisplayMember = "Text";
            cmbCalService.DataSource = serviceOptions;
            cmbCalService.SelectedIndex = 0;

            cmbCalStatus.DisplayMember = "Text";
            cmbCalStatus.DataSource = new List<CalendarStatusOption>
            {
                new CalendarStatusOption { Text = "Wszystkie", Key = "all" },
                new CalendarStatusOption { Text = "Zarezerwowana", Key = "reserved" },
                new CalendarStatusOption { Text = "Anulowana", Key = "cancelled" },
                new CalendarStatusOption { Text = "Historyczna/Zakończona", Key = "history" }
            };
            cmbCalStatus.SelectedIndex = 0;
        }

        private void LoadDocumentStatusFilter()
        {
            cmbDocumentStatus.DisplayMember = "Text";
            cmbDocumentStatus.DataSource = new List<DocumentStatusFilterOption>
            {
                new DocumentStatusFilterOption { Text = "Wszystkie", Status = null },
                new DocumentStatusFilterOption { Text = "Aktywne", Status = DocumentStatus.Active },
                new DocumentStatusFilterOption { Text = "Archiwalne", Status = DocumentStatus.Archived }
            };
            cmbDocumentStatus.SelectedIndex = 0;
        }

        private void LoadDocumentList()
        {
            if (dgvDocuments == null)
            {
                return;
            }

            DocumentStatusFilterOption statusOption = cmbDocumentStatus.SelectedItem as DocumentStatusFilterOption;
            DocumentStatus? status = statusOption == null ? null : statusOption.Status;
            string query = txtDocumentSearch == null ? string.Empty : txtDocumentSearch.Text;

            IReadOnlyList<SismedDocument> documents = dataStore.SearchDocuments(query, status);
            dgvDocuments.Rows.Clear();
            RefreshDocumentDetails(null);

            foreach (SismedDocument document in documents)
            {
                int rowIndex = dgvDocuments.Rows.Add(
                    Safe(document.Title),
                    Safe(document.Category),
                    Safe(document.Author),
                    FormatDocumentDate(document.CreatedAt),
                    FormatDocumentDate(document.UpdatedAt),
                    document.StatusText);
                dgvDocuments.Rows[rowIndex].Tag = document;
            }

            if (dgvDocuments.Rows.Count > 0)
            {
                dgvDocuments.Rows[0].Selected = true;
                dgvDocuments.CurrentCell = dgvDocuments.Rows[0].Cells[0];
            }

            SetStatus("Wczytano dokumenty: " + documents.Count);
        }

        private void SelectDocument(SismedDocument document)
        {
            if (document == null)
            {
                return;
            }

            foreach (DataGridViewRow row in dgvDocuments.Rows)
            {
                SismedDocument rowDocument = row.Tag as SismedDocument;
                if (rowDocument != null && rowDocument.Id == document.Id)
                {
                    row.Selected = true;
                    dgvDocuments.CurrentCell = row.Cells[0];
                    RefreshDocumentDetails(rowDocument);
                    return;
                }
            }
        }

        private SismedDocument GetSelectedDocument()
        {
            if (dgvDocuments == null || dgvDocuments.CurrentRow == null)
            {
                return null;
            }

            return dgvDocuments.CurrentRow.Tag as SismedDocument;
        }

        private void RefreshDocumentDetails(SismedDocument document)
        {
            selectedDocument = document;

            if (document == null)
            {
                lblDocumentDetailsTitle.Text = "Wybierz dokument";
                lblDocumentDetailsMeta.Text = "Brak wybranego dokumentu.";
                txtDocumentDetailsContent.Text = string.Empty;
                btnEditDocument.Enabled = false;
                btnArchiveDocument.Enabled = false;
                return;
            }

            lblDocumentDetailsTitle.Text = Safe(document.Title);
            lblDocumentDetailsMeta.Text =
                "Kategoria: " + Safe(document.Category)
                + Environment.NewLine
                + "Autor: " + Safe(document.Author)
                + Environment.NewLine
                + "Utworzono: " + FormatDocumentDate(document.CreatedAt)
                + Environment.NewLine
                + "Modyfikacja: " + FormatDocumentDate(document.UpdatedAt)
                + Environment.NewLine
                + "Status: " + document.StatusText
                + Environment.NewLine
                + "Ostatnio edytował: " + Safe(document.LastEditedBy);
            txtDocumentDetailsContent.Text = document.Content ?? string.Empty;
            btnEditDocument.Enabled = true;
            btnArchiveDocument.Enabled = IsCurrentUserAdministrator && document.Status != DocumentStatus.Archived;
        }

        private void LoadPatientBookingOptions()
        {
            var serviceOptions = dataStore.GetServices()
                .Select(s => new BookingServiceOption { Service = s })
                .ToList();

            cmbPatientBookingService.DisplayMember = "Text";
            cmbPatientBookingService.DataSource = serviceOptions;
            cmbPatientBookingService.SelectedIndex = -1;

            var rangeOptions = new List<BookingRangeOption>
            {
                new BookingRangeOption { Days = 7 },
                new BookingRangeOption { Days = 14 },
                new BookingRangeOption { Days = 30 }
            };

            cmbPatientBookingRange.DisplayMember = "Text";
            cmbPatientBookingRange.DataSource = rangeOptions;
            cmbPatientBookingRange.SelectedIndex = 0;
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
            dgvCal.Rows.Clear();
            ClearCalendarDetails();

            DateTime date = dtpCal.Value.Date;
            CalendarDoctorOption doctorOption = cmbCalDoctor.SelectedItem as CalendarDoctorOption;
            CalendarServiceOption serviceOption = cmbCalService.SelectedItem as CalendarServiceOption;
            CalendarStatusOption statusOption = cmbCalStatus.SelectedItem as CalendarStatusOption;
            string statusKey = statusOption == null ? "all" : statusOption.Key;

            IEnumerable<Doctor> doctors = GetCalendarDoctors(doctorOption, serviceOption);
            foreach (CalendarSlotRow row in BuildCalendarRows(doctors, date, serviceOption, statusKey))
            {
                int rowIndex = dgvCal.Rows.Add(
                    row.StartAt.ToString("HH:mm"),
                    row.Doctor == null ? "-" : row.Doctor.DisplayName,
                    row.Service == null
                        ? (row.Doctor == null ? "-" : row.Doctor.Specialization)
                        : row.Service.Name + " / " + row.Service.Specialization,
                    row.StatusText,
                    row.Patient == null ? "-" : row.Patient.DisplayName);
                dgvCal.Rows[rowIndex].Tag = row;
            }

            dgvCal.ClearSelection();
            SetStatus("Wczytano grafik wizyt: " + dgvCal.Rows.Count);
        }

        private IEnumerable<Doctor> GetCalendarDoctors(
            CalendarDoctorOption doctorOption,
            CalendarServiceOption serviceOption)
        {
            IEnumerable<Doctor> doctors = dataStore.GetDoctors();

            if (doctorOption != null && !doctorOption.IsAll)
            {
                doctors = doctors.Where(d => d.Id == doctorOption.Doctor.Id);
            }

            if (serviceOption != null && !serviceOption.IsAll)
            {
                doctors = doctors.Where(d => string.Equals(
                    d.Specialization,
                    serviceOption.Service.Specialization,
                    StringComparison.OrdinalIgnoreCase));
            }

            return doctors.OrderBy(d => d.LastName).ThenBy(d => d.FirstName).ToList();
        }

        private IEnumerable<CalendarSlotRow> BuildCalendarRows(
            IEnumerable<Doctor> doctors,
            DateTime date,
            CalendarServiceOption serviceOption,
            string statusKey)
        {
            var rows = new List<CalendarSlotRow>();
            foreach (Doctor doctor in doctors)
            {
                MedicalService service = GetCalendarServiceForDoctor(doctor, serviceOption);
                IReadOnlyList<Appointment> storedAppointments = dataStore.GetAllAppointmentsForDoctor(doctor.Id, date);
                var reservedByStart = storedAppointments
                    .Where(a => a.Status == AppointmentStatus.Reserved)
                    .GroupBy(a => a.StartAt)
                    .ToDictionary(g => g.Key, g => g.First());

                if (statusKey == "all" || statusKey == "cancelled")
                {
                    foreach (Appointment cancelled in storedAppointments.Where(a => a.Status == AppointmentStatus.Cancelled))
                    {
                        rows.Add(CreateCalendarRow(cancelled, doctor, service, "Anulowana"));
                    }
                }

                if (statusKey == "cancelled")
                {
                    continue;
                }

                DateTime start = date.AddHours(7);
                DateTime end = date.AddHours(18);
                while (start < end)
                {
                    Appointment appointment;
                    if (reservedByStart.TryGetValue(start, out appointment))
                    {
                        string status = appointment.StartAt < DateTime.Now
                            ? "Historyczna/Zakończona"
                            : "Zarezerwowana";

                        if (statusKey == "all"
                            || statusKey == "reserved" && appointment.StartAt >= DateTime.Now
                            || statusKey == "history" && appointment.StartAt < DateTime.Now)
                        {
                            rows.Add(CreateCalendarRow(appointment, doctor, service, status));
                        }
                    }
                    else if (statusKey == "all")
                    {
                        rows.Add(new CalendarSlotRow
                        {
                            StartAt = start,
                            Doctor = doctor,
                            Service = service,
                            StatusText = "Dostępny"
                        });
                    }

                    start = start.AddMinutes(15);
                }
            }

            return rows
                .OrderBy(r => r.StartAt)
                .ThenBy(r => r.Doctor == null ? string.Empty : r.Doctor.LastName)
                .ThenBy(r => r.Doctor == null ? string.Empty : r.Doctor.FirstName)
                .ToList();
        }

        private CalendarSlotRow CreateCalendarRow(
            Appointment appointment,
            Doctor doctor,
            MedicalService service,
            string status)
        {
            Patient patient = appointment.PatientId.HasValue
                ? dataStore.GetPatient(appointment.PatientId.Value)
                : null;

            return new CalendarSlotRow
            {
                StartAt = appointment.StartAt,
                Doctor = doctor,
                Service = service,
                Appointment = appointment,
                Patient = patient,
                StatusText = status
            };
        }

        private MedicalService GetCalendarServiceForDoctor(Doctor doctor, CalendarServiceOption serviceOption)
        {
            if (serviceOption != null
                && !serviceOption.IsAll
                && string.Equals(serviceOption.Service.Specialization, doctor.Specialization, StringComparison.OrdinalIgnoreCase))
            {
                return serviceOption.Service;
            }

            return dataStore.GetServices()
                .FirstOrDefault(s => string.Equals(s.Specialization, doctor.Specialization, StringComparison.OrdinalIgnoreCase));
        }

        private CalendarSlotRow GetSelectedCalendarRow()
        {
            if (dgvCal.CurrentRow == null)
            {
                return null;
            }

            return dgvCal.CurrentRow.Tag as CalendarSlotRow;
        }

        private void ShowCalendarDetails(CalendarSlotRow row)
        {
            if (row == null)
            {
                ClearCalendarDetails();
                return;
            }

            if (row.Appointment == null)
            {
                lblCalendarDetails.Text =
                    "Termin dostępny"
                    + Environment.NewLine
                    + "Data i godzina: " + row.StartAt.ToString("dd.MM.yyyy HH:mm")
                    + Environment.NewLine
                    + "Lekarz: " + (row.Doctor == null ? "-" : row.Doctor.DisplayName)
                    + Environment.NewLine
                    + "Usługa/specjalizacja: " + GetCalendarServiceText(row)
                    + Environment.NewLine
                    + "Status: Dostępny";
                btnCalendarOpenPatient.Enabled = false;
                btnCalendarCancelAppointment.Enabled = false;
                return;
            }

            Patient patient = row.Patient;
            lblCalendarDetails.Text =
                "Pacjent: " + (patient == null ? "-" : patient.DisplayName)
                + Environment.NewLine
                + "PESEL: " + (patient == null ? "-" : Safe(patient.Pesel))
                + Environment.NewLine
                + "Telefon: " + (patient == null ? "-" : Safe(patient.Phone))
                + Environment.NewLine
                + "E-mail: " + (patient == null ? "-" : Safe(patient.Email))
                + Environment.NewLine
                + "Lekarz: " + (row.Doctor == null ? "-" : row.Doctor.DisplayName)
                + Environment.NewLine
                + "Usługa/specjalizacja: " + GetCalendarServiceText(row)
                + Environment.NewLine
                + "Termin: " + row.StartAt.ToString("dd.MM.yyyy HH:mm")
                + Environment.NewLine
                + "Status: " + row.StatusText
                + Environment.NewLine
                + "Notatki: " + Safe(row.Appointment.Notes)
                + (string.IsNullOrWhiteSpace(row.Appointment.CancelReason)
                    ? string.Empty
                    : Environment.NewLine + "Powód anulowania: " + row.Appointment.CancelReason);

            bool canAct = row.Appointment.Status == AppointmentStatus.Reserved
                && row.Appointment.StartAt >= DateTime.Now;
            btnCalendarOpenPatient.Enabled = patient != null;
            btnCalendarCancelAppointment.Enabled = canAct;
        }

        private void ClearCalendarDetails()
        {
            lblCalendarDetails.Text = "Wybierz zarezerwowany slot, aby zobaczyć szczegóły.";
            txtCalendarCancelReason.Text = string.Empty;
            btnCalendarOpenPatient.Enabled = false;
            btnCalendarCancelAppointment.Enabled = false;
        }

        private static string GetCalendarServiceText(CalendarSlotRow row)
        {
            if (row == null)
            {
                return "-";
            }

            if (row.Service != null)
            {
                return row.Service.Name + " / " + row.Service.Specialization;
            }

            return row.Doctor == null ? "-" : row.Doctor.Specialization;
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

        private void SelectPatient(Patient patient, bool fillSearchFields)
        {
            if (patient == null)
            {
                return;
            }

            selectedPatient = dataStore.GetPatient(patient.Id) ?? patient;
            if (fillSearchFields)
            {
                FillPatientSearchFields(selectedPatient);
            }

            RefreshPatientCard(selectedPatient);
            RefreshReservedAppointments();
            ShowPatientMessagesPanel();
            tabControl.SelectedTab = tabPatient;
        }

        private void ShowPatientSearchResults(IReadOnlyList<Patient> patients)
        {
            dgvPatientResults.Rows.Clear();

            foreach (Patient patient in patients)
            {
                int rowIndex = dgvPatientResults.Rows.Add(
                    Safe(patient.FirstName),
                    Safe(patient.LastName),
                    Safe(patient.Pesel),
                    patient.BirthDate.HasValue ? patient.BirthDate.Value.ToString("dd.MM.yyyy") : "-",
                    Safe(patient.Phone),
                    Safe(patient.Email));
                dgvPatientResults.Rows[rowIndex].Tag = patient;
            }

            dgvPatientResults.ClearSelection();
            SetPatientActionButtonsEnabled(false);
            ShowPatientActionPanel(pnlPatientResultsPanel, "Wyniki wyszukiwania pacjentów");
            tabControl.SelectedTab = tabPatient;
        }

        private void dgvPatientResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            Patient patient = dgvPatientResults.Rows[e.RowIndex].Tag as Patient;
            if (patient == null)
            {
                return;
            }

            SelectPatient(patient, true);
            SetStatus("Wybrano pacjenta: " + selectedPatient.DisplayName);
        }

        private void ShowPatientEmptyPanel(string message)
        {
            activePatientSection = string.Empty;
            lblPatientEmptyInfo.Text = message;
            SetPatientActionButtonsEnabled(false);
            ShowPatientActionPanel(pnlPatientEmptyPanel, "Panel pacjenta");
            tabControl.SelectedTab = tabPatient;
        }

        private void ShowPatientMessagesPanel()
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            activePatientSection = PatientSectionMessages;
            ShowPatientActionPanel(pnlPatientNotesPanel, "Wiadomości i notatki pacjenta");
            LoadPatientNotes();
        }

        private void ShowPatientPlannedPanel()
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            activePatientSection = PatientSectionPlanned;
            ShowPatientActionPanel(pnlPatientPlannedPanel, "Zaplanowane wizyty");
            IEnumerable<Appointment> appointments = dataStore.GetAllAppointmentsForPatient(selectedPatient.Id)
                .Where(a => a.Status == AppointmentStatus.Reserved && a.StartAt >= DateTime.Now)
                .OrderBy(a => a.StartAt);
            LoadAppointmentSummaryGrid(dgvPatientPlanned, appointments);
            RefreshPlannedAppointmentDetails(GetSelectedPatientPlannedAppointment());
        }

        private void ShowPatientHistoryPanel()
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            activePatientSection = PatientSectionHistory;
            ShowPatientActionPanel(pnlPatientHistoryPanel, "Historia wizyt");
            IEnumerable<Appointment> appointments = dataStore.GetAllAppointmentsForPatient(selectedPatient.Id)
                .Where(a => a.Status == AppointmentStatus.Cancelled || a.StartAt < DateTime.Now)
                .OrderByDescending(a => a.StartAt);
            LoadAppointmentSummaryGrid(dgvPatientHistory, appointments);
        }

        private void ShowPatientBookingPanel()
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            activePatientSection = PatientSectionBooking;
            PreparePatientBookingPanel();
            ShowPatientActionPanel(pnlPatientBookingPanel, "Umów wizytę");
        }

        private void PreparePatientBookingPanel()
        {
            dgvPatientBookingSlots.Rows.Clear();
            Patient patient = dataStore.GetPatient(selectedPatient.Id) ?? selectedPatient;
            selectedPatient = patient;

            bool isBlocked = patient.IsBlocked;
            cmbPatientBookingService.Enabled = !isBlocked;
            cmbPatientBookingDoctor.Enabled = false;
            cmbPatientBookingRange.Enabled = false;
            btnPatientBookingNext.Enabled = !isBlocked && GetSelectedBookingService() != null;
            btnPatientBookingSearch.Enabled = false;

            if (isBlocked)
            {
                lblPatientBookingInfo.ForeColor = SismedTheme.Danger;
                lblPatientBookingInfo.Text = "Pacjent ma aktywną blokadę rezerwacji do "
                    + patient.BlockedUntil.Value.ToString("dd.MM.yyyy")
                    + ". Nie można umówić nowej wizyty.";
                return;
            }

            lblPatientBookingInfo.ForeColor = SismedTheme.Muted;
            lblPatientBookingInfo.Text = "Wybierz usługę lub specjalizację, a następnie przejdź dalej.";
        }

        private void cmbPatientBookingService_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvPatientBookingSlots.Rows.Clear();
            cmbPatientBookingDoctor.DataSource = null;
            cmbPatientBookingDoctor.Enabled = false;
            cmbPatientBookingRange.Enabled = false;
            btnPatientBookingSearch.Enabled = false;
            btnPatientBookingNext.Enabled = selectedPatient != null
                && !selectedPatient.IsBlocked
                && GetSelectedBookingService() != null;

            if (GetSelectedBookingService() != null)
            {
                lblPatientBookingInfo.ForeColor = SismedTheme.Muted;
                lblPatientBookingInfo.Text = "Wybrano usługę. Kliknij Dalej, aby wybrać lekarza i zakres terminów.";
            }
        }

        private void btnPatientBookingNext_Click(object sender, EventArgs e)
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            MedicalService service = GetSelectedBookingService();
            if (service == null)
            {
                ShowError("Wybierz usługę lub specjalizację.");
                return;
            }

            LoadBookingDoctors(service);
        }

        private void LoadBookingDoctors(MedicalService service)
        {
            var doctors = dataStore.GetDoctors()
                .Where(d => string.Equals(d.Specialization, service.Specialization, StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToList();

            var options = new List<BookingDoctorOption>
            {
                new BookingDoctorOption { Specialization = service.Specialization }
            };
            options.AddRange(doctors.Select(d => new BookingDoctorOption
            {
                Doctor = d,
                Specialization = service.Specialization
            }));

            cmbPatientBookingDoctor.DisplayMember = "Text";
            cmbPatientBookingDoctor.DataSource = options;
            cmbPatientBookingDoctor.SelectedIndex = 0;
            cmbPatientBookingDoctor.Enabled = doctors.Count > 0;
            cmbPatientBookingRange.Enabled = doctors.Count > 0;
            btnPatientBookingSearch.Enabled = doctors.Count > 0;
            dgvPatientBookingSlots.Rows.Clear();

            lblPatientBookingInfo.ForeColor = doctors.Count > 0 ? SismedTheme.Muted : SismedTheme.Danger;
            lblPatientBookingInfo.Text = doctors.Count > 0
                ? "Wybierz lekarza albo opcję Dowolny lekarz, zakres dni i kliknij Szukaj terminów."
                : "Brak lekarzy dla wybranej specjalizacji.";
        }

        private void btnPatientBookingSearch_Click(object sender, EventArgs e)
        {
            SearchPatientBookingSlots();
        }

        private void SearchPatientBookingSlots()
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            if (selectedPatient.IsBlocked)
            {
                ShowError("Pacjent ma aktywną blokadę rezerwacji do "
                    + selectedPatient.BlockedUntil.Value.ToString("dd.MM.yyyy") + ".");
                return;
            }

            MedicalService service = GetSelectedBookingService();
            BookingDoctorOption doctorOption = cmbPatientBookingDoctor.SelectedItem as BookingDoctorOption;
            BookingRangeOption rangeOption = cmbPatientBookingRange.SelectedItem as BookingRangeOption;

            if (service == null || doctorOption == null || rangeOption == null)
            {
                ShowError("Wybierz usługę, lekarza i zakres wyszukiwania.");
                return;
            }

            IEnumerable<Doctor> doctors = GetBookingDoctorsForSearch(service, doctorOption);
            var slots = new List<PatientBookingSlot>();

            for (int offset = 0; offset < rangeOption.Days; offset++)
            {
                DateTime date = DateTime.Today.AddDays(offset);
                foreach (Doctor doctor in doctors)
                {
                    foreach (AvailableSlot slot in dataStore.GetAvailableSlots(doctor.Id, date))
                    {
                        if (!HasSelectedPatientBookingConflict(doctor.Specialization, slot.StartAt))
                        {
                            slots.Add(new PatientBookingSlot
                            {
                                Doctor = doctor,
                                Service = service,
                                StartAt = slot.StartAt
                            });
                        }
                    }
                }
            }

            LoadPatientBookingSlots(slots
                .OrderBy(s => s.StartAt)
                .ThenBy(s => s.Doctor.LastName)
                .ThenBy(s => s.Doctor.FirstName));
        }

        private IEnumerable<Doctor> GetBookingDoctorsForSearch(MedicalService service, BookingDoctorOption doctorOption)
        {
            if (!doctorOption.IsAnyDoctor)
            {
                return new List<Doctor> { doctorOption.Doctor };
            }

            return dataStore.GetDoctors()
                .Where(d => string.Equals(d.Specialization, service.Specialization, StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToList();
        }

        private bool HasSelectedPatientBookingConflict(string specialization, DateTime startAt)
        {
            return dataStore.GetAllAppointmentsForPatient(selectedPatient.Id)
                .Any(a =>
                {
                    if (a.Status != AppointmentStatus.Reserved)
                    {
                        return false;
                    }

                    if (a.StartAt == startAt)
                    {
                        return true;
                    }

                    Doctor doctor = dataStore.GetDoctor(a.DoctorId);
                    return doctor != null
                        && a.StartAt.Date == startAt.Date
                        && string.Equals(doctor.Specialization, specialization, StringComparison.OrdinalIgnoreCase);
                });
        }

        private void LoadPatientBookingSlots(IEnumerable<PatientBookingSlot> slots)
        {
            dgvPatientBookingSlots.Rows.Clear();
            CultureInfo polish = CultureInfo.GetCultureInfo("pl-PL");

            foreach (PatientBookingSlot slot in slots)
            {
                int rowIndex = dgvPatientBookingSlots.Rows.Add(
                    slot.StartAt.ToString("dd.MM.yyyy"),
                    polish.DateTimeFormat.GetDayName(slot.StartAt.DayOfWeek),
                    slot.StartAt.ToString("HH:mm"),
                    slot.Doctor.DisplayName,
                    slot.Service.Name + " / " + slot.Doctor.Specialization,
                    "Umów");
                dgvPatientBookingSlots.Rows[rowIndex].Tag = slot;
            }

            dgvPatientBookingSlots.ClearSelection();
            lblPatientBookingInfo.ForeColor = dgvPatientBookingSlots.Rows.Count > 0
                ? SismedTheme.Success
                : SismedTheme.Warning;
            lblPatientBookingInfo.Text = dgvPatientBookingSlots.Rows.Count > 0
                ? "Znaleziono wolne terminy: " + dgvPatientBookingSlots.Rows.Count + ". Kliknij Umów przy wybranym terminie."
                : "Nie znaleziono wolnych terminów w wybranym zakresie.";
        }

        private void dgvPatientBookingSlots_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvPatientBookingSlots.Columns[e.ColumnIndex].Name != "bookAction")
            {
                return;
            }

            PatientBookingSlot slot = dgvPatientBookingSlots.Rows[e.RowIndex].Tag as PatientBookingSlot;
            if (slot == null)
            {
                return;
            }

            ReservePatientBookingSlot(slot);
        }

        private void ReservePatientBookingSlot(PatientBookingSlot slot)
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "Czy umówić wizytę pacjenta " + selectedPatient.DisplayName
                + " na " + slot.StartAt.ToString("dd.MM.yyyy HH:mm")
                + " u lekarza " + slot.Doctor.DisplayName + "?",
                "SISMED",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                dataStore.ReserveAppointment(slot.Doctor.Id, selectedPatient.Id, slot.StartAt);
                selectedPatient = dataStore.GetPatient(selectedPatient.Id) ?? selectedPatient;
                RefreshPatientCard(selectedPatient);
                RefreshReservedAppointments();
                LoadCalendar();
                LoadSlots();
                RefreshReceptionStats();
                MessageBox.Show("Wizyta została zarezerwowana.", "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetStatus("Zarezerwowano wizytę: " + slot.StartAt.ToString("dd.MM.yyyy HH:mm"));
                ShowPatientPlannedPanel();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                SearchPatientBookingSlots();
            }
        }

        private MedicalService GetSelectedBookingService()
        {
            BookingServiceOption option = cmbPatientBookingService.SelectedItem as BookingServiceOption;
            return option == null ? null : option.Service;
        }

        private void ShowPatientActionPanel(Panel panel, string title)
        {
            pnlPatientEmptyPanel.Visible = false;
            pnlPatientResultsPanel.Visible = false;
            pnlPatientNotesPanel.Visible = false;
            pnlPatientPlannedPanel.Visible = false;
            pnlPatientHistoryPanel.Visible = false;
            pnlPatientBookingPanel.Visible = false;

            lblPatientActionTitle.Text = title;
            panel.Visible = true;
            panel.BringToFront();
        }

        private void RefreshActivePatientSection()
        {
            if (selectedPatient == null)
            {
                return;
            }

            if (activePatientSection == PatientSectionPlanned)
            {
                ShowPatientPlannedPanel();
            }
            else if (activePatientSection == PatientSectionHistory)
            {
                ShowPatientHistoryPanel();
            }
            else if (activePatientSection == PatientSectionBooking)
            {
                ShowPatientBookingPanel();
            }
            else if (activePatientSection == PatientSectionMessages)
            {
                ShowPatientMessagesPanel();
            }
        }

        private void btnPatientMessages_Click(object sender, EventArgs e)
        {
            ShowPatientMessagesPanel();
        }

        private void btnPatientBook_Click(object sender, EventArgs e)
        {
            ShowPatientBookingPanel();
        }

        private void btnPatientPlanned_Click(object sender, EventArgs e)
        {
            ShowPatientPlannedPanel();
        }

        private void btnPatientHistory_Click(object sender, EventArgs e)
        {
            ShowPatientHistoryPanel();
        }

        private void btnAddPatientNote_Click(object sender, EventArgs e)
        {
            if (!EnsurePatientSelected())
            {
                return;
            }

            string noteText = txtPatientNote.Text.Trim();
            if (string.IsNullOrWhiteSpace(noteText))
            {
                ShowError("Treść notatki nie może być pusta.");
                return;
            }

            try
            {
                dataStore.AddPatientNote(selectedPatient.Id, noteText, GetCurrentEmployeeName());
                txtPatientNote.Text = string.Empty;
                RefreshPatientCard(selectedPatient);
                ShowPatientMessagesPanel();
                SetStatus("Dodano notatkę pacjenta.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void btnDeletePatientNote_Click(object sender, EventArgs e)
        {
            PatientNote note = GetSelectedPatientNote();
            if (note == null)
            {
                ShowError("Wybierz notatkę do usunięcia.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Czy usunąć wybraną notatkę pacjenta?",
                "SISMED",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            dataStore.DeletePatientNote(note.Id);
            RefreshPatientCard(selectedPatient);
            ShowPatientMessagesPanel();
            SetStatus("Usunięto notatkę pacjenta.");
        }

        private void LoadPatientNotes()
        {
            dgvPatientNotes.Rows.Clear();
            if (selectedPatient == null)
            {
                return;
            }

            foreach (PatientNote note in dataStore.GetPatientNotes(selectedPatient.Id))
            {
                int rowIndex = dgvPatientNotes.Rows.Add(
                    note.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                    Safe(note.CreatedByEmployee),
                    note.Text ?? string.Empty);
                dgvPatientNotes.Rows[rowIndex].Tag = note;
            }

            dgvPatientNotes.ClearSelection();
        }

        private PatientNote GetSelectedPatientNote()
        {
            if (dgvPatientNotes.CurrentRow == null)
            {
                return null;
            }

            return dgvPatientNotes.CurrentRow.Tag as PatientNote;
        }

        private void LoadAppointmentSummaryGrid(DataGridView grid, IEnumerable<Appointment> appointments)
        {
            grid.Rows.Clear();

            foreach (Appointment appointment in appointments)
            {
                Doctor doctor = dataStore.GetDoctor(appointment.DoctorId);
                int rowIndex = grid.Rows.Add(
                    appointment.StartAt.ToString("dd.MM.yyyy HH:mm"),
                    GetAppointmentServiceName(doctor),
                    doctor == null ? "-" : doctor.DisplayName,
                    GetPatientAppointmentStatus(appointment));
                grid.Rows[rowIndex].Tag = appointment;
            }

            grid.ClearSelection();
        }

        private void dgvPatientPlanned_SelectionChanged(object sender, EventArgs e)
        {
            RefreshPlannedAppointmentDetails(GetSelectedPatientPlannedAppointment());
        }

        private Appointment GetSelectedPatientPlannedAppointment()
        {
            if (dgvPatientPlanned.CurrentRow == null)
            {
                return null;
            }

            return dgvPatientPlanned.CurrentRow.Tag as Appointment;
        }

        private void RefreshPlannedAppointmentDetails(Appointment appointment)
        {
            if (appointment == null || selectedPatient == null)
            {
                lblPlannedAppointmentDetails.Text = "Wybierz zaplanowaną wizytę, aby zobaczyć szczegóły.";
                lblPlannedAppointmentTimeLeft.Text = string.Empty;
                txtCancelAppointmentReason.Text = string.Empty;
                btnCancelPatientAppointment.Enabled = false;
                btnSwapPatientAppointment.Enabled = false;
                return;
            }

            Doctor doctor = dataStore.GetDoctor(appointment.DoctorId);
            string doctorName = doctor == null ? "-" : doctor.DisplayName;
            string serviceName = GetAppointmentServiceName(doctor);

            lblPlannedAppointmentDetails.Text =
                "Data: " + appointment.StartAt.ToString("dd.MM.yyyy")
                + "    Godzina: " + appointment.StartAt.ToString("HH:mm")
                + Environment.NewLine
                + "Lekarz: " + doctorName
                + "    Usługa: " + serviceName
                + Environment.NewLine
                + "Status: " + GetPatientAppointmentStatus(appointment)
                + "    Pacjent: " + selectedPatient.DisplayName;

            lblPlannedAppointmentTimeLeft.Text = "Do wizyty zostało: " + FormatTimeUntilAppointment(appointment.StartAt);
            btnCancelPatientAppointment.Enabled = appointment.Status == AppointmentStatus.Reserved
                && appointment.StartAt >= DateTime.Now;
            btnSwapPatientAppointment.Enabled = appointment.Status == AppointmentStatus.Reserved
                && appointment.StartAt >= DateTime.Now;
        }

        private void btnSwapPatientAppointment_Click(object sender, EventArgs e)
        {
            Appointment appointment = GetSelectedPatientPlannedAppointment();
            if (appointment == null)
            {
                ShowError("Wybierz zaplanowaną wizytę do zamiany.");
                return;
            }

            Patient previousPatient = selectedPatient;
            if (previousPatient == null)
            {
                ShowError("Najpierw wybierz pacjenta.");
                return;
            }

            using (var dialog = new PatientSwapDialog(dataStore, previousPatient.Id))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                Patient newPatient = dialog.SelectedPatient;
                if (newPatient == null)
                {
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Czy przenieść wizytę z pacjenta "
                    + previousPatient.DisplayName
                    + " na pacjenta "
                    + newPatient.DisplayName
                    + "?"
                    + Environment.NewLine
                    + appointment.StartAt.ToString("dd.MM.yyyy HH:mm"),
                    "SISMED",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                try
                {
                    dataStore.SwapAppointmentPatient(appointment.Id, newPatient.Id, GetCurrentEmployeeName());
                    selectedPatient = dataStore.GetPatient(newPatient.Id) ?? newPatient;
                    FillPatientSearchFields(selectedPatient);
                    RefreshPatientCard(selectedPatient);
                    RefreshReservedAppointments();
                    LoadCalendar();
                    RefreshReceptionStats();
                    ShowPatientPlannedPanel();
                    SetStatus("Wizyta została przeniesiona na pacjenta: " + selectedPatient.DisplayName);
                    MessageBox.Show(
                        "Wizyta została przeniesiona na pacjenta " + selectedPatient.DisplayName + ".",
                        "SISMED",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void btnCancelPatientAppointment_Click(object sender, EventArgs e)
        {
            Appointment appointment = GetSelectedPatientPlannedAppointment();
            if (appointment == null)
            {
                ShowError("Wybierz zaplanowaną wizytę.");
                return;
            }

            string reason = txtCancelAppointmentReason.Text.Trim();
            if (string.IsNullOrWhiteSpace(reason))
            {
                ShowError("Podaj powód anulowania wizyty.");
                return;
            }

            bool lateCancellation = appointment.StartAt > DateTime.Now
                && appointment.StartAt.Subtract(DateTime.Now).TotalHours < 12;

            string message = "Czy anulować wybraną wizytę?"
                + Environment.NewLine
                + appointment.StartAt.ToString("dd.MM.yyyy HH:mm")
                + Environment.NewLine
                + (lateCancellation
                    ? "Anulowanie mniej niż 12 godzin przed terminem doda pacjentowi ostrzeżenie."
                    : "Anulowanie następuje więcej niż 12 godzin przed terminem i nie doda ostrzeżenia.");

            DialogResult result = MessageBox.Show(
                message,
                "SISMED",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                dataStore.CancelAppointment(appointment.Id, reason);
                selectedPatient = dataStore.GetPatient(selectedPatient.Id) ?? selectedPatient;
                txtCancelAppointmentReason.Text = string.Empty;
                RefreshPatientCard(selectedPatient);
                RefreshReservedAppointments();
                LoadCalendar();
                LoadSlots();
                RefreshReceptionStats();
                ShowPatientPlannedPanel();
                SetStatus("Anulowano wizytę pacjenta.");
                MessageBox.Show("Wizyta została anulowana.", "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private static string FormatTimeUntilAppointment(DateTime startAt)
        {
            TimeSpan left = startAt.Subtract(DateTime.Now);
            if (left.TotalMinutes <= 0)
            {
                return "termin minął";
            }

            if (left.TotalDays >= 1)
            {
                return string.Format(
                    "{0} dni {1} godz. {2} min",
                    (int)left.TotalDays,
                    left.Hours,
                    left.Minutes);
            }

            return string.Format("{0} godz. {1} min", (int)left.TotalHours, left.Minutes);
        }

        private string GetAppointmentServiceName(Doctor doctor)
        {
            if (doctor == null)
            {
                return "-";
            }

            MedicalService service = dataStore.GetServices()
                .FirstOrDefault(s => string.Equals(s.Specialization, doctor.Specialization, StringComparison.OrdinalIgnoreCase));

            return service == null ? Safe(doctor.Specialization) : Safe(service.Name);
        }

        private static string GetPatientAppointmentStatus(Appointment appointment)
        {
            if (appointment.Status == AppointmentStatus.Reserved && appointment.StartAt >= DateTime.Now)
            {
                return "Zaplanowana";
            }

            if (appointment.Status == AppointmentStatus.Reserved)
            {
                return "Historyczna";
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                return "Anulowana";
            }

            return appointment.StatusText;
        }

        private bool EnsurePatientSelected()
        {
            if (selectedPatient != null)
            {
                SetPatientActionButtonsEnabled(true);
                return true;
            }

            ShowPatientEmptyPanel("Najpierw wyszukaj i wybierz pacjenta.");
            ShowError("Najpierw wyszukaj i wybierz pacjenta.");
            return false;
        }

        private void SetPatientActionButtonsEnabled(bool enabled)
        {
            btnPatientMessages.Enabled = enabled;
            btnPatientBook.Enabled = enabled;
            btnPatientPlanned.Enabled = enabled;
            btnPatientHistory.Enabled = enabled;
            btnAddPatientNote.Enabled = enabled;
            btnDeletePatientNote.Enabled = enabled;
        }

        private string GetCurrentEmployeeName()
        {
            if (currentEmployee == null)
            {
                return "Pracownik";
            }

            return string.IsNullOrWhiteSpace(currentEmployee.FullName)
                ? Safe(currentEmployee.Login)
                : currentEmployee.FullName;
        }

        private static string FormatDocumentDate(DateTime date)
        {
            return date == DateTime.MinValue ? "-" : date.ToString("dd.MM.yyyy HH:mm");
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
                RefreshPatientDetailCard(null, 0);
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
            int warningCount = Math.Max(current.WarningCount, warnings.Count);
            lblPatientWarnings.Text = "Ostrzezenia: " + warningCount + "/3";
            lblPatientNotes.Text = "Notatka: " + (notes.Count > 0 ? Truncate(notes[0].Text, 44) : "-");
            lblPatientStatus.Text = current.IsBlocked
                ? "Blokada do " + current.BlockedUntil.Value.ToString("dd.MM.yyyy")
                : string.Empty;
            RefreshPatientDetailCard(current, warningCount);
        }

        private void RefreshPatientDetailCard(Patient patient, int warningCount)
        {
            if (patient == null)
            {
                lblPatientPanelName.Text = "- Brak wybranego pacjenta -";
                lblPatientPanelPesel.Text = "PESEL: -";
                lblPatientPanelBirthDate.Text = "Data ur.: -";
                lblPatientPanelPhone.Text = "Tel: -";
                lblPatientPanelEmail.Text = "E-mail: -";
                lblPatientPanelAddress.Text = "Adres: -";
                lblPatientPanelWarnings.Text = "Ostrzeżenia: 0";
                lblPatientPanelBlock.Text = "Blokada rezerwacji: nie";
                lblPatientPanelBlock.ForeColor = SismedTheme.Success;
                SetPatientActionButtonsEnabled(false);
                return;
            }

            lblPatientPanelName.Text = patient.DisplayName;
            lblPatientPanelPesel.Text = "PESEL: " + Safe(patient.Pesel);
            lblPatientPanelBirthDate.Text = "Data ur.: "
                + (patient.BirthDate.HasValue ? patient.BirthDate.Value.ToString("dd.MM.yyyy") : "-");
            lblPatientPanelPhone.Text = "Tel: " + Safe(patient.Phone);
            lblPatientPanelEmail.Text = "E-mail: " + Safe(patient.Email);
            lblPatientPanelAddress.Text = "Adres: " + Safe(patient.Address);
            lblPatientPanelWarnings.Text = "Ostrzeżenia: " + warningCount;
            lblPatientPanelBlock.Text = patient.IsBlocked
                ? "Blokada rezerwacji: do " + patient.BlockedUntil.Value.ToString("dd.MM.yyyy")
                : "Blokada rezerwacji: nie";
            lblPatientPanelBlock.ForeColor = patient.IsBlocked ? SismedTheme.Danger : SismedTheme.Success;
            SetPatientActionButtonsEnabled(true);
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
