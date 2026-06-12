using System;
using System.Collections.Generic;
using System.Drawing;
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
            LoadDoctorLists();
            RefreshPatientCard(null);
            LoadCalendar();
            RefreshReservedAppointments();
            SetStatus("Zalogowano: " + (currentEmployee == null ? "pracownik" : currentEmployee.DisplayName)
                + " | " + AppServices.StorageInfo);
        }

        private void MoveForm(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            selectedPatient = dataStore.FindPatient(txtSearch.Text);
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
            SetStatus("Wybrano pacjenta: " + selectedPatient.DisplayName);
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
                    txtSearch.Text = selectedPatient.Pesel;
                    RefreshPatientCard(selectedPatient);
                    RefreshReservedAppointments();
                    SetStatus("Dodano pacjenta: " + selectedPatient.DisplayName);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        private void btnOpenCalendar_Click(object sender, EventArgs e)
        {
            using (var calendar = new AppointmentCalendarWindow(dataStore))
            {
                calendar.ShowDialog(this);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Form owner = Owner;
            if (owner != null && !owner.IsDisposed)
            {
                owner.Show();
            }
            else
            {
                new login_page().Show();
            }

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
        }

        private void btnSwapFind_Click(object sender, EventArgs e)
        {
            swapPatient = dataStore.FindPatient(txtSwapSearch.Text);
            if (swapPatient == null)
            {
                lblSwapResult.ForeColor = Color.OrangeRed;
                lblSwapResult.Text = "Nie znaleziono pacjenta";
                btnSwap.Enabled = false;
                return;
            }

            lblSwapResult.ForeColor = Color.LightGreen;
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
                txtSearch.Text = selectedPatient.Pesel;
                RefreshPatientCard(selectedPatient);
                RefreshReservedAppointments();
                LoadCalendar();
                SetStatus("Zamieniono pacjenta na wizycie.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
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

        private void RefreshPatientCard(Patient patient)
        {
            if (patient == null)
            {
                lblPatientName.Text = "- Brak wybranego pacjenta -";
                lblPatientPesel.Text = "PESEL: -";
                lblPatientPhone.Text = "Tel: -";
                lblPatientWarnings.Text = "Ostrzezenia: 0/3";
                lblPatientStatus.Text = string.Empty;
                return;
            }

            Patient current = dataStore.GetPatient(patient.Id) ?? patient;
            selectedPatient = current;

            lblPatientName.Text = current.DisplayName;
            lblPatientPesel.Text = "PESEL: " + current.Pesel;
            lblPatientPhone.Text = "Tel: " + (string.IsNullOrWhiteSpace(current.Phone) ? "-" : current.Phone);
            lblPatientWarnings.Text = "Ostrzezenia: " + current.WarningCount + "/3";
            lblPatientStatus.Text = current.IsBlocked
                ? "Blokada do " + current.BlockedUntil.Value.ToString("dd.MM.yyyy")
                : string.Empty;
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

        private void SetStatus(string message)
        {
            lblStatus.Text = message;
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
