using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace VS_CUWSISMED
{
    partial class main_app
    {
        private System.ComponentModel.IContainer components = null;

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

            Color navy = Color.FromArgb(8, 21, 64);
            Color blue = Color.FromArgb(26, 72, 168);
            Color magenta = Color.FromArgb(218, 0, 148);
            Color surface = Color.FromArgb(246, 248, 252);
            Color card = Color.White;
            Color border = Color.FromArgb(218, 225, 238);
            Color text = Color.FromArgb(22, 32, 58);
            Color muted = Color.FromArgb(91, 103, 128);

            Font navFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            Font labelFont = new Font("Segoe UI", 9f, FontStyle.Bold);
            Font normalFont = new Font("Segoe UI", 9f);

            SuspendLayout();

            BackColor = surface;
            ClientSize = new Size(1280, 760);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1100, 680);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CUW SISMED";
            WindowState = FormWindowState.Maximized;

            pnlNavigation = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = navy
            };

            picLogo = new PictureBox
            {
                Image = Properties.Resources.LOGO,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(18, 20),
                Size = new Size(224, 88),
                BackColor = Color.Transparent
            };

            lblNavTitle = new Label
            {
                Text = "Centrum Umawiania Wizyt",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(176, 193, 230),
                Location = new Point(22, 112),
                Size = new Size(216, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnNavCalendar = CreateNavButton("KALENDARZ WIZYT", 170, navFont, blue);
            btnNavReception = CreateNavButton("RECEPCJA", 222, navFont, magenta);
            btnNavDocuments = CreateNavButton("DOKUMENTY", 274, navFont, blue);
            btnNavPersonnel = CreateNavButton("PERSONEL", 326, navFont, blue);
            btnNavCalendar.Click += btnNavCalendar_Click;
            btnNavReception.Click += btnNavReception_Click;
            btnNavDocuments.Click += btnNavDocuments_Click;
            btnNavPersonnel.Click += btnNavPersonnel_Click;

            btnLogout = new Guna2Button
            {
                Text = "Wyloguj",
                Font = navFont,
                ForeColor = Color.White,
                FillColor = Color.Transparent,
                BorderColor = magenta,
                BorderThickness = 1,
                BorderRadius = 8,
                Location = new Point(20, 690),
                Size = new Size(220, 38),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnLogout.Click += btnLogout_Click;

            pnlNavigation.Controls.AddRange(new Control[]
            {
                picLogo, lblNavTitle,
                btnNavCalendar, btnNavReception, btnNavDocuments, btnNavPersonnel,
                btnLogout
            });

            pnlShell = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = surface
            };

            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.White
            };

            lblScreenTitle = new Label
            {
                Text = "RECEPCJA",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = navy,
                Location = new Point(24, 16),
                Size = new Size(420, 32)
            };

            lblCurrentUser = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = muted,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(600, 19),
                Size = new Size(360, 28),
                TextAlign = ContentAlignment.MiddleRight
            };

            btnClose = new Guna2Button
            {
                Text = "X",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = magenta,
                BorderRadius = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(970, 16),
                Size = new Size(36, 32)
            };
            btnClose.Click += (sender, args) => Close();

            pnlTopBar.Controls.AddRange(new Control[] { lblScreenTitle, lblCurrentUser, btnClose });
            pnlTopBar.Resize += (sender, args) =>
            {
                lblCurrentUser.Left = pnlTopBar.Width - 460;
                btnClose.Left = pnlTopBar.Width - 52;
            };

            pnlScreenHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = surface,
                Padding = new Padding(24)
            };

            BuildReceptionScreen(card, border, text, muted, magenta, blue, labelFont, normalFont);
            BuildCalendarScreen(card, border, text, muted, magenta, blue, labelFont, normalFont);
            BuildDocumentsScreen(card, border, text, muted, magenta, normalFont);
            BuildPersonnelScreen(card, border, text, muted, magenta, blue, labelFont, normalFont);

            pnlScreenHost.Controls.AddRange(new Control[]
            {
                pnlDocumentsScreen,
                pnlPersonnelScreen,
                pnlCalendarScreen,
                pnlReceptionScreen
            });

            pnlShell.Controls.Add(pnlScreenHost);
            pnlShell.Controls.Add(pnlTopBar);

            Controls.Add(pnlShell);
            Controls.Add(pnlNavigation);

            ResumeLayout(false);
        }

        private Guna2Button CreateNavButton(string text, int top, Font font, Color fill)
        {
            return new Guna2Button
            {
                Text = text,
                Font = font,
                ForeColor = Color.White,
                FillColor = fill,
                BorderRadius = 8,
                Location = new Point(20, top),
                Size = new Size(220, 40),
                TextAlign = HorizontalAlignment.Left,
                Padding = new Padding(12, 0, 0, 0)
            };
        }

        private void BuildReceptionScreen(
            Color card,
            Color border,
            Color text,
            Color muted,
            Color magenta,
            Color blue,
            Font labelFont,
            Font normalFont)
        {
            pnlReceptionScreen = CreateScreenPanel();

            pnlReceptionSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 340,
                BackColor = card,
                Padding = new Padding(18)
            };

            var lblSearch = new Label
            {
                Text = "WYSZUKAJ PACJENTA",
                Font = labelFont,
                ForeColor = muted,
                Location = new Point(18, 18),
                Size = new Size(292, 20)
            };

            txtPatientPesel = CreateTextBox("PESEL", 18, 48, 292);
            txtPatientFirstName = CreateTextBox("Imie", 18, 90, 140);
            txtPatientLastName = CreateTextBox("Nazwisko", 170, 90, 140);
            txtPatientBirthDate = CreateTextBox("Data urodzenia dd.MM.yyyy", 18, 132, 292);
            txtPatientPhone = CreateTextBox("Telefon", 18, 174, 140);
            txtPatientEmail = CreateTextBox("E-mail", 170, 174, 140);
            btnSearch = CreateActionButton("Szukaj", 18, 222, 140, magenta);
            btnSearch.Click += btnSearch_Click;
            btnClearPatientSearch = CreateActionButton("Wyczyść", 170, 222, 140, blue);
            btnClearPatientSearch.Click += btnClearPatientSearch_Click;
            btnAddPatient = CreateActionButton("+ Dodaj pacjenta", 18, 270, 292, Color.FromArgb(0, 130, 110));
            btnAddPatient.Click += btnAddPatient_Click;

            pnlPatientCard = new Panel
            {
                BackColor = Color.FromArgb(246, 248, 252),
                Location = new Point(18, 326),
                Size = new Size(292, 260),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblPatientName = CreateInfoLabel("- Brak wybranego pacjenta -", 12, 14, text, true);
            lblPatientPesel = CreateInfoLabel("PESEL: -", 12, 62, muted, false);
            lblPatientBirthDate = CreateInfoLabel("Data ur.: -", 12, 88, muted, false);
            lblPatientPhone = CreateInfoLabel("Tel: -", 12, 114, muted, false);
            lblPatientEmail = CreateInfoLabel("E-mail: -", 12, 140, muted, false);
            lblPatientWarnings = CreateInfoLabel("Ostrzezenia: 0/3", 12, 166, muted, false);
            lblPatientNotes = CreateInfoLabel("Notatka: -", 12, 192, muted, false);
            lblPatientStatus = CreateInfoLabel("", 12, 230, Color.OrangeRed, true);
            pnlPatientCard.Controls.AddRange(new Control[]
            {
                lblPatientName, lblPatientPesel, lblPatientBirthDate, lblPatientPhone,
                lblPatientEmail, lblPatientWarnings, lblPatientNotes, lblPatientStatus
            });

            pnlReceptionSidebar.Controls.AddRange(new Control[]
            {
                lblSearch, txtPatientPesel, txtPatientFirstName, txtPatientLastName,
                txtPatientBirthDate, txtPatientPhone, txtPatientEmail,
                btnSearch, btnClearPatientSearch, btnAddPatient, pnlPatientCard
            });

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Padding = new Point(16, 6)
            };

            tabBook = new TabPage { Text = "Umowienie wizyty", BackColor = Color.White, Padding = new Padding(16) };
            tabReserved = new TabPage { Text = "Zarezerwowane wizyty", BackColor = Color.White, Padding = new Padding(16) };

            pnlBookTop = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.White };
            lblBookDoctor = CreateInlineLabel("Lekarz:", 0, 16, muted, labelFont);
            cmbDoctor = CreateComboBox(66, 10, 270);
            lblBookDate = CreateInlineLabel("Data:", 360, 16, muted, labelFont);
            dtpBook = CreateDatePicker(412, 10);
            dtpBook.MinDate = DateTime.Today;
            btnLoadSlots = CreateActionButton("Pokaż dostępne terminy", 600, 10, 220, magenta);
            btnLoadSlots.Click += btnLoadSlots_Click;
            pnlBookTop.Controls.AddRange(new Control[]
            {
                lblBookDoctor, cmbDoctor, lblBookDate, dtpBook, btnLoadSlots
            });

            dgvSlots = CreateGrid();
            dgvSlots.Dock = DockStyle.Fill;
            dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Godzina", Width = 100 });
            dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDoctor", HeaderText = "Lekarz", Width = 220 });
            dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSpec", HeaderText = "Specjalizacja", Width = 200 });
            dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate", HeaderText = "Data", Width = 120 });

            btnReserve = CreateActionButton("Zarezerwuj wizytę", 0, 0, 210, blue);
            btnReserve.Dock = DockStyle.Bottom;
            btnReserve.Height = 42;
            btnReserve.Click += btnReserve_Click;

            tabBook.Controls.Add(dgvSlots);
            tabBook.Controls.Add(btnReserve);
            tabBook.Controls.Add(pnlBookTop);

            dgvReserved = CreateGrid();
            dgvReserved.Dock = DockStyle.Fill;
            dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rDate", HeaderText = "Data", Width = 100 });
            dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rTime", HeaderText = "Godzina", Width = 90 });
            dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rDoctor", HeaderText = "Lekarz", Width = 220 });
            dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rSpec", HeaderText = "Spec.", Width = 160 });
            dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rStatus", HeaderText = "Status", Width = 130 });
            dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rNote", HeaderText = "Uwagi", Width = 180 });

            pnlReservedActions = new Panel { Dock = DockStyle.Bottom, Height = 145, BackColor = Color.FromArgb(246, 248, 252) };
            btnCancel = CreateActionButton("Anuluj wizytę", 16, 18, 150, Color.FromArgb(190, 40, 40));
            btnCancel.Click += btnCancel_Click;
            txtSwapSearch = CreateTextBox("PESEL / Telefon", 16, 76, 250);
            btnSwapFind = CreateActionButton("Znajdź", 276, 76, 92, blue);
            btnSwapFind.Click += btnSwapFind_Click;
            lblSwapResult = new Label
            {
                Text = "",
                Location = new Point(382, 82),
                Size = new Size(260, 24),
                Font = normalFont,
                ForeColor = Color.Green
            };
            btnSwap = CreateActionButton("Zatwierdź zamianę", 660, 76, 180, magenta);
            btnSwap.Enabled = false;
            btnSwap.Click += btnSwap_Click;
            pnlReservedActions.Controls.AddRange(new Control[]
            {
                btnCancel, txtSwapSearch, btnSwapFind, lblSwapResult, btnSwap
            });

            tabReserved.Controls.Add(dgvReserved);
            tabReserved.Controls.Add(pnlReservedActions);

            tabControl.TabPages.Add(tabBook);
            tabControl.TabPages.Add(tabReserved);

            pnlReceptionScreen.Controls.Add(tabControl);
            pnlReceptionScreen.Controls.Add(pnlReceptionSidebar);
        }

        private void BuildCalendarScreen(
            Color card,
            Color border,
            Color text,
            Color muted,
            Color magenta,
            Color blue,
            Font labelFont,
            Font normalFont)
        {
            pnlCalendarScreen = CreateScreenPanel();

            pnlCalTop = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = card, Padding = new Padding(18) };
            lblCalDoctor = CreateInlineLabel("Lekarz:", 18, 24, muted, labelFont);
            cmbCalDoctor = CreateComboBox(82, 18, 280);
            lblCalDate = CreateInlineLabel("Data:", 386, 24, muted, labelFont);
            dtpCal = CreateDatePicker(440, 18);
            btnLoadCal = CreateActionButton("Pokaż grafik", 620, 18, 160, magenta);
            btnLoadCal.Click += btnLoadCal_Click;
            pnlCalTop.Controls.AddRange(new Control[]
            {
                lblCalDoctor, cmbCalDoctor, lblCalDate, dtpCal, btnLoadCal
            });

            dgvCal = CreateGrid();
            dgvCal.Dock = DockStyle.Fill;
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calTime", HeaderText = "Godzina", Width = 100 });
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calPatient", HeaderText = "Pacjent", Width = 300 });
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calStatus", HeaderText = "Status", Width = 160 });
            dgvCal.CellFormatting += DgvCal_CellFormatting;

            pnlCalendarScreen.Controls.Add(dgvCal);
            pnlCalendarScreen.Controls.Add(pnlCalTop);
        }

        private void BuildDocumentsScreen(
            Color card,
            Color border,
            Color text,
            Color muted,
            Color magenta,
            Font normalFont)
        {
            pnlDocumentsScreen = CreateScreenPanel();
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Padding = new Padding(36)
            };
            var label = new Label
            {
                Text = "Moduł dokumentów jest przygotowany w głównej nawigacji.",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = text,
                Dock = DockStyle.Top,
                Height = 52
            };
            var sub = new Label
            {
                Text = "W następnej iteracji można podłączyć formularze zgód, wydruki i dokumentację wizyt.",
                Font = normalFont,
                ForeColor = muted,
                Dock = DockStyle.Top,
                Height = 32
            };
            panel.Controls.Add(sub);
            panel.Controls.Add(label);
            pnlDocumentsScreen.Controls.Add(panel);
        }

        private void BuildPersonnelScreen(
            Color card,
            Color border,
            Color text,
            Color muted,
            Color magenta,
            Color blue,
            Font labelFont,
            Font normalFont)
        {
            pnlPersonnelScreen = CreateScreenPanel();

            pnlPersonnelTop = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = card, Padding = new Padding(18) };
            txtEmployeeSearch = CreateTextBox("Imię / nazwisko / PESEL / data urodzenia / login", 18, 20, 420);
            btnEmployeeSearch = CreateActionButton("Szukaj", 452, 20, 110, magenta);
            btnEmployeeSearch.Click += btnEmployeeSearch_Click;
            btnAddEmployee = CreateActionButton("+ Dodaj pracownika", 580, 20, 180, Color.FromArgb(0, 130, 110));
            btnAddEmployee.Click += btnAddEmployee_Click;
            btnDeactivateEmployee = CreateActionButton("Dezaktywuj", 774, 20, 140, Color.FromArgb(190, 40, 40));
            btnDeactivateEmployee.Click += btnDeactivateEmployee_Click;
            pnlPersonnelTop.Controls.AddRange(new Control[]
            {
                txtEmployeeSearch, btnEmployeeSearch, btnAddEmployee, btnDeactivateEmployee
            });

            dgvEmployees = CreateGrid();
            dgvEmployees.Dock = DockStyle.Left;
            dgvEmployees.Width = 620;
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "empName", HeaderText = "Pracownik", Width = 190 });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "empLogin", HeaderText = "Login", Width = 110 });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "empRole", HeaderText = "Rola", Width = 110 });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "empStatus", HeaderText = "Status", Width = 100 });
            dgvEmployees.Columns.Add(new DataGridViewTextBoxColumn { Name = "empDoctor", HeaderText = "Lekarz", Width = 80 });
            dgvEmployees.SelectionChanged += dgvEmployees_SelectionChanged;

            pnlEmployeeDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Padding = new Padding(28)
            };

            lblPersonnelAccess = new Label
            {
                Text = "",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(190, 40, 40)
            };

            lblEmployeeName = CreateDetailLabel("Imię i nazwisko: -", text, true);
            lblEmployeePesel = CreateDetailLabel("PESEL: -", muted, false);
            lblEmployeeBirthDate = CreateDetailLabel("Data urodzenia: -", muted, false);
            lblEmployeeLogin = CreateDetailLabel("Login: -", muted, false);
            lblEmployeeRole = CreateDetailLabel("Rola: -", muted, false);
            lblEmployeeStatus = CreateDetailLabel("Status: -", muted, false);
            lblEmployeeDoctor = CreateDetailLabel("Lekarz: -", muted, false);
            lblEmployeeSpecialization = CreateDetailLabel("Specjalizacja: -", muted, false);

            pnlEmployeeDetails.Controls.AddRange(new Control[]
            {
                lblEmployeeSpecialization, lblEmployeeDoctor, lblEmployeeStatus, lblEmployeeRole,
                lblEmployeeLogin, lblEmployeeBirthDate, lblEmployeePesel, lblEmployeeName,
                lblPersonnelAccess
            });

            pnlPersonnelScreen.Controls.Add(pnlEmployeeDetails);
            pnlPersonnelScreen.Controls.Add(dgvEmployees);
            pnlPersonnelScreen.Controls.Add(pnlPersonnelTop);
        }

        private Panel CreateScreenPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };
        }

        private Guna2TextBox CreateTextBox(string placeholder, int left, int top, int width)
        {
            return new Guna2TextBox
            {
                Location = new Point(left, top),
                Size = new Size(width, 36),
                PlaceholderText = placeholder,
                Font = new Font("Segoe UI", 9f),
                BorderColor = Color.FromArgb(218, 225, 238),
                BorderThickness = 1,
                FocusedState = { BorderColor = Color.FromArgb(218, 0, 148) }
            };
        }

        private Guna2Button CreateActionButton(string text, int left, int top, int width, Color fill)
        {
            return new Guna2Button
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, 36),
                BorderRadius = 8,
                FillColor = fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
        }

        private ComboBox CreateComboBox(int left, int top, int width)
        {
            return new ComboBox
            {
                Location = new Point(left, top),
                Size = new Size(width, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
        }

        private Guna2DateTimePicker CreateDatePicker(int left, int top)
        {
            return new Guna2DateTimePicker
            {
                Location = new Point(left, top),
                Size = new Size(150, 36),
                CustomFormat = "dd.MM.yyyy",
                Format = DateTimePickerFormat.Custom,
                Value = DateTime.Today,
                BorderColor = Color.FromArgb(218, 225, 238),
                BorderThickness = 1
            };
        }

        private DataGridView CreateGrid()
        {
            var grid = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 34,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9f),
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                RowTemplate = { Height = 30 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(8, 21, 64);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 0, 148);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            return grid;
        }

        private Label CreateInlineLabel(string text, int left, int top, Color color, Font font)
        {
            return new Label
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(70, 22),
                Font = font,
                ForeColor = color
            };
        }

        private Label CreateInfoLabel(string text, int left, int top, Color color, bool bold)
        {
            return new Label
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(240, 38),
                Font = new Font("Segoe UI", 9f, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color
            };
        }

        private Label CreateDetailLabel(string text, Color color, bool bold)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = bold ? 46 : 34,
                Font = new Font("Segoe UI", bold ? 13f : 10f, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color
            };
        }

        private void DgvCal_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dgvCal.Rows[e.RowIndex];
            string status = row.Cells["calStatus"].Value == null
                ? string.Empty
                : row.Cells["calStatus"].Value.ToString();

            if (status == "Wolny")
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(0, 130, 80);
            }
            else if (status == "Zajęty")
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(190, 40, 40);
            }
        }

        private Panel pnlNavigation, pnlShell, pnlTopBar, pnlScreenHost;
        private Panel pnlReceptionScreen, pnlCalendarScreen, pnlDocumentsScreen, pnlPersonnelScreen;
        private Panel pnlReceptionSidebar, pnlPatientCard, pnlBookTop, pnlCalTop;
        private Panel pnlReservedActions, pnlPersonnelTop, pnlEmployeeDetails;
        private PictureBox picLogo;
        private Label lblNavTitle, lblScreenTitle, lblCurrentUser, lblPersonnelAccess;
        private Label lblPatientName, lblPatientPesel, lblPatientBirthDate, lblPatientPhone;
        private Label lblPatientEmail, lblPatientWarnings, lblPatientNotes, lblPatientStatus;
        private Label lblBookDoctor, lblBookDate, lblCalDoctor, lblCalDate;
        private Label lblSwapResult;
        private Label lblEmployeeName, lblEmployeePesel, lblEmployeeBirthDate, lblEmployeeLogin;
        private Label lblEmployeeRole, lblEmployeeStatus, lblEmployeeDoctor, lblEmployeeSpecialization;
        private Guna2TextBox txtPatientPesel, txtPatientFirstName, txtPatientLastName, txtPatientBirthDate;
        private Guna2TextBox txtPatientPhone, txtPatientEmail, txtSwapSearch, txtEmployeeSearch;
        private Guna2Button btnNavCalendar, btnNavReception, btnNavDocuments, btnNavPersonnel;
        private Guna2Button btnSearch, btnClearPatientSearch, btnAddPatient, btnLogout, btnLoadSlots, btnReserve;
        private Guna2Button btnLoadCal, btnCancel, btnSwap, btnSwapFind, btnClose;
        private Guna2Button btnEmployeeSearch, btnAddEmployee, btnDeactivateEmployee;
        private ComboBox cmbDoctor, cmbCalDoctor;
        private Guna2DateTimePicker dtpBook, dtpCal;
        private DataGridView dgvSlots, dgvCal, dgvReserved, dgvEmployees;
        private TabControl tabControl;
        private TabPage tabBook, tabReserved;
    }
}
