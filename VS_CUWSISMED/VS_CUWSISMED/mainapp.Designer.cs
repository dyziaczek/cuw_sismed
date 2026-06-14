using System;
using System.Drawing;
using System.Windows.Forms;

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

            Color navy = SismedTheme.Navy;
            Color blue = SismedTheme.Blue;
            Color magenta = SismedTheme.Magenta;
            Color surface = SismedTheme.Surface;
            Color card = SismedTheme.Card;
            Color border = SismedTheme.Border;
            Color text = SismedTheme.Text;
            Color muted = SismedTheme.Muted;
            Color navInactive = SismedTheme.NavyDark;

            Font navFont = SismedTheme.Font(10f, FontStyle.Bold);
            Font labelFont = SismedTheme.Font(9f, FontStyle.Bold);
            Font normalFont = SismedTheme.Font(9f);

            SuspendLayout();

            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = surface;
            ClientSize = new Size(1280, 760);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1024, 640);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CUW SISMED";
            WindowState = FormWindowState.Maximized;

            pnlNavigation = new Panel
            {
                Dock = DockStyle.Left,
                Width = SismedTheme.SidebarWidth,
                BackColor = navy
            };

            picLogo = new PictureBox
            {
                Image = Properties.Resources.LOGO,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(12, 8),
                Size = new Size(252, 168),
                BackColor = Color.Transparent
            };

            lblNavTitle = new Label
            {
                Text = "Centrum Medyczne",
                Font = SismedTheme.Font(8.5f, FontStyle.Bold),
                ForeColor = SismedTheme.SidebarMuted,
                Location = new Point(22, 178),
                Size = new Size(232, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblNavSection = new Label
            {
                Text = "MENU",
                Font = SismedTheme.Font(8.5f, FontStyle.Bold),
                ForeColor = SismedTheme.SidebarMuted,
                Location = new Point(28, 220),
                Size = new Size(210, 18)
            };

            btnNavSearch = CreateNavButton("SZUKAJ", 250, navFont, magenta);
            btnNavReception = CreateNavButton("RECEPCJA", 304, navFont, navInactive);
            btnNavCalendar = CreateNavButton("KALENDARZ WIZYT", 358, navFont, navInactive);
            btnNavDocuments = CreateNavButton("DOKUMENTY", 412, navFont, navInactive);
            btnNavPersonnel = CreateNavButton("PERSONEL", 466, navFont, navInactive);
            btnNavPatients = CreateNavButton("PACJENCI", 520, navFont, navInactive);
            btnNavSearch.Click += btnNavSearch_Click;
            btnNavCalendar.Click += btnNavCalendar_Click;
            btnNavReception.Click += btnNavReception_Click;
            btnNavDocuments.Click += btnNavDocuments_Click;
            btnNavPersonnel.Click += btnNavPersonnel_Click;
            btnNavPatients.Click += btnNavPatients_Click;

            btnLogout = new Button
            {
                Text = "Wyloguj",
                Font = navFont,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(20, 690),
                Size = new Size(236, 38),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            SismedTheme.ApplyOutlineButton(btnLogout);
            btnLogout.BackColor = Color.Transparent;
            btnLogout.FlatAppearance.BorderColor = magenta;
            btnLogout.Click += btnLogout_Click;

            pnlNavigation.Controls.AddRange(new Control[]
            {
                picLogo, lblNavTitle, lblNavSection,
                btnNavSearch, btnNavReception, btnNavCalendar, btnNavDocuments, btnNavPersonnel, btnNavPatients,
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
                Height = 72,
                BackColor = Color.White
            };

            lblScreenTitle = new Label
            {
                Text = "RECEPCJA",
                Font = SismedTheme.Font(17f, FontStyle.Bold),
                ForeColor = navy,
                Location = new Point(24, 16),
                Size = new Size(420, 32)
            };

            lblCurrentUser = new Label
            {
                Text = "",
                Font = SismedTheme.Font(9f, FontStyle.Bold),
                ForeColor = muted,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(600, 19),
                Size = new Size(360, 28),
                TextAlign = ContentAlignment.MiddleRight
            };

            btnClose = new Button
            {
                Text = "X",
                Font = SismedTheme.Font(10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = magenta,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(970, 16),
                Size = new Size(36, 32)
            };
            SismedTheme.ApplyPrimaryButton(btnClose);
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
                AutoScroll = true,
                Padding = new Padding(SismedTheme.Padding)
            };

            BuildSearchScreen(card, border, text, muted, magenta, blue, labelFont, normalFont);
            BuildReceptionScreen(card, border, text, muted, magenta, blue, labelFont, normalFont);
            BuildCalendarScreen(card, border, text, muted, magenta, blue, labelFont, normalFont);
            BuildDocumentsScreen(card, border, text, muted, magenta, normalFont);
            BuildPersonnelScreen(card, border, text, muted, magenta, blue, labelFont, normalFont);
            BuildPatientsScreen(card, text, muted, magenta);

            pnlScreenHost.Controls.AddRange(new Control[]
            {
                pnlPatientsScreen,
                pnlDocumentsScreen,
                pnlPersonnelScreen,
                pnlCalendarScreen,
                pnlReceptionScreen,
                pnlSearchScreen
            });

            pnlShell.Controls.Add(pnlScreenHost);
            pnlShell.Controls.Add(pnlTopBar);

            Controls.Add(pnlShell);
            Controls.Add(pnlNavigation);

            ResumeLayout(false);
        }

        private Button CreateNavButton(string text, int top, Font font, Color fill)
        {
            var button = new Button
            {
                Text = text,
                Font = font,
                ForeColor = Color.White,
                BackColor = fill,
                Location = new Point(20, top),
                Size = new Size(SismedTheme.SidebarWidth - 40, 44),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Image = null
            };
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
            return button;
        }

        private void BuildSearchScreen(
            Color card,
            Color border,
            Color text,
            Color muted,
            Color magenta,
            Color blue,
            Font labelFont,
            Font normalFont)
        {
            pnlSearchScreen = CreateScreenPanel();

            var searchRoot = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = SismedTheme.Surface
            };
            searchRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 292F));
            searchRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            pnlSearchTop = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(24)
            };

            var searchAccent = new Panel
            {
                BackColor = magenta,
                Location = new Point(24, 26),
                Size = new Size(5, 48)
            };

            var lblSearchTitle = new Label
            {
                Text = "Wyszukiwanie pacjenta",
                Font = SismedTheme.Font(19f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy,
                Location = new Point(42, 22),
                Size = new Size(420, 34)
            };

            var lblSearchSub = new Label
            {
                Text = "Wpisz PESEL albo dane kontaktowe pacjenta. Jeśli jest kilka wyników, wybierz pacjenta z listy.",
                Font = normalFont,
                ForeColor = muted,
                Location = new Point(42, 58),
                Size = new Size(820, 24)
            };

            txtPatientPesel = CreateTextBox("PESEL", 42, 108, 220);
            txtPatientFirstName = CreateTextBox("Imię", 282, 108, 220);
            txtPatientLastName = CreateTextBox("Nazwisko", 522, 108, 220);
            txtPatientBirthDate = CreateTextBox("Data urodzenia dd-MM-yyyy", 762, 108, 220);
            txtPatientEmail = CreateTextBox("Adres e-mail", 42, 166, 340);
            txtPatientPhone = CreateTextBox("Numer telefonu", 402, 166, 220);
            btnSearch = CreateActionButton("Szukaj", 42, 224, 130, magenta);
            btnSearch.Click += btnSearch_Click;
            btnClearPatientSearch = CreateActionButton("Wyczyść", 188, 224, 130, blue);
            btnClearPatientSearch.Click += btnClearPatientSearch_Click;
            btnAddPatient = CreateActionButton("+ Dodaj pacjenta", 334, 224, 180, SismedTheme.Success);
            btnAddPatient.Click += btnAddPatient_Click;

            pnlSearchTop.Controls.AddRange(new Control[]
            {
                searchAccent,
                lblSearchTitle,
                lblSearchSub,
                txtPatientPesel,
                txtPatientFirstName,
                txtPatientLastName,
                txtPatientBirthDate,
                txtPatientEmail,
                txtPatientPhone,
                btnSearch,
                btnClearPatientSearch,
                btnAddPatient
            });

            pnlSearchResults = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Padding = new Padding(20)
            };

            lblSearchResultsTitle = new Label
            {
                Text = "Wyniki wyszukiwania",
                Dock = DockStyle.Top,
                Height = 36,
                Font = SismedTheme.Font(14f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };

            lblSearchResultsInfo = new Label
            {
                Text = "Wyniki pojawią się tutaj, jeśli wyszukiwanie zwróci więcej niż jednego pacjenta.",
                Dock = DockStyle.Top,
                Height = 28,
                Font = normalFont,
                ForeColor = muted
            };

            dgvSearchResults = CreateGrid();
            dgvSearchResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "searchFirst", HeaderText = "Imię", Width = 120 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "searchLast", HeaderText = "Nazwisko", Width = 140 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "searchPesel", HeaderText = "PESEL", Width = 120 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "searchBirth", HeaderText = "Data ur.", Width = 110 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "searchPhone", HeaderText = "Telefon", Width = 110 });
            dgvSearchResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "searchEmail", HeaderText = "E-mail", Width = 180 });
            dgvSearchResults.CellClick += dgvSearchResults_CellClick;

            pnlSearchResults.Controls.Add(dgvSearchResults);
            pnlSearchResults.Controls.Add(lblSearchResultsInfo);
            pnlSearchResults.Controls.Add(lblSearchResultsTitle);

            searchRoot.Controls.Add(pnlSearchTop, 0, 0);
            searchRoot.Controls.Add(pnlSearchResults, 0, 1);
            pnlSearchScreen.Controls.Add(searchRoot);
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

            pnlReceptionContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.Surface,
                Padding = new Padding(0)
            };

            var pnlReceptionHero = new Panel
            {
                Dock = DockStyle.Top,
                Height = 92,
                BackColor = SismedTheme.Card,
                Padding = new Padding(24),
                Margin = new Padding(0, 0, 0, 12)
            };
            var pnlHeroAccent = new Panel
            {
                BackColor = SismedTheme.Magenta,
                Dock = DockStyle.Left,
                Width = 5
            };
            var lblReceptionHeading = new Label
            {
                Text = "Recepcja",
                Font = SismedTheme.Font(19f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy,
                Location = new Point(24, 18),
                Size = new Size(320, 34)
            };
            var lblReceptionSubtitle = new Label
            {
                Text = "Szybka identyfikacja pacjenta, podgląd wizyt i podstawowe działania rejestracji.",
                Font = SismedTheme.Font(10f),
                ForeColor = SismedTheme.Muted,
                Location = new Point(24, 54),
                Size = new Size(760, 24)
            };
            pnlReceptionHero.Controls.Add(lblReceptionSubtitle);
            pnlReceptionHero.Controls.Add(lblReceptionHeading);
            pnlReceptionHero.Controls.Add(pnlHeroAccent);

            pnlDashboardCards = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 122,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = SismedTheme.Surface,
                Padding = new Padding(0, 14, 0, 14)
            };
            pnlDashboardCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            pnlDashboardCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            lblTodayVisitsValue = new Label();
            lblPlannedVisitsValue = new Label();
            pnlDashboardCards.Controls.Add(CreateMetricCard("Dzisiejsze wizyty", lblTodayVisitsValue, SismedTheme.Magenta), 0, 0);
            pnlDashboardCards.Controls.Add(CreateMetricCard("Zarezerwowane wizyty", lblPlannedVisitsValue, SismedTheme.Blue), 1, 0);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = SismedTheme.Font(10f, FontStyle.Bold),
                Padding = new Point(16, 6)
            };

            BuildPatientWorkspaceTab(text, muted, magenta, blue, labelFont, normalFont);

            tabBook = new TabPage { Text = "Umowienie wizyty", BackColor = Color.White, Padding = new Padding(16) };
            tabReserved = new TabPage { Text = "Zarezerwowane wizyty", BackColor = Color.White, Padding = new Padding(16) };

            pnlBookTop = new Panel { Dock = DockStyle.Top, Height = 104, BackColor = Color.White };
            lblBookDoctor = CreateInlineLabel("Lekarz:", 0, 16, muted, labelFont);
            cmbDoctor = CreateComboBox(66, 10, 270);
            lblBookDate = CreateInlineLabel("Data:", 0, 62, muted, labelFont);
            dtpBook = CreateDatePicker(66, 56);
            dtpBook.MinDate = DateTime.Today;
            btnLoadSlots = CreateActionButton("Pokaż dostępne terminy", 232, 56, 220, magenta);
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

            pnlReservedActions = new Panel { Dock = DockStyle.Bottom, Height = 132, BackColor = SismedTheme.CardSoft };
            btnCancel = CreateActionButton("Anuluj wizytę", 16, 16, 150, SismedTheme.Danger);
            btnCancel.Click += btnCancel_Click;
            btnSwap = CreateActionButton("Zatwierdź zamianę", 184, 16, 180, magenta);
            btnSwap.Enabled = false;
            btnSwap.Click += btnSwap_Click;
            txtSwapSearch = CreateTextBox("PESEL / Telefon", 16, 72, 250);
            btnSwapFind = CreateActionButton("Znajdź", 276, 72, 92, blue);
            btnSwapFind.Click += btnSwapFind_Click;
            lblSwapResult = new Label
            {
                Text = "",
                Location = new Point(382, 78),
                Size = new Size(260, 24),
                Font = normalFont,
                ForeColor = SismedTheme.Success
            };
            pnlReservedActions.Controls.AddRange(new Control[]
            {
                btnCancel, txtSwapSearch, btnSwapFind, lblSwapResult, btnSwap
            });

            tabReserved.Controls.Add(dgvReserved);
            tabReserved.Controls.Add(pnlReservedActions);

            tabControl.TabPages.Add(tabPatient);
            tabControl.TabPages.Add(tabBook);
            tabControl.TabPages.Add(tabReserved);

            pnlReceptionContent.Controls.Add(tabControl);
            pnlReceptionContent.Controls.Add(pnlDashboardCards);
            pnlReceptionContent.Controls.Add(pnlReceptionHero);
            pnlReceptionScreen.Controls.Add(pnlReceptionContent);
        }

        private void BuildPatientWorkspaceTab(
            Color text,
            Color muted,
            Color magenta,
            Color blue,
            Font labelFont,
            Font normalFont)
        {
            tabPatient = new TabPage { Text = "Panel pacjenta", BackColor = Color.White, Padding = new Padding(16) };

            patientLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ColumnCount = 2,
                RowCount = 3
            };
            patientLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            patientLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            patientLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 292F));
            patientLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            patientLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            pnlPatientDetailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.CardSoft,
                Margin = new Padding(0, 0, 8, 10),
                Padding = new Padding(18),
                AutoScroll = true
            };

            lblPatientPanelTitle = new Label
            {
                Text = "Karta pacjenta",
                Font = SismedTheme.Font(10f, FontStyle.Bold),
                ForeColor = muted,
                Location = new Point(18, 14),
                Size = new Size(294, 22)
            };
            lblPatientPanelName = CreatePatientDetailLabel("- Brak wybranego pacjenta -", 18, 42, text, true, 36);
            lblPatientPanelPesel = CreatePatientDetailLabel("PESEL: -", 18, 84, muted, false, 22);
            lblPatientPanelBirthDate = CreatePatientDetailLabel("Data ur.: -", 18, 108, muted, false, 22);
            lblPatientPanelPhone = CreatePatientDetailLabel("Tel: -", 18, 132, muted, false, 22);
            lblPatientPanelEmail = CreatePatientDetailLabel("E-mail: -", 18, 156, muted, false, 22);
            lblPatientPanelWarnings = CreatePatientDetailLabel("Ostrzezenia: 0", 18, 184, muted, false, 22);
            lblPatientPanelBlock = CreatePatientDetailLabel("Blokada rezerwacji: nie", 18, 210, SismedTheme.Success, true, 26);

            btnPatientEditData = CreateActionButton("Edytuj dane", 0, 0, 130, SismedTheme.Success);
            btnPatientEditData.Click += btnPatientEditData_Click;
            btnPatientMessages = CreateActionButton("Wiadomości", 0, 0, 130, magenta);
            btnPatientMessages.Click += btnPatientMessages_Click;
            btnPatientBook = CreateActionButton("Umów wizytę", 0, 0, 130, blue);
            btnPatientBook.Click += btnPatientBook_Click;
            btnPatientPlanned = CreateActionButton("Zarezerwowane wizyty", 0, 0, 178, blue);
            btnPatientPlanned.Click += btnPatientPlanned_Click;
            btnPatientHistory = CreateActionButton("Historia wizyt", 0, 0, 140, blue);
            btnPatientHistory.Click += btnPatientHistory_Click;

            pnlPatientDetailsPanel.Controls.AddRange(new Control[]
            {
                lblPatientPanelTitle, lblPatientPanelName, lblPatientPanelPesel, lblPatientPanelBirthDate,
                lblPatientPanelPhone, lblPatientPanelEmail, lblPatientPanelWarnings,
                lblPatientPanelBlock
            });

            pnlPatientAddressPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.CardSoft,
                Margin = new Padding(8, 0, 0, 10),
                Padding = new Padding(18),
                AutoScroll = true
            };

            lblPatientAddressTitle = new Label
            {
                Text = "Adres pacjenta",
                Font = SismedTheme.Font(10f, FontStyle.Bold),
                ForeColor = muted,
                Location = new Point(18, 14),
                Size = new Size(294, 22)
            };

            Label lblAddressCity = CreateAddressCaption("Miasto", 18, 42, 180);
            Label lblAddressPostalCode = CreateAddressCaption("Kod pocztowy", 214, 42, 120);
            Label lblAddressStreet = CreateAddressCaption("Ulica", 18, 102, 316);
            Label lblAddressHouseNumber = CreateAddressCaption("Numer domu", 18, 162, 150);
            Label lblAddressApartmentNumber = CreateAddressCaption("Numer lokalu", 184, 162, 150);
            txtPatientCity = CreateTextBox("", 18, 60, 180);
            txtPatientPostalCode = CreateTextBox("", 214, 60, 120);
            txtPatientStreet = CreateTextBox("", 18, 120, 316);
            txtPatientHouseNumber = CreateTextBox("", 18, 180, 150);
            txtPatientApartmentNumber = CreateTextBox("", 184, 180, 150);
            SetAddressFieldsReadOnly(true);

            lblPatientAddressHint = new Label
            {
                Text = "Adres jest wymagany przed umówieniem wizyty.",
                Font = SismedTheme.Font(9f, FontStyle.Bold),
                ForeColor = SismedTheme.Warning,
                Location = new Point(18, 224),
                Size = new Size(360, 34)
            };

            pnlPatientAddressPanel.Controls.AddRange(new Control[]
            {
                lblPatientAddressTitle,
                lblAddressCity,
                lblAddressPostalCode,
                lblAddressStreet,
                lblAddressHouseNumber,
                lblAddressApartmentNumber,
                txtPatientCity,
                txtPatientPostalCode,
                txtPatientStreet,
                txtPatientHouseNumber,
                txtPatientApartmentNumber,
                lblPatientAddressHint
            });

            pnlPatientActionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.White,
                Padding = new Padding(0, 8, 0, 8)
            };
            pnlPatientActionsPanel.Controls.AddRange(new Control[]
            {
                btnPatientEditData,
                btnPatientBook,
                btnPatientPlanned,
                btnPatientHistory,
                btnPatientMessages
            });
            foreach (Control action in pnlPatientActionsPanel.Controls)
            {
                action.Margin = new Padding(0, 0, 12, 8);
            }

            pnlPatientActionHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.Card,
                Padding = new Padding(18)
            };

            lblPatientActionTitle = new Label
            {
                Text = "Panel pacjenta",
                Dock = DockStyle.Top,
                Height = 36,
                Font = SismedTheme.Font(14f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };

            pnlPatientActionBody = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.Card
            };

            pnlPatientEmptyPanel = CreatePatientActionPanel();
            lblPatientEmptyInfo = new Label
            {
                Text = "Wyszukaj pacjenta, aby zobaczyć kartę, notatki i wizyty.",
                Dock = DockStyle.Fill,
                Font = SismedTheme.Font(11f, FontStyle.Bold),
                ForeColor = muted,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlPatientEmptyPanel.Controls.Add(lblPatientEmptyInfo);

            pnlPatientResultsPanel = CreatePatientActionPanel();
            dgvPatientResults = CreateGrid();
            dgvPatientResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPatientResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "resFirst", HeaderText = "Imię", Width = 120 });
            dgvPatientResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "resLast", HeaderText = "Nazwisko", Width = 140 });
            dgvPatientResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "resPesel", HeaderText = "PESEL", Width = 120 });
            dgvPatientResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "resBirth", HeaderText = "Data ur.", Width = 110 });
            dgvPatientResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "resPhone", HeaderText = "Telefon", Width = 110 });
            dgvPatientResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "resEmail", HeaderText = "E-mail", Width = 180 });
            dgvPatientResults.CellClick += dgvPatientResults_CellClick;
            pnlPatientResultsPanel.Controls.Add(dgvPatientResults);

            pnlPatientNotesPanel = CreatePatientActionPanel();
            dgvPatientNotes = CreateGrid();
            dgvPatientNotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPatientNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "noteCreated", HeaderText = "Utworzono", Width = 145 });
            dgvPatientNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "noteEmployee", HeaderText = "Pracownik", Width = 170 });
            dgvPatientNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "noteText", HeaderText = "Treść", Width = 360 });

            var pnlPatientNoteEditor = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 112,
                BackColor = SismedTheme.CardSoft,
                Padding = new Padding(12)
            };
            txtPatientNote = new TextBox
            {
                Multiline = true,
                Location = new Point(12, 12),
                Size = new Size(360, 82),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Font = normalFont,
                ForeColor = text,
                BorderStyle = BorderStyle.FixedSingle
            };
            btnAddPatientNote = CreateActionButton("Dodaj notatkę", 392, 14, 150, magenta);
            btnAddPatientNote.Click += btnAddPatientNote_Click;
            btnDeletePatientNote = CreateActionButton("Usuń notatkę", 392, 58, 150, SismedTheme.Danger);
            btnDeletePatientNote.Click += btnDeletePatientNote_Click;
            pnlPatientNoteEditor.Resize += (sender, args) =>
            {
                int buttonsLeft = Math.Max(392, pnlPatientNoteEditor.Width - 174);
                btnAddPatientNote.Left = buttonsLeft;
                btnDeletePatientNote.Left = buttonsLeft;
                txtPatientNote.Width = Math.Max(240, buttonsLeft - 28);
            };
            pnlPatientNoteEditor.Controls.AddRange(new Control[] { txtPatientNote, btnAddPatientNote, btnDeletePatientNote });
            pnlPatientNotesPanel.Controls.Add(dgvPatientNotes);
            pnlPatientNotesPanel.Controls.Add(pnlPatientNoteEditor);

            pnlPatientPlannedPanel = CreatePatientActionPanel();
            dgvPatientPlanned = CreateAppointmentSummaryGrid();
            dgvPatientPlanned.SelectionChanged += dgvPatientPlanned_SelectionChanged;
            pnlPatientPlannedDetails = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 162,
                BackColor = SismedTheme.CardSoft,
                Padding = new Padding(14)
            };
            lblPlannedAppointmentDetails = new Label
            {
                Text = "Wybierz zarezerwowaną wizytę, aby zobaczyć szczegóły.",
                Location = new Point(14, 12),
                Size = new Size(520, 74),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = normalFont,
                ForeColor = text
            };
            lblPlannedAppointmentTimeLeft = new Label
            {
                Text = "",
                Location = new Point(14, 88),
                Size = new Size(520, 22),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = SismedTheme.Font(9f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };
            txtCancelAppointmentReason = new TextBox
            {
                Location = new Point(14, 118),
                Size = new Size(420, 24),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Font = normalFont,
                ForeColor = text,
                BorderStyle = BorderStyle.FixedSingle
            };
            btnSwapPatientAppointment = CreateActionButton("Zamień wizytę", 394, 114, 140, SismedTheme.Magenta);
            btnSwapPatientAppointment.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnSwapPatientAppointment.Enabled = false;
            btnSwapPatientAppointment.Click += btnSwapPatientAppointment_Click;
            btnCancelPatientAppointment = CreateActionButton("Anuluj wizytę", 548, 114, 150, SismedTheme.Danger);
            btnCancelPatientAppointment.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnCancelPatientAppointment.Enabled = false;
            btnCancelPatientAppointment.Click += btnCancelPatientAppointment_Click;
            pnlPatientPlannedDetails.Resize += (sender, args) =>
            {
                btnCancelPatientAppointment.Left = Math.Max(548, pnlPatientPlannedDetails.Width - 170);
                btnSwapPatientAppointment.Left = Math.Max(394, btnCancelPatientAppointment.Left - 154);
                txtCancelAppointmentReason.Width = Math.Max(240, btnSwapPatientAppointment.Left - 32);
            };
            pnlPatientPlannedDetails.Controls.AddRange(new Control[]
            {
                lblPlannedAppointmentDetails,
                lblPlannedAppointmentTimeLeft,
                txtCancelAppointmentReason,
                btnSwapPatientAppointment,
                btnCancelPatientAppointment
            });
            pnlPatientPlannedPanel.Controls.Add(dgvPatientPlanned);
            pnlPatientPlannedPanel.Controls.Add(pnlPatientPlannedDetails);

            pnlPatientHistoryPanel = CreatePatientActionPanel();
            dgvPatientHistory = CreateAppointmentSummaryGrid();
            pnlPatientHistoryPanel.Controls.Add(dgvPatientHistory);

            pnlPatientBookingPanel = CreatePatientActionPanel();
            pnlPatientBookingTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 136,
                BackColor = SismedTheme.CardSoft,
                Padding = new Padding(12)
            };
            lblPatientBookingService = CreateInlineLabel("Usługa:", 12, 18, muted, labelFont);
            cmbPatientBookingService = CreateComboBox(88, 12, 300);
            cmbPatientBookingService.SelectedIndexChanged += cmbPatientBookingService_SelectedIndexChanged;
            btnPatientBookingNext = CreateActionButton("Dalej", 404, 12, 96, magenta);
            btnPatientBookingNext.Enabled = false;
            btnPatientBookingNext.Click += btnPatientBookingNext_Click;
            lblPatientBookingDoctor = CreateInlineLabel("Lekarz:", 12, 64, muted, labelFont);
            cmbPatientBookingDoctor = CreateComboBox(88, 58, 300);
            cmbPatientBookingDoctor.Enabled = false;
            lblPatientBookingRange = CreateInlineLabel("Zakres:", 404, 64, muted, labelFont);
            cmbPatientBookingRange = CreateComboBox(470, 58, 120);
            cmbPatientBookingRange.Enabled = false;
            btnPatientBookingSearch = CreateActionButton("Szukaj terminów", 606, 58, 154, blue);
            btnPatientBookingSearch.Enabled = false;
            btnPatientBookingSearch.Click += btnPatientBookingSearch_Click;
            lblPatientBookingInfo = new Label
            {
                Text = "Wybierz usługę lub specjalizację, aby rozpocząć umawianie wizyty.",
                Location = new Point(12, 104),
                Size = new Size(760, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = normalFont,
                ForeColor = muted
            };
            pnlPatientBookingTop.Controls.AddRange(new Control[]
            {
                lblPatientBookingService, cmbPatientBookingService, btnPatientBookingNext,
                lblPatientBookingDoctor, cmbPatientBookingDoctor, lblPatientBookingRange,
                cmbPatientBookingRange, btnPatientBookingSearch, lblPatientBookingInfo
            });

            dgvPatientBookingSlots = CreateGrid();
            dgvPatientBookingSlots.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPatientBookingSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "bookDate", HeaderText = "Data", Width = 105 });
            dgvPatientBookingSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "bookDay", HeaderText = "Dzień tygodnia", Width = 120 });
            dgvPatientBookingSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "bookTime", HeaderText = "Godzina", Width = 90 });
            dgvPatientBookingSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "bookDoctor", HeaderText = "Lekarz", Width = 180 });
            dgvPatientBookingSlots.Columns.Add(new DataGridViewTextBoxColumn { Name = "bookService", HeaderText = "Specjalizacja/usługa", Width = 220 });
            dgvPatientBookingSlots.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "bookAction",
                HeaderText = "",
                Text = "Umów",
                UseColumnTextForButtonValue = true,
                Width = 84
            });
            dgvPatientBookingSlots.CellContentClick += dgvPatientBookingSlots_CellContentClick;
            pnlPatientBookingPanel.Controls.Add(dgvPatientBookingSlots);
            pnlPatientBookingPanel.Controls.Add(pnlPatientBookingTop);

            pnlPatientActionBody.Controls.AddRange(new Control[]
            {
                pnlPatientBookingPanel,
                pnlPatientHistoryPanel,
                pnlPatientPlannedPanel,
                pnlPatientNotesPanel,
                pnlPatientResultsPanel,
                pnlPatientEmptyPanel
            });

            pnlPatientActionHost.Controls.Add(pnlPatientActionBody);
            pnlPatientActionHost.Controls.Add(lblPatientActionTitle);

            patientLayout.Controls.Add(pnlPatientDetailsPanel, 0, 0);
            patientLayout.Controls.Add(pnlPatientAddressPanel, 1, 0);
            patientLayout.Controls.Add(pnlPatientActionsPanel, 0, 1);
            patientLayout.SetColumnSpan(pnlPatientActionsPanel, 2);
            patientLayout.Controls.Add(pnlPatientActionHost, 0, 2);
            patientLayout.SetColumnSpan(pnlPatientActionHost, 2);
            tabPatient.Controls.Add(patientLayout);
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

            pnlCalTop = new Panel { Dock = DockStyle.Top, Height = 116, BackColor = card, Padding = new Padding(18) };
            var calFilters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            lblCalDate = CreateInlineLabel("Data", 0, 0, muted, labelFont);
            dtpCal = CreateDatePicker(0, 0);
            lblCalDoctor = CreateInlineLabel("Lekarz", 0, 0, muted, labelFont);
            cmbCalDoctor = CreateComboBox(0, 0, 260);
            lblCalService = CreateInlineLabel("Specjalizacja/usługa", 0, 0, muted, labelFont);
            cmbCalService = CreateComboBox(0, 0, 260);
            lblCalStatus = CreateInlineLabel("Status", 0, 0, muted, labelFont);
            cmbCalStatus = CreateComboBox(0, 0, 180);
            btnLoadCal = CreateActionButton("Pokaż grafik", 0, 0, 160, magenta);
            btnLoadCal.Margin = new Padding(8, 22, 8, 8);
            btnLoadCal.Click += btnLoadCal_Click;

            calFilters.Controls.Add(CreateFilterGroup(lblCalDate, dtpCal, 170));
            calFilters.Controls.Add(CreateFilterGroup(lblCalDoctor, cmbCalDoctor, 280));
            calFilters.Controls.Add(CreateFilterGroup(lblCalService, cmbCalService, 280));
            calFilters.Controls.Add(CreateFilterGroup(lblCalStatus, cmbCalStatus, 200));
            calFilters.Controls.Add(btnLoadCal);
            pnlCalTop.Controls.Add(calFilters);

            pnlCalendarDetails = new Panel
            {
                Dock = DockStyle.Right,
                Width = 330,
                BackColor = card,
                Padding = new Padding(18)
            };
            lblCalendarDetailsTitle = new Label
            {
                Text = "Szczegóły wizyty",
                Dock = DockStyle.Top,
                Height = 34,
                Font = SismedTheme.Font(14f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };
            lblCalendarDetails = new Label
            {
                Text = "Wybierz zarezerwowany slot, aby zobaczyć szczegóły.",
                Dock = DockStyle.Top,
                Height = 260,
                Font = normalFont,
                ForeColor = text
            };
            btnCalendarOpenPatient = CreateActionButton("Przejdź do pacjenta", 0, 0, 210, blue);
            btnCalendarOpenPatient.Dock = DockStyle.Top;
            btnCalendarOpenPatient.Height = 36;
            btnCalendarOpenPatient.Enabled = false;
            btnCalendarOpenPatient.Click += btnCalendarOpenPatient_Click;
            pnlCalendarDetails.Controls.Add(btnCalendarOpenPatient);
            pnlCalendarDetails.Controls.Add(lblCalendarDetails);
            pnlCalendarDetails.Controls.Add(lblCalendarDetailsTitle);

            dgvCal = CreateGrid();
            dgvCal.Dock = DockStyle.Fill;
            dgvCal.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calTime", HeaderText = "Godzina", Width = 90 });
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calDoctor", HeaderText = "Lekarz", Width = 180 });
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calService", HeaderText = "Specjalizacja/usługa", Width = 220 });
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calStatus", HeaderText = "Status", Width = 130 });
            dgvCal.Columns.Add(new DataGridViewTextBoxColumn { Name = "calPatient", HeaderText = "Pacjent", Width = 180 });
            dgvCal.SelectionChanged += dgvCal_SelectionChanged;
            dgvCal.CellFormatting += DgvCal_CellFormatting;

            pnlCalendarScreen.Controls.Add(dgvCal);
            pnlCalendarScreen.Controls.Add(pnlCalendarDetails);
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

            var documentsRoot = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = SismedTheme.Surface
            };
            documentsRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
            documentsRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            documentsRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            documentsRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
            documentsRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var hero = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(24)
            };
            var heroAccent = new Panel
            {
                BackColor = magenta,
                Dock = DockStyle.Left,
                Width = 5
            };
            var heroTitle = new Label
            {
                Text = "Dokumenty",
                Font = SismedTheme.Font(19f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy,
                Location = new Point(24, 18),
                Size = new Size(320, 34)
            };
            var heroSubtitle = new Label
            {
                Text = "Wewnętrzna dokumentacja SISMED: procedury, notatki operacyjne i materiały dla pracowników.",
                Font = normalFont,
                ForeColor = muted,
                Location = new Point(24, 54),
                Size = new Size(780, 24)
            };
            hero.Controls.Add(heroSubtitle);
            hero.Controls.Add(heroTitle);
            hero.Controls.Add(heroAccent);

            pnlDocumentsTop = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Margin = new Padding(0, 0, 12, 12),
                Padding = new Padding(0)
            };

            var documentsActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.Transparent,
                Padding = new Padding(18, 22, 18, 10)
            };

            txtDocumentSearch = CreateTextBox("Szukaj po tytule, kategorii, treści lub autorze", 0, 0, 340);
            txtDocumentSearch.Margin = new Padding(0, 0, 12, 10);
            cmbDocumentStatus = CreateComboBox(0, 0, 142);
            cmbDocumentStatus.Margin = new Padding(0, 4, 12, 10);
            btnDocumentSearch = CreateActionButton("Szukaj", 0, 0, 96, magenta);
            btnDocumentSearch.Margin = new Padding(0, 0, 12, 10);
            btnDocumentSearch.Click += btnDocumentSearch_Click;
            btnDocumentClear = CreateActionButton("Wyczyść", 0, 0, 100, SismedTheme.Blue);
            btnDocumentClear.Margin = new Padding(0, 0, 12, 10);
            btnDocumentClear.Click += btnDocumentClear_Click;
            btnAddDocument = CreateActionButton("+ Dodaj dokument", 0, 0, 156, SismedTheme.Success);
            btnAddDocument.Margin = new Padding(0, 0, 12, 10);
            btnAddDocument.Click += btnAddDocument_Click;
            btnEditDocument = CreateActionButton("Edytuj", 0, 0, 92, SismedTheme.Blue);
            btnEditDocument.Margin = new Padding(0, 0, 12, 10);
            btnEditDocument.Click += btnEditDocument_Click;
            btnArchiveDocument = CreateActionButton("Archiwizuj", 0, 0, 118, SismedTheme.Danger);
            btnArchiveDocument.Margin = new Padding(0, 0, 12, 10);
            btnArchiveDocument.Click += btnArchiveDocument_Click;
            documentsActions.Controls.AddRange(new Control[]
            {
                txtDocumentSearch, cmbDocumentStatus, btnDocumentSearch, btnDocumentClear,
                btnAddDocument, btnEditDocument, btnArchiveDocument
            });
            pnlDocumentsTop.Controls.Add(documentsActions);

            dgvDocuments = CreateGrid();
            dgvDocuments.Dock = DockStyle.Fill;
            dgvDocuments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDocuments.Margin = new Padding(0, 0, 12, 0);
            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "docTitle", HeaderText = "Tytuł", FillWeight = 170 });
            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "docCategory", HeaderText = "Kategoria", FillWeight = 100 });
            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "docAuthor", HeaderText = "Autor", FillWeight = 120 });
            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "docCreated", HeaderText = "Utworzono", FillWeight = 95 });
            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "docUpdated", HeaderText = "Modyfikacja", FillWeight = 95 });
            dgvDocuments.Columns.Add(new DataGridViewTextBoxColumn { Name = "docStatus", HeaderText = "Status", FillWeight = 80 });
            dgvDocuments.SelectionChanged += dgvDocuments_SelectionChanged;

            pnlDocumentDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Padding = new Padding(22)
            };
            lblDocumentDetailsTitle = new Label
            {
                Text = "Wybierz dokument",
                Dock = DockStyle.Top,
                Height = 42,
                Font = SismedTheme.Font(14f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };
            lblDocumentDetailsMeta = new Label
            {
                Text = "Brak wybranego dokumentu.",
                Dock = DockStyle.Top,
                Height = 112,
                Font = SismedTheme.Font(9f),
                ForeColor = muted
            };
            txtDocumentDetailsContent = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = SismedTheme.Font(10f),
                ForeColor = text,
                BackColor = SismedTheme.CardSoft,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlDocumentDetails.Controls.Add(txtDocumentDetailsContent);
            pnlDocumentDetails.Controls.Add(lblDocumentDetailsMeta);
            pnlDocumentDetails.Controls.Add(lblDocumentDetailsTitle);

            documentsRoot.Controls.Add(hero, 0, 0);
            documentsRoot.SetColumnSpan(hero, 2);
            documentsRoot.Controls.Add(pnlDocumentsTop, 0, 1);
            documentsRoot.SetColumnSpan(pnlDocumentsTop, 2);
            documentsRoot.Controls.Add(dgvDocuments, 0, 2);
            documentsRoot.Controls.Add(pnlDocumentDetails, 1, 2);

            pnlDocumentsScreen.Controls.Add(documentsRoot);
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
            btnAddEmployee = CreateActionButton("+ Dodaj pracownika", 580, 20, 180, SismedTheme.Success);
            btnAddEmployee.Click += btnAddEmployee_Click;
            btnEditEmployee = CreateActionButton("Edytuj konto", 774, 20, 140, SismedTheme.Blue);
            btnEditEmployee.Click += btnEditEmployee_Click;
            btnDeactivateEmployee = CreateActionButton("Dezaktywuj", 928, 20, 140, SismedTheme.Danger);
            btnDeactivateEmployee.Click += btnDeactivateEmployee_Click;
            pnlPersonnelTop.Controls.AddRange(new Control[]
            {
                txtEmployeeSearch, btnEmployeeSearch, btnAddEmployee, btnEditEmployee, btnDeactivateEmployee
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
                Font = SismedTheme.Font(9f, FontStyle.Bold),
                ForeColor = SismedTheme.Danger
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

        private void BuildPatientsScreen(Color card, Color text, Color muted, Color magenta)
        {
            pnlPatientsScreen = CreateScreenPanel();

            pnlPatientsTop = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = card, Padding = new Padding(18) };
            txtPatientsSearch = CreateTextBox("Imię / nazwisko / PESEL / telefon / e-mail", 18, 20, 380);
            cmbPatientsFilter = CreateComboBox(412, 24, 220);
            cmbPatientsFilter.Items.Add("Wszyscy pacjenci");
            cmbPatientsFilter.Items.Add("Pacjenci z ostrzeżeniami");
            cmbPatientsFilter.Items.Add("Pacjenci z aktywną blokadą");
            cmbPatientsFilter.SelectedIndex = 0;
            cmbPatientsFilter.SelectedIndexChanged += cmbPatientsFilter_SelectedIndexChanged;
            btnPatientsSearch = CreateActionButton("Szukaj", 650, 20, 110, magenta);
            btnPatientsSearch.Click += btnPatientsSearch_Click;
            btnPatientsClear = CreateActionButton("Wyczyść", 774, 20, 110, SismedTheme.Blue);
            btnPatientsClear.Click += btnPatientsClear_Click;
            pnlPatientsTop.Controls.AddRange(new Control[]
            {
                txtPatientsSearch, cmbPatientsFilter, btnPatientsSearch, btnPatientsClear
            });

            dgvPatients = CreateGrid();
            dgvPatients.Dock = DockStyle.Left;
            dgvPatients.Width = 840;
            dgvPatients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patFirstName", HeaderText = "Imię", Width = 105 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patLastName", HeaderText = "Nazwisko", Width = 120 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patPesel", HeaderText = "PESEL", Width = 110 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patBirth", HeaderText = "Data ur.", Width = 95 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patPhone", HeaderText = "Telefon", Width = 95 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patEmail", HeaderText = "E-mail", Width = 180 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patCity", HeaderText = "Miasto", Width = 110 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patPostal", HeaderText = "Kod", Width = 70 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patStreet", HeaderText = "Ulica", Width = 130 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patHouse", HeaderText = "Dom", Width = 60 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patApartment", HeaderText = "Lokal", Width = 60 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patWarnings", HeaderText = "Ostrz.", Width = 70 });
            dgvPatients.Columns.Add(new DataGridViewTextBoxColumn { Name = "patBlocked", HeaderText = "Blokada", Width = 140 });
            dgvPatients.SelectionChanged += dgvPatients_SelectionChanged;

            pnlPatientDirectoryDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = card,
                Padding = new Padding(28),
                AutoScroll = true
            };

            btnOpenPatientReception = CreateActionButton("Otwórz w recepcji", 0, 0, 180, magenta);
            btnOpenPatientReception.Dock = DockStyle.Top;
            btnOpenPatientReception.Height = 38;
            btnOpenPatientReception.Click += btnOpenPatientReception_Click;

            lblDirectoryPatientTitle = CreateDetailLabel("Dane pacjenta", text, true);
            lblDirectoryPatientName = CreateDetailLabel("Imię i nazwisko: -", text, true);
            lblDirectoryPatientPesel = CreateDetailLabel("PESEL: -", muted, false);
            lblDirectoryPatientBirthDate = CreateDetailLabel("Data urodzenia: -", muted, false);
            lblDirectoryPatientPhone = CreateDetailLabel("Telefon: -", muted, false);
            lblDirectoryPatientEmail = CreateDetailLabel("E-mail: -", muted, false);
            lblDirectoryPatientCity = CreateDetailLabel("Miasto: -", muted, false);
            lblDirectoryPatientPostalCode = CreateDetailLabel("Kod pocztowy: -", muted, false);
            lblDirectoryPatientStreet = CreateDetailLabel("Ulica: -", muted, false);
            lblDirectoryPatientHouseNumber = CreateDetailLabel("Numer domu: -", muted, false);
            lblDirectoryPatientApartmentNumber = CreateDetailLabel("Numer lokalu: -", muted, false);
            lblDirectoryPatientWarnings = CreateDetailLabel("Ostrzeżenia: -", muted, false);
            lblDirectoryPatientBlock = CreateDetailLabel("Blokada rezerwacji: -", muted, false);
            lblDirectoryPatientNotesCount = CreateDetailLabel("Notatki: -", muted, false);
            lblDirectoryPatientReservedCount = CreateDetailLabel("Zarezerwowane wizyty: -", muted, false);
            lblDirectoryPatientHistoryCount = CreateDetailLabel("Historia wizyt: -", muted, false);

            pnlPatientDirectoryDetails.Controls.AddRange(new Control[]
            {
                btnOpenPatientReception,
                lblDirectoryPatientHistoryCount,
                lblDirectoryPatientReservedCount,
                lblDirectoryPatientNotesCount,
                lblDirectoryPatientBlock,
                lblDirectoryPatientWarnings,
                lblDirectoryPatientApartmentNumber,
                lblDirectoryPatientHouseNumber,
                lblDirectoryPatientStreet,
                lblDirectoryPatientPostalCode,
                lblDirectoryPatientCity,
                lblDirectoryPatientEmail,
                lblDirectoryPatientPhone,
                lblDirectoryPatientBirthDate,
                lblDirectoryPatientPesel,
                lblDirectoryPatientName,
                lblDirectoryPatientTitle
            });

            pnlPatientsScreen.Controls.Add(pnlPatientDirectoryDetails);
            pnlPatientsScreen.Controls.Add(dgvPatients);
            pnlPatientsScreen.Controls.Add(pnlPatientsTop);
        }

        private Panel CreateScreenPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.Surface,
                Visible = false
            };
        }

        private Panel CreateMetricCard(string title, Label valueLabel, Color accent)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.Card,
                Margin = new Padding(0, 0, SismedTheme.Gap, 0),
                Padding = new Padding(18)
            };

            var accentBar = new Panel
            {
                BackColor = accent,
                Dock = DockStyle.Left,
                Width = 5
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = SismedTheme.Font(9f, FontStyle.Bold),
                ForeColor = SismedTheme.Muted,
                Location = new Point(22, 18),
                Size = new Size(240, 22)
            };

            valueLabel.Text = "0";
            valueLabel.Font = SismedTheme.Font(22f, FontStyle.Bold);
            valueLabel.ForeColor = SismedTheme.Navy;
            valueLabel.Location = new Point(22, 44);
            valueLabel.Size = new Size(240, 40);

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(accentBar);
            return card;
        }

        private TextBox CreateTextBox(string placeholder, int left, int top, int width)
        {
            var textBox = new TextBox
            {
                Location = new Point(left, top),
                Size = new Size(width, 36),
            };
            SismedTheme.ApplyTextBox(textBox, placeholder);
            return textBox;
        }

        private Button CreateActionButton(string text, int left, int top, int width, Color fill)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, 36)
            };
            if (fill == SismedTheme.Magenta)
            {
                SismedTheme.ApplyPrimaryButton(button);
            }
            else if (fill == SismedTheme.Success)
            {
                SismedTheme.ApplySuccessButton(button);
            }
            else if (fill == SismedTheme.Danger)
            {
                SismedTheme.ApplyDangerButton(button);
            }
            else
            {
                SismedTheme.ApplySecondaryButton(button);
                button.BackColor = fill;
            }

            return button;
        }

        private ComboBox CreateComboBox(int left, int top, int width)
        {
            return new ComboBox
            {
                Location = new Point(left, top),
                Size = new Size(width, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = SismedTheme.Font(9f)
            };
        }

        private DateTimePicker CreateDatePicker(int left, int top)
        {
            return new DateTimePicker
            {
                Location = new Point(left, top),
                Size = new Size(150, 36),
                CustomFormat = "dd.MM.yyyy",
                Format = DateTimePickerFormat.Custom,
                Value = DateTime.Today,
            };
        }

        private DataGridView CreateGrid()
        {
            var grid = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeight = 34,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                Dock = DockStyle.Fill,
                Font = SismedTheme.Font(9f),
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                RowTemplate = { Height = 30 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false
            };
            SismedTheme.ApplyGrid(grid);
            return grid;
        }

        private Panel CreatePatientActionPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SismedTheme.Card,
                Visible = false
            };
        }

        private DataGridView CreateAppointmentSummaryGrid()
        {
            DataGridView grid = CreateGrid();
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "apTerm", HeaderText = "Termin wykonania", Width = 170 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "apService", HeaderText = "Usługa", Width = 220 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "apDoctor", HeaderText = "Lekarz", Width = 200 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "apStatus", HeaderText = "Status wizyty", Width = 140 });
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

        private Panel CreateFilterGroup(Label label, Control input, int width)
        {
            var panel = new Panel
            {
                Width = width,
                Height = 72,
                Margin = new Padding(0, 0, 14, 8),
                BackColor = Color.Transparent
            };

            label.Location = new Point(0, 0);
            label.Size = new Size(width, 20);
            input.Location = new Point(0, 26);
            input.Width = width;
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel.Controls.Add(input);
            panel.Controls.Add(label);
            return panel;
        }

        private Label CreateInfoLabel(string text, int left, int top, Color color, bool bold)
        {
            return new Label
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(240, 38),
                Font = SismedTheme.Font(9f, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color
            };
        }

        private Label CreatePatientDetailLabel(string text, int left, int top, Color color, bool bold, int height)
        {
            return new Label
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(296, height),
                Font = SismedTheme.Font(bold ? 9.5f : 9f, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color,
                AutoEllipsis = true
            };
        }

        private Label CreateAddressCaption(string text, int left, int top, int width)
        {
            return new Label
            {
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, 18),
                Font = SismedTheme.Font(8.5f, FontStyle.Bold),
                ForeColor = SismedTheme.Muted
            };
        }

        private Label CreateDetailLabel(string text, Color color, bool bold)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = bold ? 46 : 34,
                Font = SismedTheme.Font(bold ? 13f : 10f, bold ? FontStyle.Bold : FontStyle.Regular),
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

            if (status == "Dostępny")
            {
                row.DefaultCellStyle.ForeColor = SismedTheme.Success;
            }
            else if (status == "Zarezerwowana")
            {
                row.DefaultCellStyle.ForeColor = SismedTheme.Danger;
            }
            else if (status == "Anulowana")
            {
                row.DefaultCellStyle.ForeColor = SismedTheme.Muted;
            }
            else if (status == "Historyczna/Zakończona")
            {
                row.DefaultCellStyle.ForeColor = SismedTheme.Warning;
            }
        }

        private Panel pnlNavigation, pnlShell, pnlTopBar, pnlScreenHost;
        private Panel pnlSearchScreen, pnlReceptionScreen, pnlCalendarScreen, pnlDocumentsScreen, pnlPersonnelScreen, pnlPatientsScreen;
        private Panel pnlSearchTop, pnlSearchResults;
        private Panel pnlReceptionContent, pnlBookTop, pnlCalTop;
        private Panel pnlPatientActionBody, pnlPatientEmptyPanel, pnlPatientResultsPanel, pnlPatientNotesPanel;
        private Panel pnlPatientPlannedPanel, pnlPatientHistoryPanel, pnlPatientBookingPanel, pnlPatientBookingTop;
        private Panel pnlPatientPlannedDetails;
        private Panel pnlReservedActions, pnlPersonnelTop, pnlEmployeeDetails, pnlCalendarDetails;
        private Panel pnlPatientsTop, pnlPatientDirectoryDetails;
        private TableLayoutPanel pnlDashboardCards, patientLayout;
        private Panel pnlPatientDetailsPanel, pnlPatientAddressPanel, pnlPatientActionHost, pnlDocumentsTop, pnlDocumentDetails;
        private FlowLayoutPanel pnlPatientActionsPanel;
        private PictureBox picLogo;
        private Label lblNavTitle, lblNavSection, lblScreenTitle, lblCurrentUser, lblPersonnelAccess;
        private Label lblSearchResultsTitle, lblSearchResultsInfo;
        private Label lblPatientPanelTitle, lblPatientPanelName, lblPatientPanelPesel, lblPatientPanelBirthDate;
        private Label lblPatientPanelPhone, lblPatientPanelEmail, lblPatientPanelWarnings;
        private Label lblPatientAddressTitle, lblPatientAddressHint;
        private Label lblPatientPanelBlock, lblPatientActionTitle, lblPatientEmptyInfo, lblPatientBookingInfo;
        private Label lblPatientBookingService, lblPatientBookingDoctor, lblPatientBookingRange;
        private Label lblPlannedAppointmentDetails, lblPlannedAppointmentTimeLeft;
        private Label lblTodayVisitsValue, lblPlannedVisitsValue;
        private Label lblBookDoctor, lblBookDate, lblCalDoctor, lblCalDate, lblCalService, lblCalStatus;
        private Label lblCalendarDetailsTitle, lblCalendarDetails;
        private Label lblDocumentDetailsTitle, lblDocumentDetailsMeta;
        private Label lblSwapResult;
        private Label lblEmployeeName, lblEmployeePesel, lblEmployeeBirthDate, lblEmployeeLogin;
        private Label lblEmployeeRole, lblEmployeeStatus, lblEmployeeDoctor, lblEmployeeSpecialization;
        private Label lblDirectoryPatientTitle, lblDirectoryPatientName, lblDirectoryPatientPesel, lblDirectoryPatientBirthDate;
        private Label lblDirectoryPatientPhone, lblDirectoryPatientEmail, lblDirectoryPatientCity, lblDirectoryPatientPostalCode;
        private Label lblDirectoryPatientStreet, lblDirectoryPatientHouseNumber, lblDirectoryPatientApartmentNumber;
        private Label lblDirectoryPatientWarnings, lblDirectoryPatientBlock, lblDirectoryPatientNotesCount;
        private Label lblDirectoryPatientReservedCount, lblDirectoryPatientHistoryCount;
        private TextBox txtPatientPesel, txtPatientFirstName, txtPatientLastName, txtPatientBirthDate;
        private TextBox txtPatientPhone, txtPatientEmail, txtSwapSearch, txtEmployeeSearch, txtDocumentSearch, txtPatientsSearch;
        private TextBox txtPatientCity, txtPatientPostalCode, txtPatientStreet, txtPatientHouseNumber, txtPatientApartmentNumber;
        private TextBox txtPatientNote, txtCancelAppointmentReason, txtDocumentDetailsContent;
        private Button btnNavSearch, btnNavCalendar, btnNavReception, btnNavDocuments, btnNavPersonnel, btnNavPatients;
        private Button btnSearch, btnClearPatientSearch, btnAddPatient, btnLogout, btnLoadSlots, btnReserve;
        private Button btnLoadCal, btnCancel, btnSwap, btnSwapFind, btnClose;
        private Button btnCalendarOpenPatient;
        private Button btnDocumentSearch, btnDocumentClear, btnAddDocument, btnEditDocument, btnArchiveDocument;
        private Button btnPatientEditData, btnPatientMessages, btnPatientBook, btnPatientPlanned, btnPatientHistory;
        private Button btnPatientBookingNext, btnPatientBookingSearch;
        private Button btnCancelPatientAppointment, btnSwapPatientAppointment;
        private Button btnAddPatientNote, btnDeletePatientNote;
        private Button btnEmployeeSearch, btnAddEmployee, btnEditEmployee, btnDeactivateEmployee;
        private Button btnPatientsSearch, btnPatientsClear, btnOpenPatientReception;
        private ComboBox cmbDoctor, cmbCalDoctor, cmbCalService, cmbCalStatus, cmbDocumentStatus;
        private ComboBox cmbPatientsFilter;
        private ComboBox cmbPatientBookingService, cmbPatientBookingDoctor, cmbPatientBookingRange;
        private DateTimePicker dtpBook, dtpCal;
        private DataGridView dgvSearchResults, dgvSlots, dgvCal, dgvReserved, dgvEmployees, dgvDocuments, dgvPatients;
        private DataGridView dgvPatientResults, dgvPatientNotes, dgvPatientPlanned, dgvPatientHistory;
        private DataGridView dgvPatientBookingSlots;
        private TabControl tabControl;
        private TabPage tabPatient, tabBook, tabReserved;
    }
}
