using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace VS_CUWSISMED
{
    public partial class login_page : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public Employee AuthenticatedEmployee { get; private set; }

        public login_page()
        {
            InitializeComponent();
            this.MouseDown += MoveForm;

            txtpassword.PasswordChar = '●';
            txtpassword.UseSystemPasswordChar = false;
        }
        private void MoveForm(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void bttnlogin_Click(object sender, EventArgs e)
        {
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
        private bool pokazHaslo = false;
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
