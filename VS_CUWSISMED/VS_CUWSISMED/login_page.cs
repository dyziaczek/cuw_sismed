using System;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public partial class login_page : System.Windows.Forms.Form
    {
        private bool pokazHaslo;

        public Employee AuthenticatedEmployee { get; private set; }

        public login_page()
        {
            InitializeComponent();
            PositionCloseButton();

            if (DesignTimeHelper.IsActive)
            {
                return;
            }

            WireRuntimeEvents();
            txtpassword.PasswordChar = '●';
            txtpassword.UseSystemPasswordChar = false;
        }

        private void WireRuntimeEvents()
        {
            bttnlogin.Click -= bttnlogin_Click;
            bttnlogin.Click += bttnlogin_Click;
            bttnshowpassword.Click -= bttnshowpassword_Click;
            bttnshowpassword.Click += bttnshowpassword_Click;
            btnCloseLogin.Click -= btnCloseLogin_Click;
            btnCloseLogin.Click += btnCloseLogin_Click;
            btnCloseLogin.MouseEnter -= btnCloseLogin_MouseEnter;
            btnCloseLogin.MouseEnter += btnCloseLogin_MouseEnter;
            btnCloseLogin.MouseLeave -= btnCloseLogin_MouseLeave;
            btnCloseLogin.MouseLeave += btnCloseLogin_MouseLeave;
            pnlVisual.Resize -= pnlVisual_Resize;
            pnlVisual.Resize += pnlVisual_Resize;

            AttachDragSurface(this);
            AttachDragSurface(pnlLogin);
            AttachDragSurface(pnlVisual);
            AttachDragSurface(pictureBox1);
            AttachDragSurface(lblWelcome);
            AttachDragSurface(lblHint);
            AttachDragSurface(lblVisualTitle);
            AttachDragSurface(lblVisualSub);
            AttachDragSurface(lblVisualFooter);
            AttachDragSurface(label1);
            AttachDragSurface(pnlAccent);
        }

        private void AttachDragSurface(Control control)
        {
            if (control == null)
            {
                return;
            }

            control.MouseDown -= MoveForm;
            control.MouseDown += MoveForm;
        }

        private void MoveForm(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowDragHelper.DragWindow(Handle);
            }
        }

        private void bttnlogin_Click(object sender, EventArgs e)
        {
            if (DesignTimeHelper.IsActive)
            {
                return;
            }

            Employee employee = AppServices.AuthService.Authenticate(txtusername.Text, txtpassword.Text);

            if (employee != null)
            {
                AuthenticatedEmployee = employee;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Błędne dane logowania, spróbuj ponownie");
            }
        }

        private void login_page_Load(object sender, EventArgs e)
        {
        }

        private void pnlVisual_Resize(object sender, EventArgs e)
        {
            PositionCloseButton();
        }

        private void PositionCloseButton()
        {
            if (btnCloseLogin == null || pnlVisual == null)
            {
                return;
            }

            int left = Math.Max(12, pnlVisual.ClientSize.Width - btnCloseLogin.Width - 14);
            btnCloseLogin.Location = new System.Drawing.Point(left, 14);
            btnCloseLogin.BringToFront();
        }

        private void btnCloseLogin_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnCloseLogin_MouseEnter(object sender, EventArgs e)
        {
            btnCloseLogin.BackColor = System.Drawing.Color.FromArgb(22, 39, 88);
            btnCloseLogin.ForeColor = System.Drawing.Color.White;
        }

        private void btnCloseLogin_MouseLeave(object sender, EventArgs e)
        {
            btnCloseLogin.BackColor = SismedTheme.Navy;
            btnCloseLogin.ForeColor = System.Drawing.Color.White;
        }

        private void bttnshowpassword_Click(object sender, EventArgs e)
        {
            pokazHaslo = !pokazHaslo;

            if (pokazHaslo)
            {
                txtpassword.PasswordChar = '\0';
                bttnshowpassword.Text = "Ukryj";
            }
            else
            {
                txtpassword.PasswordChar = '●';
                bttnshowpassword.Text = "Pokaż";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblVisualTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
