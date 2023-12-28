namespace sistem_de_gestionare_a_bibliotecii
{
    partial class Biblioteca
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.loginButton = new System.Windows.Forms.Button();
            this.signUpButton = new System.Windows.Forms.Button();
            this.bibliotecaLoginPanel = new System.Windows.Forms.Panel();
            this.bibliotecaPanelTextBoxTelefon = new System.Windows.Forms.TextBox();
            this.bibliotecaPanelLabelTelefon = new System.Windows.Forms.Label();
            this.bibliotecaPanelTextBoxPrenume = new System.Windows.Forms.TextBox();
            this.bibliotecaPanelLabelPrenume = new System.Windows.Forms.Label();
            this.bibliotecaPanelButtonBack = new System.Windows.Forms.Button();
            this.bibliotecaPanelButtonLogin = new System.Windows.Forms.Button();
            this.bibliotecaPanelTextBoxParola = new System.Windows.Forms.TextBox();
            this.bibliotecaPanelLabelParola = new System.Windows.Forms.Label();
            this.bibliotecaPanelTextBoxEmail = new System.Windows.Forms.TextBox();
            this.bibliotecaPanelLabelEmail = new System.Windows.Forms.Label();
            this.bibliotecaPanelTextBoxNume = new System.Windows.Forms.TextBox();
            this.bibliotecaPanelLabelNume = new System.Windows.Forms.Label();
            this.bibliotecaLoginPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(12, 48);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(193, 80);
            this.loginButton.TabIndex = 1;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // signUpButton
            // 
            this.signUpButton.Location = new System.Drawing.Point(227, 48);
            this.signUpButton.Name = "signUpButton";
            this.signUpButton.Size = new System.Drawing.Size(193, 80);
            this.signUpButton.TabIndex = 2;
            this.signUpButton.Text = "Sign Up";
            this.signUpButton.UseVisualStyleBackColor = true;
            this.signUpButton.Click += new System.EventHandler(this.SignUpButton_Click);
            // 
            // bibliotecaLoginPanel
            // 
            this.bibliotecaLoginPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.bibliotecaLoginPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelTextBoxTelefon);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelLabelTelefon);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelTextBoxPrenume);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelLabelEmail);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelTextBoxEmail);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelLabelPrenume);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelButtonBack);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelButtonLogin);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelTextBoxParola);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelLabelParola);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelTextBoxNume);
            this.bibliotecaLoginPanel.Controls.Add(this.bibliotecaPanelLabelNume);
            this.bibliotecaLoginPanel.Location = new System.Drawing.Point(26, 144);
            this.bibliotecaLoginPanel.Name = "bibliotecaLoginPanel";
            this.bibliotecaLoginPanel.Size = new System.Drawing.Size(499, 324);
            this.bibliotecaLoginPanel.TabIndex = 3;
            this.bibliotecaLoginPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.bibliotecaLoginPanel_Paint);
            // 
            // bibliotecaPanelTextBoxTelefon
            // 
            this.bibliotecaPanelTextBoxTelefon.Location = new System.Drawing.Point(177, 178);
            this.bibliotecaPanelTextBoxTelefon.Name = "bibliotecaPanelTextBoxTelefon";
            this.bibliotecaPanelTextBoxTelefon.Size = new System.Drawing.Size(210, 22);
            this.bibliotecaPanelTextBoxTelefon.TabIndex = 11;
            // 
            // bibliotecaPanelLabelTelefon
            // 
            this.bibliotecaPanelLabelTelefon.AutoSize = true;
            this.bibliotecaPanelLabelTelefon.Location = new System.Drawing.Point(67, 181);
            this.bibliotecaPanelLabelTelefon.Name = "bibliotecaPanelLabelTelefon";
            this.bibliotecaPanelLabelTelefon.Size = new System.Drawing.Size(70, 17);
            this.bibliotecaPanelLabelTelefon.TabIndex = 10;
            this.bibliotecaPanelLabelTelefon.Text = "Nr telefon";
            // 
            // bibliotecaPanelTextBoxPrenume
            // 
            this.bibliotecaPanelTextBoxPrenume.Location = new System.Drawing.Point(177, 50);
            this.bibliotecaPanelTextBoxPrenume.Name = "bibliotecaPanelTextBoxPrenume";
            this.bibliotecaPanelTextBoxPrenume.Size = new System.Drawing.Size(210, 22);
            this.bibliotecaPanelTextBoxPrenume.TabIndex = 9;
            // 
            // bibliotecaPanelLabelPrenume
            // 
            this.bibliotecaPanelLabelPrenume.AutoSize = true;
            this.bibliotecaPanelLabelPrenume.Location = new System.Drawing.Point(67, 53);
            this.bibliotecaPanelLabelPrenume.Name = "bibliotecaPanelLabelPrenume";
            this.bibliotecaPanelLabelPrenume.Size = new System.Drawing.Size(65, 17);
            this.bibliotecaPanelLabelPrenume.TabIndex = 8;
            this.bibliotecaPanelLabelPrenume.Text = "Prenume";
            // 
            // bibliotecaPanelButtonBack
            // 
            this.bibliotecaPanelButtonBack.Location = new System.Drawing.Point(317, 222);
            this.bibliotecaPanelButtonBack.Name = "bibliotecaPanelButtonBack";
            this.bibliotecaPanelButtonBack.Size = new System.Drawing.Size(115, 43);
            this.bibliotecaPanelButtonBack.TabIndex = 7;
            this.bibliotecaPanelButtonBack.Text = "Back";
            this.bibliotecaPanelButtonBack.UseVisualStyleBackColor = true;
            this.bibliotecaPanelButtonBack.Click += new System.EventHandler(this.bibliotecaPanelButtonBack_Click);
            // 
            // bibliotecaPanelButtonLogin
            // 
            this.bibliotecaPanelButtonLogin.Location = new System.Drawing.Point(176, 222);
            this.bibliotecaPanelButtonLogin.Name = "bibliotecaPanelButtonLogin";
            this.bibliotecaPanelButtonLogin.Size = new System.Drawing.Size(115, 43);
            this.bibliotecaPanelButtonLogin.TabIndex = 6;
            this.bibliotecaPanelButtonLogin.Text = "Login";
            this.bibliotecaPanelButtonLogin.UseVisualStyleBackColor = true;
            this.bibliotecaPanelButtonLogin.Click += new System.EventHandler(this.bibliotecaPanelButtonLogin_Click);
            // 
            // bibliotecaPanelTextBoxParola
            // 
            this.bibliotecaPanelTextBoxParola.Location = new System.Drawing.Point(177, 139);
            this.bibliotecaPanelTextBoxParola.Name = "bibliotecaPanelTextBoxParola";
            this.bibliotecaPanelTextBoxParola.Size = new System.Drawing.Size(210, 22);
            this.bibliotecaPanelTextBoxParola.TabIndex = 5;
            // 
            // bibliotecaPanelLabelParola
            // 
            this.bibliotecaPanelLabelParola.AutoSize = true;
            this.bibliotecaPanelLabelParola.Location = new System.Drawing.Point(67, 142);
            this.bibliotecaPanelLabelParola.Name = "bibliotecaPanelLabelParola";
            this.bibliotecaPanelLabelParola.Size = new System.Drawing.Size(49, 17);
            this.bibliotecaPanelLabelParola.TabIndex = 4;
            this.bibliotecaPanelLabelParola.Text = "Parola";
            // 
            // bibliotecaPanelTextBoxEmail
            // 
            this.bibliotecaPanelTextBoxEmail.Location = new System.Drawing.Point(177, 96);
            this.bibliotecaPanelTextBoxEmail.Name = "bibliotecaPanelTextBoxEmail";
            this.bibliotecaPanelTextBoxEmail.Size = new System.Drawing.Size(210, 22);
            this.bibliotecaPanelTextBoxEmail.TabIndex = 3;
            // 
            // bibliotecaPanelLabelEmail
            // 
            this.bibliotecaPanelLabelEmail.AutoSize = true;
            this.bibliotecaPanelLabelEmail.Location = new System.Drawing.Point(67, 99);
            this.bibliotecaPanelLabelEmail.Name = "bibliotecaPanelLabelEmail";
            this.bibliotecaPanelLabelEmail.Size = new System.Drawing.Size(42, 17);
            this.bibliotecaPanelLabelEmail.TabIndex = 2;
            this.bibliotecaPanelLabelEmail.Text = "Email";
            // 
            // bibliotecaPanelTextBoxNume
            // 
            this.bibliotecaPanelTextBoxNume.Location = new System.Drawing.Point(177, 11);
            this.bibliotecaPanelTextBoxNume.Name = "bibliotecaPanelTextBoxNume";
            this.bibliotecaPanelTextBoxNume.Size = new System.Drawing.Size(210, 22);
            this.bibliotecaPanelTextBoxNume.TabIndex = 1;
            // 
            // bibliotecaPanelLabelNume
            // 
            this.bibliotecaPanelLabelNume.AutoSize = true;
            this.bibliotecaPanelLabelNume.Location = new System.Drawing.Point(67, 14);
            this.bibliotecaPanelLabelNume.Name = "bibliotecaPanelLabelNume";
            this.bibliotecaPanelLabelNume.Size = new System.Drawing.Size(45, 17);
            this.bibliotecaPanelLabelNume.TabIndex = 0;
            this.bibliotecaPanelLabelNume.Text = "Nume";
            // 
            // Biblioteca
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(604, 535);
            this.Controls.Add(this.bibliotecaLoginPanel);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.signUpButton);
            this.MinimumSize = new System.Drawing.Size(600, 300);
            this.Name = "Biblioteca";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Biblioteca";
            this.Load += new System.EventHandler(this.Biblioteca_Load);
            this.bibliotecaLoginPanel.ResumeLayout(false);
            this.bibliotecaLoginPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
