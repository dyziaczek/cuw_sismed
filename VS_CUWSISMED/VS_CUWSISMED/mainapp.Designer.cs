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
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── controls ──────────────────────────────────────────────────────
            this.pnlSidebar = new Panel();
            this.lblAppTitle = new Label();
            this.lblAppSub = new Label();
            this.pnlSideSearch = new Panel();
            this.txtSearch = new Guna2TextBox();
            this.btnSearch = new Guna2Button();
            this.pnlPatientCard = new Panel();
            this.lblPatientName = new Label();
            this.lblPatientPesel = new Label();
            this.lblPatientPhone = new Label();
            this.lblPatientWarnings = new Label();
            this.lblPatientStatus = new Label();
            this.btnLogout = new Guna2Button();

            this.pnlMain = new Panel();
            this.tabControl = new TabControl();
            this.tabBook = new TabPage();
            this.tabCalendar = new TabPage();
            this.tabReserved = new TabPage();

            // ── TAB 1: Umówienie wizyty ────────────────────────────────────────
            this.pnlBookTop = new Panel();
            this.lblBookDoctor = new Label();
            this.cmbDoctor = new Guna2ComboBox();
            this.lblBookDate = new Label();
            this.dtpBook = new Guna2DateTimePicker();
            this.btnLoadSlots = new Guna2Button();
            this.dgvSlots = new DataGridView();
            this.btnReserve = new Guna2Button();

            // ── TAB 2: Kalendarz wizyt ─────────────────────────────────────────
            this.pnlCalTop = new Panel();
            this.lblCalDoctor = new Label();
            this.cmbCalDoctor = new Guna2ComboBox();
            this.lblCalDate = new Label();
            this.dtpCal = new Guna2DateTimePicker();
            this.btnLoadCal = new Guna2Button();
            this.dgvCal = new DataGridView();

            // ── TAB 3: Zarezerwowane wizyty ────────────────────────────────────
            this.dgvReserved = new DataGridView();
            this.pnlReservedActions = new Panel();
            this.btnCancel = new Guna2Button();
            this.btnSwap = new Guna2Button();
            this.txtSwapSearch = new Guna2TextBox();
            this.btnSwapFind = new Guna2Button();
            this.lblSwapResult = new Label();

            this.pnlStatusBar = new Panel();
            this.lblStatus = new Label();

            this.SuspendLayout();

            // ────────────────────────────────────────────────────────────────────
            // KOLORY
            // ────────────────────────────────────────────────────────────────────
            Color bg = Color.FromArgb(10, 12, 35);
            Color sidebar = Color.FromArgb(15, 18, 50);
            Color cardBg = Color.FromArgb(20, 24, 65);
            Color accent = Color.FromArgb(220, 0, 150);   // magenta
            Color accentDark = Color.FromArgb(140, 0, 90);
            Color navy = Color.FromArgb(0, 20, 80);
            Color textPri = Color.White;
            Color textSec = Color.FromArgb(160, 170, 220);
            Color border = Color.FromArgb(40, 50, 120);
            // Kolor tekstu w Guna2ComboBox — ciemny, bo kontrolka ma jasne tło
            Color cmbTextClr = Color.FromArgb(15, 18, 50);

            Font fontTitle = new Font("Segoe UI", 13f, FontStyle.Bold);
            Font fontSub = new Font("Segoe UI", 8.5f);
            Font fontLabel = new Font("Segoe UI", 9f, FontStyle.Bold);
            Font fontNormal = new Font("Segoe UI", 9f);

            // ────────────────────────────────────────────────────────────────────
            // FORM
            // ────────────────────────────────────────────────────────────────────
            this.BackColor = bg;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1200, 720);
            this.DoubleBuffered = true;
            this.Text = "CUW SISMED";

            // ────────────────────────────────────────────────────────────────────
            // SIDEBAR  (280 × 720)
            // ────────────────────────────────────────────────────────────────────
            this.pnlSidebar.BackColor = sidebar;
            this.pnlSidebar.Location = new Point(0, 0);
            this.pnlSidebar.Size = new Size(280, 720);

            // Tytuł
            this.lblAppTitle.Text = "CUW · SISMED";
            this.lblAppTitle.Font = fontTitle;
            this.lblAppTitle.ForeColor = accent;
            this.lblAppTitle.Location = new Point(20, 24);
            this.lblAppTitle.Size = new Size(240, 28);

            this.lblAppSub.Text = "Centrum Umawiania Wizyt";
            this.lblAppSub.Font = fontSub;
            this.lblAppSub.ForeColor = textSec;
            this.lblAppSub.Location = new Point(20, 50);
            this.lblAppSub.Size = new Size(240, 18);

            // Wyszukiwarka
            this.pnlSideSearch.BackColor = Color.Transparent;
            this.pnlSideSearch.Location = new Point(12, 90);
            this.pnlSideSearch.Size = new Size(256, 110);

            var lblSearchHint = new Label
            {
                Text = "WYSZUKAJ PACJENTA",
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = textSec,
                Location = new Point(0, 0),
                Size = new Size(256, 16),
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.txtSearch.Location = new Point(0, 20);
            this.txtSearch.Size = new Size(256, 34);
            this.txtSearch.PlaceholderText = "PESEL / Telefon / E-mail";
            this.txtSearch.Font = fontNormal;
            this.txtSearch.ForeColor = textPri;
            this.txtSearch.BackColor = cardBg;
            this.txtSearch.BorderColor = border;
            this.txtSearch.BorderThickness = 2;
            this.txtSearch.FocusedState.BorderColor = accent;
            this.txtSearch.HoverState.BorderColor = accentDark;
            this.txtSearch.DefaultText = "";
            this.txtSearch.SelectedText = "";
            this.txtSearch.PlaceholderForeColor = textSec;

            this.btnSearch.Text = "Szukaj";
            this.btnSearch.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.btnSearch.ForeColor = textPri;
            this.btnSearch.FillColor = accent;
            this.btnSearch.BorderRadius = 8;
            this.btnSearch.Location = new Point(0, 62);
            this.btnSearch.Size = new Size(256, 34);
            this.btnSearch.BorderColor = Color.Transparent;
            this.btnSearch.Click += new EventHandler(this.btnSearch_Click);

            this.pnlSideSearch.Controls.AddRange(new Control[]
            {
                lblSearchHint, this.txtSearch, this.btnSearch
            });

            // Karta pacjenta  (Y = 90 + 110 + 8 = 208)
            this.pnlPatientCard.BackColor = cardBg;
            this.pnlPatientCard.Location = new Point(12, 210);
            this.pnlPatientCard.Size = new Size(256, 180);

            this.lblPatientName.Text = "— Brak wybranego pacjenta —";
            this.lblPatientName.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            this.lblPatientName.ForeColor = textPri;
            this.lblPatientName.Location = new Point(10, 12);
            this.lblPatientName.Size = new Size(236, 40);
            this.lblPatientName.TextAlign = ContentAlignment.TopLeft;

            this.lblPatientPesel.Text = "PESEL: —";
            this.lblPatientPesel.Font = fontNormal;
            this.lblPatientPesel.ForeColor = textSec;
            this.lblPatientPesel.Location = new Point(10, 58);
            this.lblPatientPesel.Size = new Size(236, 18);

            this.lblPatientPhone.Text = "Tel: —";
            this.lblPatientPhone.Font = fontNormal;
            this.lblPatientPhone.ForeColor = textSec;
            this.lblPatientPhone.Location = new Point(10, 80);
            this.lblPatientPhone.Size = new Size(236, 18);

            this.lblPatientWarnings.Text = "Ostrzeżenia: 0/3";
            this.lblPatientWarnings.Font = fontNormal;
            this.lblPatientWarnings.ForeColor = textSec;
            this.lblPatientWarnings.Location = new Point(10, 102);
            this.lblPatientWarnings.Size = new Size(236, 18);

            this.lblPatientStatus.Text = "";
            this.lblPatientStatus.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.lblPatientStatus.ForeColor = Color.OrangeRed;
            this.lblPatientStatus.Location = new Point(10, 126);
            this.lblPatientStatus.Size = new Size(236, 18);

            this.pnlPatientCard.Controls.AddRange(new Control[]
            {
                this.lblPatientName, this.lblPatientPesel, this.lblPatientPhone,
                this.lblPatientWarnings, this.lblPatientStatus
            });
            this.pnlPatientCard.Paint += (s, e) =>
            {
                using (var pen = new System.Drawing.Pen(accent, 2))
                    e.Graphics.DrawRectangle(pen, 0, 0,
                        this.pnlPatientCard.Width - 1, this.pnlPatientCard.Height - 1);
            };

            // ── PRZYCISK DODAJ PACJENTA (Y = 210 + 180 + 10 = 400) ───────────
            var btnAddPatient = new Guna2Button
            {
                Text = "+ Dodaj pacjenta",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = Color.FromArgb(0, 130, 110),
                BorderRadius = 8,
                Location = new Point(12, 400),
                Size = new Size(256, 34),
                BorderColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Name = "btnAddPatient"
            };
            btnAddPatient.Click += new EventHandler(this.btnAddPatient_Click);

            // ── PRZYCISK KALENDARZ WIZYT (Y = 400 + 34 + 8 = 442) ────────────
            var btnOpenCalendar = new Guna2Button
            {
                Text = "📅  Kalendarz wizyt",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = Color.FromArgb(40, 50, 130),
                BorderRadius = 8,
                Location = new Point(12, 442),
                Size = new Size(256, 34),
                BorderColor = Color.FromArgb(220, 0, 150),
                Cursor = Cursors.Hand
            };
            btnOpenCalendar.Click += new EventHandler(this.btnOpenCalendar_Click);

            // Wyloguj
            this.btnLogout.Text = "Wyloguj";
            this.btnLogout.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.btnLogout.ForeColor = accent;
            this.btnLogout.FillColor = Color.Transparent;
            this.btnLogout.BorderColor = accent;
            this.btnLogout.BorderRadius = 8;
            this.btnLogout.Location = new Point(12, 670);
            this.btnLogout.Size = new Size(256, 32);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            this.pnlSidebar.Controls.AddRange(new Control[]
            {
                this.lblAppTitle, this.lblAppSub,
                this.pnlSideSearch, this.pnlPatientCard,
                btnAddPatient, btnOpenCalendar,
                this.btnLogout
            });

            this.pnlSidebar.Paint += (s, e) =>
            {
                using (var pen = new System.Drawing.Pen(accent, 3))
                    e.Graphics.DrawLine(pen, 0, 0, 0, this.pnlSidebar.Height);
                using (var pen = new System.Drawing.Pen(border, 1))
                    e.Graphics.DrawLine(pen, 10, 78, 270, 78);
            };

            // ────────────────────────────────────────────────────────────────────
            // MAIN PANEL + TABCONTROL
            // ────────────────────────────────────────────────────────────────────
            this.pnlMain.BackColor = bg;
            this.pnlMain.Location = new Point(280, 0);
            this.pnlMain.Size = new Size(920, 690);

            this.tabControl.Location = new Point(10, 10);
            this.tabControl.Size = new Size(900, 670);
            this.tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl.ItemSize = new Size(0, 34);
            this.tabControl.SizeMode = TabSizeMode.Fixed;
            this.tabControl.Appearance = TabAppearance.Normal;
            this.tabControl.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            this.tabControl.DrawItem += TabControl_DrawItem;
            this.tabControl.BackColor = bg;
            this.tabControl.Padding = new Point(16, 6);

            this.tabBook.Text = "Umówienie wizyty";
            this.tabBook.BackColor = bg;
            this.tabCalendar.Text = "Kalendarz wizyt";
            this.tabCalendar.BackColor = bg;
            this.tabReserved.Text = "Zarezerwowane wizyty";
            this.tabReserved.BackColor = bg;

            // ════════════════════════════════════════════════════════════════════
            // TAB 1 — UMÓWIENIE WIZYTY
            // ════════════════════════════════════════════════════════════════════
            this.pnlBookTop.BackColor = cardBg;
            this.pnlBookTop.Location = new Point(0, 0);
            this.pnlBookTop.Size = new Size(892, 60);

            this.lblBookDoctor.Text = "Lekarz:";
            this.lblBookDoctor.Font = fontLabel;
            this.lblBookDoctor.ForeColor = textSec;
            this.lblBookDoctor.Location = new Point(12, 20);
            this.lblBookDoctor.Size = new Size(55, 20);

            // ── Guna2ComboBox: ForeColor = CIEMNY (bo tło kontrolki jest jasne) ──
            this.cmbDoctor.Location = new Point(70, 12);
            this.cmbDoctor.Size = new Size(260, 34);
            this.cmbDoctor.Font = fontNormal;
            this.cmbDoctor.ForeColor = cmbTextClr;   // ← ciemny tekst
            this.cmbDoctor.BorderColor = border;
            this.cmbDoctor.BorderThickness = 2;
            this.cmbDoctor.FocusedState.BorderColor = accent;
            this.cmbDoctor.ItemHeight = 28;
            this.cmbDoctor.DropDownHeight = 200;

            this.lblBookDate.Text = "Data:";
            this.lblBookDate.Font = fontLabel;
            this.lblBookDate.ForeColor = textSec;
            this.lblBookDate.Location = new Point(350, 20);
            this.lblBookDate.Size = new Size(40, 20);

            this.dtpBook.Location = new Point(395, 12);
            this.dtpBook.Size = new Size(160, 34);
            this.dtpBook.Font = fontNormal;
            this.dtpBook.ForeColor = textPri;
            this.dtpBook.BackColor = cardBg;
            this.dtpBook.BorderColor = border;
            this.dtpBook.BorderThickness = 2;
            this.dtpBook.CustomFormat = "dd.MM.yyyy";
            this.dtpBook.Format = DateTimePickerFormat.Custom;
            this.dtpBook.MinDate = DateTime.Today;
            this.dtpBook.Value = DateTime.Today;

            this.btnLoadSlots.Text = "Pokaż dostępne terminy";
            this.btnLoadSlots.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.btnLoadSlots.ForeColor = textPri;
            this.btnLoadSlots.FillColor = accent;
            this.btnLoadSlots.BorderRadius = 8;
            this.btnLoadSlots.Location = new Point(570, 12);
            this.btnLoadSlots.Size = new Size(210, 34);
            this.btnLoadSlots.BorderColor = Color.Transparent;
            this.btnLoadSlots.Click += new EventHandler(this.btnLoadSlots_Click);

            this.pnlBookTop.Controls.AddRange(new Control[]
            {
                this.lblBookDoctor, this.cmbDoctor,
                this.lblBookDate, this.dtpBook, this.btnLoadSlots
            });

            // Grid wolnych slotów
            this.dgvSlots.Location = new Point(0, 65);
            this.dgvSlots.Size = new Size(892, 510);
            this.dgvSlots.BackgroundColor = bg;
            this.dgvSlots.BorderStyle = BorderStyle.None;
            this.dgvSlots.GridColor = border;
            this.dgvSlots.Font = fontNormal;
            this.dgvSlots.ReadOnly = true;
            this.dgvSlots.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSlots.MultiSelect = false;
            this.dgvSlots.RowHeadersVisible = false;
            this.dgvSlots.AllowUserToAddRows = false;
            this.dgvSlots.AllowUserToDeleteRows = false;
            this.dgvSlots.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSlots.ColumnHeadersHeight = 36;
            this.dgvSlots.RowTemplate.Height = 30;
            ApplyDgvTheme(this.dgvSlots, bg, cardBg, accent, textPri, textSec, border);
            this.dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Godzina", Width = 100 });
            this.dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDoctor", HeaderText = "Lekarz", Width = 220 });
            this.dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSpec", HeaderText = "Specjalizacja", Width = 200 });
            this.dgvSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate", HeaderText = "Data", Width = 120 });

            this.btnReserve.Text = "Zarezerwuj wizytę";
            this.btnReserve.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            this.btnReserve.ForeColor = Color.White;
            this.btnReserve.FillColor = accentDark;
            this.btnReserve.BorderRadius = 10;
            this.btnReserve.Location = new Point(680, 582);
            this.btnReserve.Size = new Size(210, 38);
            this.btnReserve.BorderColor = Color.Transparent;
            this.btnReserve.Click += new EventHandler(this.btnReserve_Click);

            this.tabBook.Controls.AddRange(new Control[]
            {
                this.pnlBookTop, this.dgvSlots, this.btnReserve
            });

            // ════════════════════════════════════════════════════════════════════
            // TAB 2 — KALENDARZ WIZYT
            // ════════════════════════════════════════════════════════════════════
            this.pnlCalTop.BackColor = cardBg;
            this.pnlCalTop.Location = new Point(0, 0);
            this.pnlCalTop.Size = new Size(892, 60);

            this.lblCalDoctor.Text = "Lekarz:";
            this.lblCalDoctor.Font = fontLabel;
            this.lblCalDoctor.ForeColor = textSec;
            this.lblCalDoctor.Location = new Point(12, 20);
            this.lblCalDoctor.Size = new Size(55, 20);

            // ── Guna2ComboBox: ForeColor = CIEMNY ──
            this.cmbCalDoctor.Location = new Point(70, 12);
            this.cmbCalDoctor.Size = new Size(260, 34);
            this.cmbCalDoctor.Font = fontNormal;
            this.cmbCalDoctor.ForeColor = cmbTextClr;   // ← ciemny tekst
            this.cmbCalDoctor.BorderColor = border;
            this.cmbCalDoctor.BorderThickness = 2;
            this.cmbCalDoctor.FocusedState.BorderColor = accent;
            this.cmbCalDoctor.ItemHeight = 28;
            this.cmbCalDoctor.DropDownHeight = 200;

            this.lblCalDate.Text = "Data:";
            this.lblCalDate.Font = fontLabel;
            this.lblCalDate.ForeColor = textSec;
            this.lblCalDate.Location = new Point(350, 20);
            this.lblCalDate.Size = new Size(40, 20);

            this.dtpCal.Location = new Point(395, 12);
            this.dtpCal.Size = new Size(160, 34);
            this.dtpCal.Font = fontNormal;
            this.dtpCal.ForeColor = textPri;
            this.dtpCal.BackColor = cardBg;
            this.dtpCal.BorderColor = border;
            this.dtpCal.BorderThickness = 2;
            this.dtpCal.CustomFormat = "dd.MM.yyyy";
            this.dtpCal.Format = DateTimePickerFormat.Custom;
            this.dtpCal.Value = DateTime.Today;

            this.btnLoadCal.Text = "Pokaż grafik";
            this.btnLoadCal.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.btnLoadCal.ForeColor = textPri;
            this.btnLoadCal.FillColor = accent;
            this.btnLoadCal.BorderRadius = 8;
            this.btnLoadCal.Location = new Point(570, 12);
            this.btnLoadCal.Size = new Size(160, 34);
            this.btnLoadCal.BorderColor = Color.Transparent;
            this.btnLoadCal.Click += new EventHandler(this.btnLoadCal_Click);

            this.pnlCalTop.Controls.AddRange(new Control[]
            {
                this.lblCalDoctor, this.cmbCalDoctor,
                this.lblCalDate, this.dtpCal, this.btnLoadCal
            });

            this.dgvCal.Location = new Point(0, 65);
            this.dgvCal.Size = new Size(892, 560);
            this.dgvCal.BackgroundColor = bg;
            this.dgvCal.BorderStyle = BorderStyle.None;
            this.dgvCal.GridColor = border;
            this.dgvCal.Font = fontNormal;
            this.dgvCal.ReadOnly = true;
            this.dgvCal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvCal.MultiSelect = false;
            this.dgvCal.RowHeadersVisible = false;
            this.dgvCal.AllowUserToAddRows = false;
            this.dgvCal.AllowUserToDeleteRows = false;
            this.dgvCal.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvCal.ColumnHeadersHeight = 36;
            this.dgvCal.RowTemplate.Height = 30;
            ApplyDgvTheme(this.dgvCal, bg, cardBg, accent, textPri, textSec, border);
            this.dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calTime", HeaderText = "Godzina", Width = 100 });
            this.dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calPatient", HeaderText = "Pacjent", Width = 300 });
            this.dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calStatus", HeaderText = "Status", Width = 160 });
            this.dgvCal.CellFormatting += DgvCal_CellFormatting;

            this.tabCalendar.Controls.AddRange(new Control[] { this.pnlCalTop, this.dgvCal });

            // ════════════════════════════════════════════════════════════════════
            // TAB 3 — ZAREZERWOWANE WIZYTY
            // ════════════════════════════════════════════════════════════════════
            this.dgvReserved.Location = new Point(0, 0);
            this.dgvReserved.Size = new Size(892, 440);
            this.dgvReserved.BackgroundColor = bg;
            this.dgvReserved.BorderStyle = BorderStyle.None;
            this.dgvReserved.GridColor = border;
            this.dgvReserved.Font = fontNormal;
            this.dgvReserved.ReadOnly = true;
            this.dgvReserved.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvReserved.MultiSelect = false;
            this.dgvReserved.RowHeadersVisible = false;
            this.dgvReserved.AllowUserToAddRows = false;
            this.dgvReserved.AllowUserToDeleteRows = false;
            this.dgvReserved.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvReserved.ColumnHeadersHeight = 36;
            this.dgvReserved.RowTemplate.Height = 30;
            ApplyDgvTheme(this.dgvReserved, bg, cardBg, accent, textPri, textSec, border);
            this.dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rDate", HeaderText = "Data", Width = 100 });
            this.dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rTime", HeaderText = "Godzina", Width = 90 });
            this.dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rDoctor", HeaderText = "Lekarz", Width = 220 });
            this.dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rSpec", HeaderText = "Spec.", Width = 160 });
            this.dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rStatus", HeaderText = "Status", Width = 130 });
            this.dgvReserved.Columns.Add(new DataGridViewTextBoxColumn { Name = "rNote", HeaderText = "Uwagi", Width = 180 });

            // Panel akcji
            this.pnlReservedActions.BackColor = cardBg;
            this.pnlReservedActions.Location = new Point(0, 445);
            this.pnlReservedActions.Size = new Size(892, 185);

            var lblActionsTitle = new Label
            {
                Text = "AKCJE",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = textSec,
                Location = new Point(12, 8),
                Size = new Size(200, 16),
                BackColor = Color.Transparent
            };

            this.btnCancel.Text = "Anuluj wizytę";
            this.btnCancel.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.FillColor = Color.FromArgb(180, 30, 30);
            this.btnCancel.BorderRadius = 8;
            this.btnCancel.Location = new Point(12, 32);
            this.btnCancel.Size = new Size(160, 34);
            this.btnCancel.BorderColor = Color.Transparent;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            var lblSwapTitle = new Label
            {
                Text = "ZAMIANA WIZYTY — wyszukaj nowego pacjenta:",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = textSec,
                Location = new Point(12, 82),
                Size = new Size(400, 16),
                BackColor = Color.Transparent
            };

            this.txtSwapSearch.Location = new Point(12, 104);
            this.txtSwapSearch.Size = new Size(260, 34);
            this.txtSwapSearch.PlaceholderText = "PESEL / Telefon";
            this.txtSwapSearch.Font = fontNormal;
            this.txtSwapSearch.ForeColor = textPri;
            this.txtSwapSearch.BackColor = bg;
            this.txtSwapSearch.BorderColor = border;
            this.txtSwapSearch.BorderThickness = 2;
            this.txtSwapSearch.FocusedState.BorderColor = accent;
            this.txtSwapSearch.DefaultText = "";
            this.txtSwapSearch.SelectedText = "";
            this.txtSwapSearch.PlaceholderForeColor = textSec;

            this.btnSwapFind.Text = "Znajdź";
            this.btnSwapFind.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.btnSwapFind.ForeColor = textPri;
            this.btnSwapFind.FillColor = Color.FromArgb(40, 60, 130);
            this.btnSwapFind.BorderRadius = 8;
            this.btnSwapFind.Location = new Point(280, 104);
            this.btnSwapFind.Size = new Size(100, 34);
            this.btnSwapFind.BorderColor = Color.Transparent;
            this.btnSwapFind.Click += new EventHandler(this.btnSwapFind_Click);

            this.lblSwapResult.Text = "";
            this.lblSwapResult.Font = fontNormal;
            this.lblSwapResult.ForeColor = Color.LightGreen;
            this.lblSwapResult.Location = new Point(390, 110);
            this.lblSwapResult.Size = new Size(300, 22);
            this.lblSwapResult.BackColor = Color.Transparent;

            this.btnSwap.Text = "Zatwierdź zamianę";
            this.btnSwap.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            this.btnSwap.ForeColor = textPri;
            this.btnSwap.FillColor = accent;
            this.btnSwap.BorderRadius = 8;
            this.btnSwap.Location = new Point(700, 104);
            this.btnSwap.Size = new Size(180, 34);
            this.btnSwap.BorderColor = Color.Transparent;
            this.btnSwap.Enabled = false;
            this.btnSwap.Click += new EventHandler(this.btnSwap_Click);

            this.pnlReservedActions.Controls.AddRange(new Control[]
            {
                lblActionsTitle, this.btnCancel,
                lblSwapTitle, this.txtSwapSearch, this.btnSwapFind,
                this.lblSwapResult, this.btnSwap
            });

            this.tabReserved.Controls.AddRange(new Control[]
            {
                this.dgvReserved, this.pnlReservedActions
            });

            // Złóż tabControl
            this.tabControl.TabPages.AddRange(new TabPage[]
            {
                this.tabBook, this.tabCalendar, this.tabReserved
            });
            this.pnlMain.Controls.Add(this.tabControl);

            // ── Status bar ────────────────────────────────────────────────────
            this.pnlStatusBar.BackColor = navy;
            this.pnlStatusBar.Location = new Point(280, 690);
            this.pnlStatusBar.Size = new Size(920, 30);

            this.lblStatus.Text = "Gotowy";
            this.lblStatus.Font = fontSub;
            this.lblStatus.ForeColor = textSec;
            this.lblStatus.Location = new Point(10, 7);
            this.lblStatus.Size = new Size(900, 18);
            this.lblStatus.BackColor = Color.Transparent;
            this.pnlStatusBar.Controls.Add(this.lblStatus);

            // Drag to move
            this.pnlSidebar.MouseDown += MoveForm;
            this.lblAppTitle.MouseDown += MoveForm;
            this.lblAppSub.MouseDown += MoveForm;

            // Przycisk zamknięcia
            var btnClose = new Guna2Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.White,
                FillColor = Color.Transparent,
                BorderColor = Color.Transparent,
                BorderRadius = 6,
                Location = new Point(1158, 4),
                Size = new Size(36, 28)
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                this.pnlSidebar, this.pnlMain, this.pnlStatusBar, btnClose
            });
            btnClose.BringToFront();

            this.ResumeLayout(false);
        }

        // ── Helper: ciemny motyw DataGridView ────────────────────────────────
        private static void ApplyDgvTheme(DataGridView dgv, Color bg, Color cellBg,
            Color accent, Color textPri, Color textSec, Color border)
        {
            dgv.BackgroundColor = bg;
            dgv.DefaultCellStyle.BackColor = cellBg;
            dgv.DefaultCellStyle.ForeColor = textPri;
            dgv.DefaultCellStyle.SelectionBackColor = accent;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(
                Math.Min(cellBg.R + 8, 255), Math.Min(cellBg.G + 8, 255), Math.Min(cellBg.B + 18, 255));
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = textPri;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = bg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = accent;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = bg;
            dgv.EnableHeadersVisualStyles = false;
        }

        // ── Helper: rysowanie zakładek ────────────────────────────────────────
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            Color bg = Color.FromArgb(10, 12, 35);
            Color accent = Color.FromArgb(220, 0, 150);
            Color sel = Color.FromArgb(20, 24, 65);
            Color textSec = Color.FromArgb(160, 170, 220);

            var tab = this.tabControl.TabPages[e.Index];
            var rect = this.tabControl.GetTabRect(e.Index);
            bool isSelected = (e.Index == this.tabControl.SelectedIndex);

            using (var br = new SolidBrush(isSelected ? sel : bg))
                e.Graphics.FillRectangle(br, rect);

            if (isSelected)
                using (var pen = new System.Drawing.Pen(accent, 3))
                    e.Graphics.DrawLine(pen, rect.Left, rect.Bottom - 3, rect.Right, rect.Bottom - 3);

            using (var br = new SolidBrush(isSelected ? Color.White : textSec))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
                e.Graphics.DrawString(tab.Text,
                    new Font("Segoe UI", 9f, isSelected ? FontStyle.Bold : FontStyle.Regular),
                    br, rect, sf);
        }

        // ── Color coding dla statusu w tabeli kalendarza ──────────────────────
        private void DgvCal_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = this.dgvCal.Rows[e.RowIndex];
            string st = row.Cells["calStatus"].Value?.ToString() ?? "";
            if (st == "Wolny")
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(80, 220, 120);
                row.DefaultCellStyle.BackColor = Color.FromArgb(18, 40, 28);
            }
            else if (st == "Zajęty")
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(255, 120, 120);
                row.DefaultCellStyle.BackColor = Color.FromArgb(40, 16, 20);
            }
        }

        #endregion

        // controls
        private Panel pnlSidebar, pnlSideSearch, pnlPatientCard, pnlMain;
        private Panel pnlBookTop, pnlCalTop, pnlReservedActions, pnlStatusBar;
        private Label lblAppTitle, lblAppSub;
        private Label lblPatientName, lblPatientPesel, lblPatientPhone;
        private Label lblPatientWarnings, lblPatientStatus;
        private Label lblBookDoctor, lblBookDate, lblCalDoctor, lblCalDate;
        private Label lblSwapResult, lblStatus;
        private Guna2TextBox txtSearch, txtSwapSearch;
        private Guna2Button btnSearch, btnLogout, btnLoadSlots, btnReserve;
        private Guna2Button btnLoadCal, btnCancel, btnSwap, btnSwapFind;
        private Guna2ComboBox cmbDoctor, cmbCalDoctor;
        private Guna2DateTimePicker dtpBook, dtpCal;
        private DataGridView dgvSlots, dgvCal, dgvReserved;
        private TabControl tabControl;
        private TabPage tabBook, tabCalendar, tabReserved;
    }
}
