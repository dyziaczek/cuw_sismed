using System;
using System.Drawing;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public sealed class DocumentDialog : Form
    {
        private readonly SismedDocument originalDocument;
        private readonly TextBox txtTitle;
        private readonly TextBox txtCategory;
        private readonly TextBox txtContent;
        private readonly ComboBox cmbStatus;
        private readonly Button btnSave;
        private readonly Button btnCancel;

        private sealed class StatusOption
        {
            public DocumentStatus Status { get; set; }
            public string Text { get; set; }
        }

        public SismedDocument Document { get; private set; }

        public DocumentDialog()
            : this(null, true)
        {
        }

        public DocumentDialog(SismedDocument document, bool allowStatusSelection)
        {
            originalDocument = document;

            Text = document == null ? "Dodaj dokument" : "Edytuj dokument";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new Size(640, 560);
            MinimumSize = new Size(520, 460);
            BackColor = SismedTheme.Surface;
            Shown += (sender, args) => SismedTheme.FitFormToWorkingArea(this);

            var title = new Label
            {
                Text = document == null ? "Nowy dokument" : "Edycja dokumentu",
                Location = new Point(24, 20),
                Size = new Size(430, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Font = SismedTheme.Font(16f, FontStyle.Bold),
                ForeColor = SismedTheme.Navy
            };

            txtTitle = CreateTextBox("Tytuł dokumentu", 24, 76, 592);
            txtCategory = CreateTextBox("Kategoria", 24, 124, 360);

            var lblStatus = new Label
            {
                Text = "Status",
                Location = new Point(408, 100),
                Size = new Size(180, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = SismedTheme.Font(8.5f, FontStyle.Bold),
                ForeColor = SismedTheme.Muted
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(408, 124),
                Size = new Size(208, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = SismedTheme.Font(9f),
                Enabled = allowStatusSelection
            };
            cmbStatus.DisplayMember = "Text";
            cmbStatus.Items.Add(new StatusOption { Status = DocumentStatus.Active, Text = "Aktywny" });
            cmbStatus.Items.Add(new StatusOption { Status = DocumentStatus.Archived, Text = "Archiwalny" });
            cmbStatus.SelectedIndex = 0;

            var lblContent = new Label
            {
                Text = "Treść dokumentu",
                Location = new Point(24, 176),
                Size = new Size(240, 22),
                Font = SismedTheme.Font(9f, FontStyle.Bold),
                ForeColor = SismedTheme.Muted
            };

            txtContent = new TextBox
            {
                Location = new Point(24, 204),
                Size = new Size(592, 276),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = SismedTheme.Font(10f),
                ForeColor = SismedTheme.Text,
                BackColor = SismedTheme.Card,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSave = new Button
            {
                Text = "Zapisz",
                Location = new Point(424, 504),
                Size = new Size(90, 34),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            SismedTheme.ApplySuccessButton(btnSave);
            btnSave.Click += btnSave_Click;

            btnCancel = new Button
            {
                Text = "Anuluj",
                Location = new Point(526, 504),
                Size = new Size(90, 34),
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
                title, txtTitle, txtCategory, lblStatus, cmbStatus, lblContent, txtContent,
                btnSave, btnCancel
            });

            LoadDocument(document);
        }

        private TextBox CreateTextBox(string placeholder, int left, int top, int width)
        {
            var textBox = new TextBox
            {
                Location = new Point(left, top),
                Size = new Size(width, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            SismedTheme.ApplyTextBox(textBox, placeholder);
            return textBox;
        }

        private void LoadDocument(SismedDocument document)
        {
            if (document == null)
            {
                return;
            }

            txtTitle.Text = document.Title;
            txtCategory.Text = document.Category;
            txtContent.Text = document.Content;

            for (int i = 0; i < cmbStatus.Items.Count; i++)
            {
                StatusOption option = cmbStatus.Items[i] as StatusOption;
                if (option != null && option.Status == document.Status)
                {
                    cmbStatus.SelectedIndex = i;
                    break;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string category = txtCategory.Text.Trim();
            string content = txtContent.Text.Trim();

            if (title.Length == 0)
            {
                MessageBox.Show("Podaj tytuł dokumentu.", "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (category.Length == 0)
            {
                MessageBox.Show("Podaj kategorię dokumentu.", "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (content.Length == 0)
            {
                MessageBox.Show("Treść dokumentu nie może być pusta.", "SISMED", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StatusOption option = cmbStatus.SelectedItem as StatusOption;
            DocumentStatus status = option == null ? DocumentStatus.Active : option.Status;

            Document = new SismedDocument
            {
                Id = originalDocument == null ? 0 : originalDocument.Id,
                Title = title,
                Category = category,
                Content = content,
                Author = originalDocument == null ? string.Empty : originalDocument.Author,
                CreatedAt = originalDocument == null ? DateTime.MinValue : originalDocument.CreatedAt,
                UpdatedAt = originalDocument == null ? DateTime.MinValue : originalDocument.UpdatedAt,
                Status = status,
                LastEditedBy = originalDocument == null ? string.Empty : originalDocument.LastEditedBy
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
