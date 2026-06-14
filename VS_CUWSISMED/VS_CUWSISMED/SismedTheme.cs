using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    internal static class SismedTheme
    {
        public static readonly Color Navy = Color.FromArgb(9, 24, 70);
        public static readonly Color NavyDark = Color.FromArgb(6, 15, 46);
        public static readonly Color Blue = Color.FromArgb(28, 79, 179);
        public static readonly Color Magenta = Color.FromArgb(218, 0, 148);
        public static readonly Color MagentaSoft = Color.FromArgb(248, 229, 242);
        public static readonly Color Surface = Color.FromArgb(244, 247, 252);
        public static readonly Color Card = Color.White;
        public static readonly Color CardSoft = Color.FromArgb(248, 250, 254);
        public static readonly Color Border = Color.FromArgb(220, 228, 241);
        public static readonly Color Text = Color.FromArgb(25, 34, 58);
        public static readonly Color Muted = Color.FromArgb(92, 104, 128);
        public static readonly Color SidebarMuted = Color.FromArgb(178, 194, 228);
        public static readonly Color Success = Color.FromArgb(0, 135, 101);
        public static readonly Color Danger = Color.FromArgb(194, 45, 62);
        public static readonly Color Warning = Color.FromArgb(226, 134, 36);

        public const int SidebarWidth = 276;
        public const int Radius = 10;
        public const int Gap = 12;
        public const int Padding = 24;

        private static readonly string FontFamilyName = ResolveFontFamilyName();
        private const int EM_SETCUEBANNER = 0x1501;

        public static Font Font(float size)
        {
            return Font(size, FontStyle.Regular);
        }

        public static Font Font(float size, FontStyle style)
        {
            return new Font(FontFamilyName, size, style, GraphicsUnit.Point);
        }

        public static Font GetFont(float size)
        {
            return Font(size);
        }

        public static Font GetFont(float size, FontStyle style)
        {
            return Font(size, style);
        }

        public static void ApplyTextBox(TextBox textBox)
        {
            ApplyTextBox(textBox, null);
        }

        public static void ApplyTextBox(TextBox textBox, string placeholder)
        {
            textBox.Font = Font(9f);
            textBox.ForeColor = Text;
            textBox.BackColor = Card;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.AutoSize = false;

            if (!string.IsNullOrWhiteSpace(placeholder))
            {
                ApplyCueBanner(textBox, placeholder);
            }
        }

        public static void ApplyPrimaryButton(Button button)
        {
            ApplyButton(button, Magenta, Color.White);
        }

        public static void ApplySecondaryButton(Button button)
        {
            ApplyButton(button, Blue, Color.White);
        }

        public static void ApplySuccessButton(Button button)
        {
            ApplyButton(button, Success, Color.White);
        }

        public static void ApplyDangerButton(Button button)
        {
            ApplyButton(button, Danger, Color.White);
        }

        public static void ApplyOutlineButton(Button button)
        {
            ApplyButton(button, Color.Transparent, Magenta);
            button.FlatAppearance.BorderColor = Magenta;
            button.FlatAppearance.BorderSize = 1;
        }

        public static void ApplyGrid(DataGridView grid)
        {
            grid.BackgroundColor = Card;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.Font = Font(9f);
            grid.ColumnHeadersDefaultCellStyle.BackColor = Navy;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = Font(9f, FontStyle.Bold);
            grid.ColumnHeadersHeight = 36;
            grid.DefaultCellStyle.BackColor = Card;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Magenta;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = CardSoft;
            grid.GridColor = Border;
            grid.RowTemplate.Height = 32;
        }

        public static void FitFormToWorkingArea(Form form)
        {
            if (form == null)
            {
                return;
            }

            Rectangle workingArea = Screen.FromControl(form).WorkingArea;
            int maxWidth = Math.Max(360, workingArea.Width - 40);
            int maxHeight = Math.Max(320, workingArea.Height - 40);

            if (form.MinimumSize.Width > maxWidth || form.MinimumSize.Height > maxHeight)
            {
                form.MinimumSize = new Size(
                    Math.Min(form.MinimumSize.Width, maxWidth),
                    Math.Min(form.MinimumSize.Height, maxHeight));
            }

            int width = Math.Min(form.Width, maxWidth);
            int height = Math.Min(form.Height, maxHeight);
            if (width != form.Width || height != form.Height)
            {
                form.Size = new Size(width, height);
            }

            form.Left = workingArea.Left + Math.Max(0, (workingArea.Width - form.Width) / 2);
            form.Top = workingArea.Top + Math.Max(0, (workingArea.Height - form.Height) / 2);
        }

        private static void ApplyButton(Button button, Color fill, Color fore)
        {
            button.Font = Font(9f, FontStyle.Bold);
            button.BackColor = fill;
            button.ForeColor = fore;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = fill == Color.Transparent ? Magenta : fill;
            button.FlatAppearance.BorderSize = fill == Color.Transparent ? 1 : 0;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
        }

        private static void ApplyCueBanner(TextBox textBox, string placeholder)
        {
            if (textBox.IsHandleCreated)
            {
                SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, placeholder);
                return;
            }

            textBox.HandleCreated += (sender, args) =>
            {
                SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, placeholder);
            };
        }

        private static string ResolveFontFamilyName()
        {
            bool hasMulish = FontFamily.Families.Any(f => f.Name == "Mulish");
            return hasMulish ? "Mulish" : "Segoe UI";
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
    }
}
