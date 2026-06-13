using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    internal sealed class LoginInputPanel : Panel
    {
        public LoginInputPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
        }

        public int Radius { get; set; } = 18;
        public Color BorderColor { get; set; } = Color.FromArgb(176, 190, 215);
        public Color FocusedBorderColor { get; set; } = SismedTheme.Magenta;
        public float BorderThickness { get; set; } = 1.8f;

        private bool isFocused;

        protected override void OnResize(System.EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle borderBounds = new Rectangle(1, 1, Width - 3, Height - 3);
            using (GraphicsPath path = RoundedPath(borderBounds, Radius))
            using (Pen pen = new Pen(isFocused ? FocusedBorderColor : BorderColor, BorderThickness))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            AttachFocusEvents(e.Control);
        }

        private void AttachFocusEvents(Control control)
        {
            control.Enter += (sender, args) => SetFocused(true);
            control.Leave += (sender, args) =>
            {
                if (!ContainsFocus)
                {
                    SetFocused(false);
                }
            };
        }

        private void SetFocused(bool focused)
        {
            if (isFocused == focused)
            {
                return;
            }

            isFocused = focused;
            Invalidate();
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            using (GraphicsPath path = RoundedPath(ClientRectangle, Radius))
            {
                Region = new Region(path);
            }
        }

        private static GraphicsPath RoundedPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();
            Rectangle arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter - 1;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter - 1;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.X;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    internal sealed class LoginPrimaryButton : Button
    {
        private bool isHovered;

        public LoginPrimaryButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
        }

        public int Radius { get; set; } = 18;

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color fill = Enabled
                ? (isHovered ? Color.FromArgb(197, 0, 133) : SismedTheme.Magenta)
                : SismedTheme.Border;

            using (GraphicsPath path = RoundedPath(ClientRectangle, Radius))
            using (SolidBrush brush = new SolidBrush(fill))
            {
                pevent.Graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                pevent.Graphics,
                Text,
                Font,
                ClientRectangle,
                Enabled ? Color.White : SismedTheme.Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            using (GraphicsPath path = RoundedPath(ClientRectangle, Radius))
            {
                Region = new Region(path);
            }
        }

        private static GraphicsPath RoundedPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();
            Rectangle arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter - 1;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter - 1;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.X;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

}
