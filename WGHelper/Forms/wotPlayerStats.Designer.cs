namespace WGHelper.Forms
{
    partial class wotPlayerStats
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
            this.button1 = new System.Windows.Forms.Button();
            this.labelNickname = new System.Windows.Forms.Label();
            this.labelClientLanguage = new System.Windows.Forms.Label();
            this.labelCreatedAt = new System.Windows.Forms.Label();
            this.labelGlobalRating = new System.Windows.Forms.Label();
            this.labelLogoutAt = new System.Windows.Forms.Label();
            this.labelUpdatedAt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 416);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelNickname
            // 
            this.labelNickname.AutoSize = true;
            this.labelNickname.Location = new System.Drawing.Point(12, 9);
            this.labelNickname.Name = "labelNickname";
            this.labelNickname.Size = new System.Drawing.Size(77, 13);
            this.labelNickname.TabIndex = 1;
            this.labelNickname.Text = "labelNickname";
            // 
            // labelClientLanguage
            // 
            this.labelClientLanguage.AutoSize = true;
            this.labelClientLanguage.Location = new System.Drawing.Point(12, 22);
            this.labelClientLanguage.Name = "labelClientLanguage";
            this.labelClientLanguage.Size = new System.Drawing.Size(103, 13);
            this.labelClientLanguage.TabIndex = 2;
            this.labelClientLanguage.Text = "labelClientLanguage";
            // 
            // labelCreatedAt
            // 
            this.labelCreatedAt.AutoSize = true;
            this.labelCreatedAt.Location = new System.Drawing.Point(12, 35);
            this.labelCreatedAt.Name = "labelCreatedAt";
            this.labelCreatedAt.Size = new System.Drawing.Size(76, 13);
            this.labelCreatedAt.TabIndex = 3;
            this.labelCreatedAt.Text = "labelCreatedAt";
            // 
            // labelGlobalRating
            // 
            this.labelGlobalRating.AutoSize = true;
            this.labelGlobalRating.Location = new System.Drawing.Point(12, 48);
            this.labelGlobalRating.Name = "labelGlobalRating";
            this.labelGlobalRating.Size = new System.Drawing.Size(90, 13);
            this.labelGlobalRating.TabIndex = 4;
            this.labelGlobalRating.Text = "labelGlobalRating";
            // 
            // labelLogoutAt
            // 
            this.labelLogoutAt.AutoSize = true;
            this.labelLogoutAt.Location = new System.Drawing.Point(12, 61);
            this.labelLogoutAt.Name = "labelLogoutAt";
            this.labelLogoutAt.Size = new System.Drawing.Size(72, 13);
            this.labelLogoutAt.TabIndex = 5;
            this.labelLogoutAt.Text = "labelLogoutAt";
            // 
            // labelUpdatedAt
            // 
            this.labelUpdatedAt.AutoSize = true;
            this.labelUpdatedAt.Location = new System.Drawing.Point(12, 74);
            this.labelUpdatedAt.Name = "labelUpdatedAt";
            this.labelUpdatedAt.Size = new System.Drawing.Size(80, 13);
            this.labelUpdatedAt.TabIndex = 6;
            this.labelUpdatedAt.Text = "labelUpdatedAt";
            // 
            // wotPlayerStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 451);
            this.Controls.Add(this.labelUpdatedAt);
            this.Controls.Add(this.labelLogoutAt);
            this.Controls.Add(this.labelGlobalRating);
            this.Controls.Add(this.labelCreatedAt);
            this.Controls.Add(this.labelClientLanguage);
            this.Controls.Add(this.labelNickname);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "wotPlayerStats";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "wotPlayerStats";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelNickname;
        private System.Windows.Forms.Label labelClientLanguage;
        private System.Windows.Forms.Label labelCreatedAt;
        private System.Windows.Forms.Label labelGlobalRating;
        private System.Windows.Forms.Label labelLogoutAt;
        private System.Windows.Forms.Label labelUpdatedAt;
    }
}