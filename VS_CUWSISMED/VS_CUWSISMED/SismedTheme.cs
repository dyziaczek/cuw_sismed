using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

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

        public static Font Font(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(FontFamilyName, size, style, GraphicsUnit.Point);
        }

        public static void ApplyTextBox(Guna2TextBox textBox)
        {
            textBox.Font = Font(9f);
            textBox.ForeColor = Text;
            textBox.PlaceholderForeColor = Muted;
            textBox.BorderColor = Border;
            textBox.BorderThickness = 1;
            textBox.BorderRadius = Radius;
            textBox.FillColor = Card;
            textBox.FocusedState.BorderColor = Magenta;
            textBox.HoverState.BorderColor = Blue;
        }

        public static void ApplyPrimaryButton(Guna2Button button)
        {
            ApplyButton(button, Magenta, Color.White);
        }

        public static void ApplySecondaryButton(Guna2Button button)
        {
            ApplyButton(button, Blue, Color.White);
        }

        public static void ApplySuccessButton(Guna2Button button)
        {
            ApplyButton(button, Success, Color.White);
        }

        public static void ApplyDangerButton(Guna2Button button)
        {
            ApplyButton(button, Danger, Color.White);
        }

        public static void ApplyOutlineButton(Guna2Button button)
        {
            ApplyButton(button, Color.Transparent, Magenta);
            button.BorderColor = Magenta;
            button.BorderThickness = 1;
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

        private static void ApplyButton(Guna2Button button, Color fill, Color fore)
        {
            button.Font = Font(9f, FontStyle.Bold);
            button.FillColor = fill;
            button.ForeColor = fore;
            button.BorderRadius = Radius;
            button.BorderColor = Color.Transparent;
            button.Cursor = Cursors.Hand;
        }

        private static string ResolveFontFamilyName()
        {
            bool hasMulish = FontFamily.Families.Any(f => f.Name == "Mulish");
            return hasMulish ? "Mulish" : "Segoe UI";
        }
    }
}
