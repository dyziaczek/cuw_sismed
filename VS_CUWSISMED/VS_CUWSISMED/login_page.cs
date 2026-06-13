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

            if (DesignTimeHelper.IsActive)
            {
                return;
            }

            MouseDown += MoveForm;
            txtpassword.PasswordChar = '●';
            txtpassword.UseSystemPasswordChar = false;
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

        private void btnCloseLogin_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bttnshowpassword_Click(object sender, EventArgs e)
        {
            pokazHaslo = !pokazHaslo;

            if (pokazHaslo)
            {
                txtpassword.PasswordChar = '\0';
                bttnshowpassword.Image = Properties.Resources.eye_lined;
            }
            else
            {
                txtpassword.PasswordChar = '●';
                bttnshowpassword.Image = Properties.Resources.open_eye;
            }
        }
    }
}
