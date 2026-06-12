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
                main_app frm = new main_app(employee);
                frm.FormClosed += (closedSender, args) =>
                {
                    if (!IsDisposed)
                    {
                        Show();
                    }
                };
                frm.Show(this);
                Hide();
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

        private void bttnregister_Click(object sender, EventArgs e)
        {
            using (var dialog = new RegisterEmployeeDialog())
            {
                dialog.ShowDialog(this);
            }
        }
    }
}
