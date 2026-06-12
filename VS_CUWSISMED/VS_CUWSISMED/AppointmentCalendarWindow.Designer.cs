using System.Drawing;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    partial class AppointmentCalendarWindow
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox cmbDoctor;
        private DateTimePicker dtpDate;
        private Button btnLoad;
        private DataGridView dgvCalendar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cmbDoctor = new ComboBox();
            dtpDate = new DateTimePicker();
            btnLoad = new Button();
            dgvCalendar = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(dgvCalendar)).BeginInit();
            SuspendLayout();

            BackColor = Color.FromArgb(10, 12, 35);
            ClientSize = new Size(760, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Kalendarz wizyt SISMED";

            cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDoctor.Location = new Point(18, 18);
            cmbDoctor.Size = new Size(260, 24);

            dtpDate.CustomFormat = "dd.MM.yyyy";
            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.Location = new Point(294, 18);
            dtpDate.Size = new Size(130, 22);

            btnLoad.Location = new Point(438, 16);
            btnLoad.Size = new Size(120, 28);
            btnLoad.Text = "Pokaz";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;

            dgvCalendar.AllowUserToAddRows = false;
            dgvCalendar.AllowUserToDeleteRows = false;
            dgvCalendar.BackgroundColor = Color.FromArgb(10, 12, 35);
            dgvCalendar.BorderStyle = BorderStyle.None;
            dgvCalendar.ColumnHeadersHeight = 32;
            dgvCalendar.Location = new Point(18, 58);
            dgvCalendar.MultiSelect = false;
            dgvCalendar.ReadOnly = true;
            dgvCalendar.RowHeadersVisible = false;
            dgvCalendar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCalendar.Size = new Size(724, 444);
            dgvCalendar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTime",
                HeaderText = "Godzina",
                Width = 120
            });
            dgvCalendar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPatient",
                HeaderText = "Pacjent",
                Width = 420
            });
            dgvCalendar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStatus",
                HeaderText = "Status",
                Width = 160
            });

            Controls.Add(cmbDoctor);
            Controls.Add(dtpDate);
            Controls.Add(btnLoad);
            Controls.Add(dgvCalendar);
            ((System.ComponentModel.ISupportInitialize)(dgvCalendar)).EndInit();
            ResumeLayout(false);
        }
    }
}
