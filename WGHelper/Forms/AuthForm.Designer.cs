namespace WGHelper.Forms
{
    partial class AuthForm
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
            this.button_Back = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // button_Back
            // 
            this.button_Back.Location = new System.Drawing.Point(12, 427);
            this.button_Back.Name = "button_Back";
            this.button_Back.Size = new System.Drawing.Size(92, 23);
            this.button_Back.TabIndex = 0;
            this.button_Back.Text = "button_Back";
            this.button_Back.UseVisualStyleBackColor = true;
            this.button_Back.Click += new System.EventHandler(this.button_Back_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(12, 12);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(984, 409);
            this.webBrowser1.TabIndex = 1;
            this.webBrowser1.Url = new System.Uri("https://api.worldoftanks.ru/wot/auth/login/?application_id=146bc6b8d619f5030ed02c" +
        "db5ce759b4&display=popup", System.UriKind.Absolute);
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // AuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 462);
            this.ControlBox = false;
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.button_Back);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AuthForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AuthForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Back;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}