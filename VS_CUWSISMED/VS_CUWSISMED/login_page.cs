using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
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
        }
        private void MoveForm(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void bttnlogin_Click(object sender, EventArgs e)
        {
            string login = txtusername.Text;
            string haslo = txtpassword.Text;

            if (login == "rejestrator" && haslo == "admin")
            {
                mainapp frm = new mainapp();
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Błędne dane logowania, spróbuj ponownie");
            }
        }

        private void login_page_Load(object sender, EventArgs e)
        {

        }
    }

}
