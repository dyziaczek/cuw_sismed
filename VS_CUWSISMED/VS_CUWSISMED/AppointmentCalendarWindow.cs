using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public partial class AppointmentCalendarWindow : Form
    {
        private readonly IClinicDataStore dataStore;

        public AppointmentCalendarWindow()
            : this(AppServices.DataStore)
        {
        }

        public AppointmentCalendarWindow(IClinicDataStore dataStore)
        {
            this.dataStore = dataStore;
            InitializeComponent();
            LoadDoctors();
            LoadCalendar();
        }

        private void LoadDoctors()
        {
            IReadOnlyList<Doctor> doctors = dataStore.GetDoctors();
            cmbDoctor.DataSource = new List<Doctor>(doctors);
            cmbDoctor.DisplayMember = "DisplayName";
            cmbDoctor.ValueMember = "Id";
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadCalendar();
        }

        private void LoadCalendar()
        {
            Doctor doctor = cmbDoctor.SelectedItem as Doctor;
            dgvCalendar.Rows.Clear();

            if (doctor == null)
            {
                return;
            }

            foreach (Appointment appointment in dataStore.GetAppointmentsForDoctor(doctor.Id, dtpDate.Value.Date))
            {
                Patient patient = appointment.PatientId.HasValue
                    ? dataStore.GetPatient(appointment.PatientId.Value)
                    : null;

                int rowIndex = dgvCalendar.Rows.Add(
                    appointment.StartAt.ToString("HH:mm"),
                    patient == null ? string.Empty : patient.DisplayName,
                    appointment.StatusText);

                DataGridViewRow row = dgvCalendar.Rows[rowIndex];
                row.Tag = appointment;
                if (appointment.Status == AppointmentStatus.Free)
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(80, 220, 120);
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(255, 120, 120);
                }
            }
        }
    }
}
