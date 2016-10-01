namespace WGHelper
{
    partial class FormSettings
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
            this.buttonBack = new System.Windows.Forms.Button();
            this.labelWorldofTanks = new System.Windows.Forms.Label();
            this.WoTPath_textBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonSetWoTFolder = new System.Windows.Forms.Button();
            this.label_clients_paths = new System.Windows.Forms.Label();
            this.checkBox_autoUpdateServersOnline = new System.Windows.Forms.CheckBox();
            this.checkBox_autoping = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.Location = new System.Drawing.Point(12, 184);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(75, 23);
            this.buttonBack.TabIndex = 0;
            this.buttonBack.Text = "buttonBack";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // labelWorldofTanks
            // 
            this.labelWorldofTanks.AutoSize = true;
            this.labelWorldofTanks.Location = new System.Drawing.Point(12, 35);
            this.labelWorldofTanks.Name = "labelWorldofTanks";
            this.labelWorldofTanks.Size = new System.Drawing.Size(80, 13);
            this.labelWorldofTanks.TabIndex = 1;
            this.labelWorldofTanks.Text = "World of Tanks";
            // 
            // WoTPath_textBox
            // 
            this.WoTPath_textBox.Enabled = false;
            this.WoTPath_textBox.Location = new System.Drawing.Point(15, 61);
            this.WoTPath_textBox.Name = "WoTPath_textBox";
            this.WoTPath_textBox.Size = new System.Drawing.Size(293, 20);
            this.WoTPath_textBox.TabIndex = 2;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.SelectedPath = "C:\\Users\\M0stwan1eD\\Desktop";
            // 
            // buttonSetWoTFolder
            // 
            this.buttonSetWoTFolder.Location = new System.Drawing.Point(314, 59);
            this.buttonSetWoTFolder.Name = "buttonSetWoTFolder";
            this.buttonSetWoTFolder.Size = new System.Drawing.Size(120, 23);
            this.buttonSetWoTFolder.TabIndex = 3;
            this.buttonSetWoTFolder.Text = "buttonSetWoTFolder";
            this.buttonSetWoTFolder.UseVisualStyleBackColor = true;
            this.buttonSetWoTFolder.Click += new System.EventHandler(this.buttonSetWoTFolder_Click);
            // 
            // label_clients_paths
            // 
            this.label_clients_paths.AutoSize = true;
            this.label_clients_paths.Location = new System.Drawing.Point(12, 9);
            this.label_clients_paths.Name = "label_clients_paths";
            this.label_clients_paths.Size = new System.Drawing.Size(108, 13);
            this.label_clients_paths.TabIndex = 4;
            this.label_clients_paths.Text = "Paths to game clients";
            // 
            // checkBox_autoUpdateServersOnline
            // 
            this.checkBox_autoUpdateServersOnline.AutoSize = true;
            this.checkBox_autoUpdateServersOnline.Location = new System.Drawing.Point(15, 87);
            this.checkBox_autoUpdateServersOnline.Name = "checkBox_autoUpdateServersOnline";
            this.checkBox_autoUpdateServersOnline.Size = new System.Drawing.Size(192, 17);
            this.checkBox_autoUpdateServersOnline.TabIndex = 5;
            this.checkBox_autoUpdateServersOnline.Text = "Automatically update servers online";
            this.checkBox_autoUpdateServersOnline.UseVisualStyleBackColor = true;
            this.checkBox_autoUpdateServersOnline.CheckedChanged += new System.EventHandler(this.checkBox_autoUpdateServersOnline_CheckedChanged);
            // 
            // checkBox_autoping
            // 
            this.checkBox_autoping.AutoSize = true;
            this.checkBox_autoping.Location = new System.Drawing.Point(15, 110);
            this.checkBox_autoping.Name = "checkBox_autoping";
            this.checkBox_autoping.Size = new System.Drawing.Size(158, 17);
            this.checkBox_autoping.TabIndex = 6;
            this.checkBox_autoping.Text = "Automatically check latency";
            this.checkBox_autoping.UseVisualStyleBackColor = true;
            this.checkBox_autoping.CheckedChanged += new System.EventHandler(this.checkBox_autoping_CheckedChanged);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 219);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox_autoping);
            this.Controls.Add(this.checkBox_autoUpdateServersOnline);
            this.Controls.Add(this.label_clients_paths);
            this.Controls.Add(this.buttonSetWoTFolder);
            this.Controls.Add(this.WoTPath_textBox);
            this.Controls.Add(this.labelWorldofTanks);
            this.Controls.Add(this.buttonBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Label labelWorldofTanks;
        private System.Windows.Forms.TextBox WoTPath_textBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button buttonSetWoTFolder;
        private System.Windows.Forms.Label label_clients_paths;
        private System.Windows.Forms.CheckBox checkBox_autoUpdateServersOnline;
        private System.Windows.Forms.CheckBox checkBox_autoping;
    }
}