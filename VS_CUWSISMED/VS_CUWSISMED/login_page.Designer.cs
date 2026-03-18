using System;

namespace VS_CUWSISMED
{
    partial class login_page
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(login_page));
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.txtusername = new Guna.UI2.WinForms.Guna2TextBox();
            this.bttnlogin = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtpassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.BorderColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.ControlBoxStyle = Guna.UI2.WinForms.Enums.ControlBoxStyle.Custom;
            this.guna2ControlBox1.CustomIconSize = 18F;
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.Font = new System.Drawing.Font("Figtree", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2ControlBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(554, 22);
            this.guna2ControlBox1.MaximumSize = new System.Drawing.Size(50, 50);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.PressedColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.Size = new System.Drawing.Size(50, 50);
            this.guna2ControlBox1.TabIndex = 0;
            // 
            // txtusername
            // 
            this.txtusername.BackColor = System.Drawing.Color.White;
            this.txtusername.BorderColor = System.Drawing.Color.Navy;
            this.txtusername.BorderThickness = 3;
            this.txtusername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtusername.DefaultText = "";
            this.txtusername.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtusername.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtusername.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtusername.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtusername.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtusername.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtusername.ForeColor = System.Drawing.Color.DimGray;
            this.txtusername.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtusername.IconLeft = global::VS_CUWSISMED.Properties.Resources.avatar;
            this.txtusername.IconLeftSize = new System.Drawing.Size(18, 18);
            this.txtusername.IconRightSize = new System.Drawing.Size(18, 18);
            this.txtusername.Location = new System.Drawing.Point(165, 205);
            this.txtusername.Name = "txtusername";
            this.txtusername.PasswordChar = '\0';
            this.txtusername.PlaceholderForeColor = System.Drawing.Color.Transparent;
            this.txtusername.PlaceholderText = "";
            this.txtusername.SelectedText = "";
            this.txtusername.Size = new System.Drawing.Size(297, 36);
            this.txtusername.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtusername.TabIndex = 1;
            // 
            // bttnlogin
            // 
            this.bttnlogin.AutoRoundedCorners = true;
            this.bttnlogin.BackColor = System.Drawing.Color.Transparent;
            this.bttnlogin.BorderColor = System.Drawing.Color.Transparent;
            this.bttnlogin.BorderRadius = 14;
            this.bttnlogin.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.bttnlogin.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.bttnlogin.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.bttnlogin.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.bttnlogin.FillColor = System.Drawing.Color.LightGray;
            this.bttnlogin.Font = new System.Drawing.Font("Figtree", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bttnlogin.ForeColor = System.Drawing.Color.Navy;
            this.bttnlogin.Location = new System.Drawing.Point(248, 289);
            this.bttnlogin.Name = "bttnlogin";
            this.bttnlogin.PressedColor = System.Drawing.Color.Transparent;
            this.bttnlogin.Size = new System.Drawing.Size(133, 30);
            this.bttnlogin.TabIndex = 2;
            this.bttnlogin.Text = "Zaloguj";
            this.bttnlogin.Click += new System.EventHandler(this.bttnlogin_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::VS_CUWSISMED.Properties.Resources._222;
            this.pictureBox1.Location = new System.Drawing.Point(82, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(468, 168);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // txtpassword
            // 
            this.txtpassword.BackColor = System.Drawing.Color.White;
            this.txtpassword.BorderColor = System.Drawing.Color.Navy;
            this.txtpassword.BorderThickness = 3;
            this.txtpassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtpassword.DefaultText = "";
            this.txtpassword.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtpassword.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtpassword.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtpassword.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtpassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtpassword.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpassword.ForeColor = System.Drawing.Color.DimGray;
            this.txtpassword.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtpassword.IconLeft = global::VS_CUWSISMED.Properties.Resources._lock;
            this.txtpassword.IconLeftSize = new System.Drawing.Size(18, 18);
            this.txtpassword.IconRight = global::VS_CUWSISMED.Properties.Resources.visible_opened_eye_interface_option;
            this.txtpassword.IconRightSize = new System.Drawing.Size(18, 18);
            this.txtpassword.Location = new System.Drawing.Point(165, 247);
            this.txtpassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.PasswordChar = '●';
            this.txtpassword.PlaceholderForeColor = System.Drawing.Color.Transparent;
            this.txtpassword.PlaceholderText = "";
            this.txtpassword.SelectedText = "";
            this.txtpassword.Size = new System.Drawing.Size(297, 36);
            this.txtpassword.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtpassword.TabIndex = 5;
            this.txtpassword.UseSystemPasswordChar = true;
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 150;
            this.guna2Elipse1.TargetControl = this;
            // 
            // login_page
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::VS_CUWSISMED.Properties.Resources.LOGINN;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(616, 410);
            this.Controls.Add(this.txtpassword);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.bttnlogin);
            this.Controls.Add(this.txtusername);
            this.Controls.Add(this.guna2ControlBox1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "login_page";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.login_page_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2TextBox txtusername;
        private Guna.UI2.WinForms.Guna2Button bttnlogin;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2TextBox txtpassword;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
    }
}

