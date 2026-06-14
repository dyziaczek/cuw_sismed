using System;
using System.Drawing;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    public partial class login_page
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(login_page));
            this.btnCloseLogin = new System.Windows.Forms.Label();
            this.pnlLogin = new System.Windows.Forms.Panel();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlUsernameField = new VS_CUWSISMED.LoginInputPanel();
            this.picUsernameIcon = new System.Windows.Forms.PictureBox();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.pnlPasswordField = new VS_CUWSISMED.LoginInputPanel();
            this.picPasswordIcon = new System.Windows.Forms.PictureBox();
            this.txtpassword = new System.Windows.Forms.TextBox();
            this.bttnshowpassword = new System.Windows.Forms.Button();
            this.bttnlogin = new VS_CUWSISMED.LoginPrimaryButton();
            this.pnlVisual = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.lblVisualTitle = new System.Windows.Forms.Label();
            this.lblVisualSub = new System.Windows.Forms.Label();
            this.lblVisualFooter = new System.Windows.Forms.Label();
            this.pnlLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlUsernameField.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUsernameIcon)).BeginInit();
            this.pnlPasswordField.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPasswordIcon)).BeginInit();
            this.pnlVisual.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCloseLogin
            // 
            this.btnCloseLogin.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCloseLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(24)))), ((int)(((byte)(70)))));
            this.btnCloseLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCloseLogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCloseLogin.ForeColor = System.Drawing.Color.White;
            this.btnCloseLogin.Location = new System.Drawing.Point(772, 14);
            this.btnCloseLogin.Name = "btnCloseLogin";
            this.btnCloseLogin.Size = new System.Drawing.Size(30, 30);
            this.btnCloseLogin.TabIndex = 0;
            this.btnCloseLogin.Text = "X";
            this.btnCloseLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlLogin
            // 
            this.pnlLogin.BackColor = System.Drawing.Color.White;
            this.pnlLogin.Controls.Add(this.lblHint);
            this.pnlLogin.Controls.Add(this.lblWelcome);
            this.pnlLogin.Controls.Add(this.pictureBox1);
            this.pnlLogin.Controls.Add(this.pnlUsernameField);
            this.pnlLogin.Controls.Add(this.pnlPasswordField);
            this.pnlLogin.Controls.Add(this.bttnlogin);
            this.pnlLogin.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLogin.Location = new System.Drawing.Point(0, 0);
            this.pnlLogin.Name = "pnlLogin";
            this.pnlLogin.Padding = new System.Windows.Forms.Padding(42, 28, 42, 28);
            this.pnlLogin.Size = new System.Drawing.Size(448, 460);
            this.pnlLogin.TabIndex = 1;
            // 
            // lblHint
            // 
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(104)))), ((int)(((byte)(128)))));
            this.lblHint.Location = new System.Drawing.Point(57, 233);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(338, 22);
            this.lblHint.TabIndex = 2;
            this.lblHint.Text = "Zaloguj się do panelu obsługi pacjenta";
            // 
            // lblWelcome
            // 
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(24)))), ((int)(((byte)(70)))));
            this.lblWelcome.Location = new System.Drawing.Point(54, 200);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(338, 34);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "Witaj w CUW SISMED";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::VS_CUWSISMED.Properties.Resources.LOGO;
            this.pictureBox1.Location = new System.Drawing.Point(-60, -40);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(560, 295);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pnlUsernameField
            // 
            this.pnlUsernameField.BackColor = System.Drawing.Color.White;
            this.pnlUsernameField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(190)))), ((int)(((byte)(215)))));
            this.pnlUsernameField.BorderThickness = 1.8F;
            this.pnlUsernameField.Controls.Add(this.picUsernameIcon);
            this.pnlUsernameField.Controls.Add(this.txtusername);
            this.pnlUsernameField.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(0)))), ((int)(((byte)(148)))));
            this.pnlUsernameField.Location = new System.Drawing.Point(54, 266);
            this.pnlUsernameField.Name = "pnlUsernameField";
            this.pnlUsernameField.Radius = 18;
            this.pnlUsernameField.Size = new System.Drawing.Size(338, 46);
            this.pnlUsernameField.TabIndex = 3;
            // 
            // picUsernameIcon
            // 
            this.picUsernameIcon.BackColor = System.Drawing.Color.Transparent;
            this.picUsernameIcon.Image = global::VS_CUWSISMED.Properties.Resources.avatar;
            this.picUsernameIcon.Location = new System.Drawing.Point(16, 11);
            this.picUsernameIcon.Name = "picUsernameIcon";
            this.picUsernameIcon.Size = new System.Drawing.Size(24, 24);
            this.picUsernameIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUsernameIcon.TabIndex = 0;
            this.picUsernameIcon.TabStop = false;
            // 
            // txtusername
            // 
            this.txtusername.BackColor = System.Drawing.Color.White;
            this.txtusername.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtusername.Location = new System.Drawing.Point(52, 13);
            this.txtusername.Name = "txtusername";
            this.txtusername.Size = new System.Drawing.Size(268, 16);
            this.txtusername.TabIndex = 1;
            // 
            // pnlPasswordField
            // 
            this.pnlPasswordField.BackColor = System.Drawing.Color.White;
            this.pnlPasswordField.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(190)))), ((int)(((byte)(215)))));
            this.pnlPasswordField.BorderThickness = 1.8F;
            this.pnlPasswordField.Controls.Add(this.picPasswordIcon);
            this.pnlPasswordField.Controls.Add(this.txtpassword);
            this.pnlPasswordField.Controls.Add(this.bttnshowpassword);
            this.pnlPasswordField.FocusedBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(0)))), ((int)(((byte)(148)))));
            this.pnlPasswordField.Location = new System.Drawing.Point(54, 322);
            this.pnlPasswordField.Name = "pnlPasswordField";
            this.pnlPasswordField.Radius = 18;
            this.pnlPasswordField.Size = new System.Drawing.Size(338, 46);
            this.pnlPasswordField.TabIndex = 4;
            // 
            // picPasswordIcon
            // 
            this.picPasswordIcon.BackColor = System.Drawing.Color.Transparent;
            this.picPasswordIcon.Image = global::VS_CUWSISMED.Properties.Resources._lock;
            this.picPasswordIcon.Location = new System.Drawing.Point(16, 11);
            this.picPasswordIcon.Name = "picPasswordIcon";
            this.picPasswordIcon.Size = new System.Drawing.Size(24, 24);
            this.picPasswordIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPasswordIcon.TabIndex = 0;
            this.picPasswordIcon.TabStop = false;
            // 
            // txtpassword
            // 
            this.txtpassword.BackColor = System.Drawing.Color.White;
            this.txtpassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtpassword.Location = new System.Drawing.Point(52, 13);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.PasswordChar = '●';
            this.txtpassword.Size = new System.Drawing.Size(220, 16);
            this.txtpassword.TabIndex = 2;
            this.txtpassword.UseSystemPasswordChar = true;
            // 
            // bttnshowpassword
            // 
            this.bttnshowpassword.BackColor = System.Drawing.Color.Transparent;
            this.bttnshowpassword.FlatAppearance.BorderSize = 0;
            this.bttnshowpassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttnshowpassword.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.bttnshowpassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(24)))), ((int)(((byte)(70)))));
            this.bttnshowpassword.Location = new System.Drawing.Point(282, 9);
            this.bttnshowpassword.Name = "bttnshowpassword";
            this.bttnshowpassword.Size = new System.Drawing.Size(46, 28);
            this.bttnshowpassword.TabIndex = 3;
            this.bttnshowpassword.Text = "Pokaż";
            this.bttnshowpassword.UseVisualStyleBackColor = false;
            // 
            // bttnlogin
            // 
            this.bttnlogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bttnlogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttnlogin.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.bttnlogin.Location = new System.Drawing.Point(54, 386);
            this.bttnlogin.Name = "bttnlogin";
            this.bttnlogin.Radius = 18;
            this.bttnlogin.Size = new System.Drawing.Size(338, 46);
            this.bttnlogin.TabIndex = 4;
            this.bttnlogin.Text = "Zaloguj";
            this.bttnlogin.UseVisualStyleBackColor = false;
            // 
            // pnlVisual
            // 
            this.pnlVisual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(24)))), ((int)(((byte)(70)))));
            this.pnlVisual.Controls.Add(this.label1);
            this.pnlVisual.Controls.Add(this.pnlAccent);
            this.pnlVisual.Controls.Add(this.lblVisualTitle);
            this.pnlVisual.Controls.Add(this.lblVisualSub);
            this.pnlVisual.Controls.Add(this.lblVisualFooter);
            this.pnlVisual.Controls.Add(this.btnCloseLogin);
            this.pnlVisual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVisual.Location = new System.Drawing.Point(448, 0);
            this.pnlVisual.Name = "pnlVisual";
            this.pnlVisual.Padding = new System.Windows.Forms.Padding(34);
            this.pnlVisual.Size = new System.Drawing.Size(312, 460);
            this.pnlVisual.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(194)))), ((int)(((byte)(228)))));
            this.label1.Location = new System.Drawing.Point(15, 410);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 50);
            this.label1.TabIndex = 4;
            this.label1.Text = "W razie problemów skontaktuj         się z administratorem.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(0)))), ((int)(((byte)(148)))));
            this.pnlAccent.Location = new System.Drawing.Point(34, 54);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(64, 6);
            this.pnlAccent.TabIndex = 0;
            // 
            // lblVisualTitle
            // 
            this.lblVisualTitle.Font = new System.Drawing.Font("Segoe UI", 21F, System.Drawing.FontStyle.Bold);
            this.lblVisualTitle.ForeColor = System.Drawing.Color.White;
            this.lblVisualTitle.Location = new System.Drawing.Point(34, 82);
            this.lblVisualTitle.Name = "lblVisualTitle";
            this.lblVisualTitle.Size = new System.Drawing.Size(264, 82);
            this.lblVisualTitle.TabIndex = 1;
            this.lblVisualTitle.Text = "Centrum Umawiania Wizyt";
            // 
            // lblVisualSub
            // 
            this.lblVisualSub.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblVisualSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(194)))), ((int)(((byte)(228)))));
            this.lblVisualSub.Location = new System.Drawing.Point(34, 184);
            this.lblVisualSub.Name = "lblVisualSub";
            this.lblVisualSub.Size = new System.Drawing.Size(236, 116);
            this.lblVisualSub.TabIndex = 2;
            this.lblVisualSub.Text = "Zaloguj się do systemu umawiania wizyt!  ";
            this.lblVisualSub.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblVisualFooter
            // 
            this.lblVisualFooter.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblVisualFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(229)))), ((int)(((byte)(242)))));
            this.lblVisualFooter.Location = new System.Drawing.Point(15, 386);
            this.lblVisualFooter.Name = "lblVisualFooter";
            this.lblVisualFooter.Size = new System.Drawing.Size(236, 28);
            this.lblVisualFooter.TabIndex = 3;
            this.lblVisualFooter.Text = "CUW SISMED";
            // 
            // login_page
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(760, 460);
            this.Controls.Add(this.pnlVisual);
            this.Controls.Add(this.pnlLogin);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(760, 460);
            this.Name = "login_page";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CUW SISMED - logowanie";
            this.pnlLogin.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlUsernameField.ResumeLayout(false);
            this.pnlUsernameField.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUsernameIcon)).EndInit();
            this.pnlPasswordField.ResumeLayout(false);
            this.pnlPasswordField.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPasswordIcon)).EndInit();
            this.pnlVisual.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Label btnCloseLogin;
        private Panel pnlLogin;
        private Panel pnlVisual;
        private Panel pnlAccent;
        private Label lblWelcome;
        private Label lblHint;
        private Label lblVisualTitle;
        private Label lblVisualSub;
        private Label lblVisualFooter;
        private LoginInputPanel pnlUsernameField;
        private LoginInputPanel pnlPasswordField;
        private PictureBox picUsernameIcon;
        private PictureBox picPasswordIcon;
        private TextBox txtusername;
        private PictureBox pictureBox1;
        private TextBox txtpassword;
        private Button bttnshowpassword;
        private Label label1;
        private LoginPrimaryButton bttnlogin;
    }
}