#pragma warning disable CS0169 // The field 'Biblioteca.toolStripMenuItem1' is never used
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
#pragma warning restore CS0169 // The field 'Biblioteca.toolStripMenuItem1' is never used
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button signUpButton;
        private System.Windows.Forms.Panel bibliotecaLoginPanel;
        private System.Windows.Forms.Label bibliotecaPanelLabelNume;
        private System.Windows.Forms.Label bibliotecaPanelLabelEmail;
        private System.Windows.Forms.TextBox bibliotecaPanelTextBoxNume;
        private System.Windows.Forms.TextBox bibliotecaPanelTextBoxEmail;
        private System.Windows.Forms.Button bibliotecaPanelButtonLogin;
        private System.Windows.Forms.Button bibliotecaPanelButtonBack;
        private System.Windows.Forms.TextBox bibliotecaPanelTextBoxPrenume;
        private System.Windows.Forms.Label bibliotecaPanelLabelPrenume;
        private System.Windows.Forms.TextBox bibliotecaPanelTextBoxTelefon;
        private System.Windows.Forms.Label bibliotecaPanelLabelTelefon;
        private System.Windows.Forms.TextBox bibliotecaPanelTextBoxParola;
        private System.Windows.Forms.Label bibliotecaPanelLabelParola;
    }
}

