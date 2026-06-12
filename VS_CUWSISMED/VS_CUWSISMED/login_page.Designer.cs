using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace VS_CUWSISMED
{
    partial class login_page
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(login_page));
            guna2ControlBox1 = new Guna2ControlBox();
            guna2Elipse1 = new Guna2Elipse(components);
            guna2Elipse2 = new Guna2Elipse(components);
            pnlLogin = new Panel();
            pictureBox1 = new PictureBox();
            lblWelcome = new Label();
            lblHint = new Label();
            txtusername = new Guna2TextBox();
            txtpassword = new Guna2TextBox();
            bttnshowpassword = new Guna2Button();
            bttnlogin = new Guna2Button();
            pnlVisual = new Panel();
            lblVisualTitle = new Label();
            lblVisualSub = new Label();
            lblVisualFooter = new Label();
            pnlAccent = new Panel();
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
            pnlLogin.SuspendLayout();
            pnlVisual.SuspendLayout();
            SuspendLayout();

            BackColor = SismedTheme.Surface;
            ClientSize = new Size(760, 460);
            DoubleBuffered = true;
            Font = SismedTheme.Font(9f);
            FormBorderStyle = FormBorderStyle.None;
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Name = "login_page";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CUW SISMED - logowanie";
            Load += login_page_Load;

            guna2Elipse1.BorderRadius = 24;
            guna2Elipse1.TargetControl = this;
            guna2Elipse2.BorderRadius = 20;
            guna2Elipse2.TargetControl = pnlLogin;

            guna2ControlBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            guna2ControlBox1.BackColor = Color.Transparent;
            guna2ControlBox1.BorderColor = Color.Transparent;
            guna2ControlBox1.FillColor = Color.Transparent;
            guna2ControlBox1.IconColor = Color.White;
            guna2ControlBox1.Location = new Point(716, 14);
            guna2ControlBox1.Size = new Size(30, 30);
            guna2ControlBox1.TabIndex = 0;

            pnlLogin.BackColor = SismedTheme.Card;
            pnlLogin.Dock = DockStyle.Left;
            pnlLogin.Padding = new Padding(42, 28, 42, 28);
            pnlLogin.Size = new Size(448, 460);

            pictureBox1.Image = Properties.Resources.LOGO;
            pictureBox1.Location = new Point(54, 34);
            pictureBox1.Size = new Size(338, 106);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabStop = false;

            lblWelcome.Text = "Witaj w CUW SISMED";
            lblWelcome.Font = SismedTheme.Font(18f, FontStyle.Bold);
            lblWelcome.ForeColor = SismedTheme.Navy;
            lblWelcome.Location = new Point(54, 158);
            lblWelcome.Size = new Size(338, 34);

            lblHint.Text = "Zaloguj się do panelu obsługi pacjenta";
            lblHint.Font = SismedTheme.Font(9.5f);
            lblHint.ForeColor = SismedTheme.Muted;
            lblHint.Location = new Point(54, 194);
            lblHint.Size = new Size(338, 22);

            txtusername.Location = new Point(54, 236);
            txtusername.Size = new Size(338, 40);
            txtusername.PlaceholderText = "Login";
            txtusername.IconLeft = Properties.Resources.avatar;
            txtusername.IconLeftSize = new Size(18, 18);
            txtusername.TabIndex = 1;
            SismedTheme.ApplyTextBox(txtusername);

            txtpassword.Location = new Point(54, 288);
            txtpassword.Size = new Size(338, 40);
            txtpassword.PlaceholderText = "Hasło";
            txtpassword.IconLeft = Properties.Resources._lock;
            txtpassword.IconLeftSize = new Size(18, 18);
            txtpassword.PasswordChar = '●';
            txtpassword.TabIndex = 2;
            txtpassword.UseSystemPasswordChar = true;
            SismedTheme.ApplyTextBox(txtpassword);

            bttnshowpassword.BackColor = Color.Transparent;
            bttnshowpassword.FillColor = Color.Transparent;
            bttnshowpassword.Image = Properties.Resources.open_eye;
            bttnshowpassword.ImageSize = new Size(18, 18);
            bttnshowpassword.Location = new Point(356, 294);
            bttnshowpassword.Size = new Size(30, 28);
            bttnshowpassword.TabIndex = 3;
            bttnshowpassword.Click += bttnshowpassword_Click;

            bttnlogin.Location = new Point(54, 350);
            bttnlogin.Size = new Size(338, 42);
            bttnlogin.Text = "Zaloguj";
            bttnlogin.TabIndex = 4;
            SismedTheme.ApplyPrimaryButton(bttnlogin);
            bttnlogin.Click += bttnlogin_Click;

            pnlLogin.Controls.AddRange(new Control[]
            {
                pictureBox1, lblWelcome, lblHint,
                txtusername, txtpassword, bttnshowpassword,
                bttnlogin
            });

            pnlVisual.BackColor = SismedTheme.Navy;
            pnlVisual.Dock = DockStyle.Fill;
            pnlVisual.Padding = new Padding(34);
            pnlVisual.MouseDown += MoveForm;

            pnlAccent.BackColor = SismedTheme.Magenta;
            pnlAccent.Location = new Point(34, 54);
            pnlAccent.Size = new Size(64, 6);

            lblVisualTitle.Text = "Nowoczesna rejestracja medyczna";
            lblVisualTitle.Font = SismedTheme.Font(20f, FontStyle.Bold);
            lblVisualTitle.ForeColor = Color.White;
            lblVisualTitle.Location = new Point(34, 82);
            lblVisualTitle.Size = new Size(236, 88);

            lblVisualSub.Text = "Pacjenci, wizyty, personel i dokumenty w jednym uporządkowanym panelu.";
            lblVisualSub.Font = SismedTheme.Font(10f);
            lblVisualSub.ForeColor = SismedTheme.SidebarMuted;
            lblVisualSub.Location = new Point(34, 184);
            lblVisualSub.Size = new Size(236, 76);

            lblVisualFooter.Text = "CUW SISMED";
            lblVisualFooter.Font = SismedTheme.Font(11f, FontStyle.Bold);
            lblVisualFooter.ForeColor = SismedTheme.MagentaSoft;
            lblVisualFooter.Location = new Point(34, 382);
            lblVisualFooter.Size = new Size(236, 28);

            pnlVisual.Controls.AddRange(new Control[]
            {
                guna2ControlBox1, pnlAccent, lblVisualTitle, lblVisualSub, lblVisualFooter
            });

            Controls.Add(pnlVisual);
            Controls.Add(pnlLogin);
            MouseDown += MoveForm;
            ((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
            pnlLogin.ResumeLayout(false);
            pnlVisual.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Guna2ControlBox guna2ControlBox1;
        private Guna2Elipse guna2Elipse1;
        private Guna2Elipse guna2Elipse2;
        private Panel pnlLogin;
        private Panel pnlVisual;
        private Panel pnlAccent;
        private Label lblWelcome;
        private Label lblHint;
        private Label lblVisualTitle;
        private Label lblVisualSub;
        private Label lblVisualFooter;
        private Guna2TextBox txtusername;
        private Guna2Button bttnlogin;
        private PictureBox pictureBox1;
        private Guna2TextBox txtpassword;
        private Guna2Button bttnshowpassword;
    }
}
