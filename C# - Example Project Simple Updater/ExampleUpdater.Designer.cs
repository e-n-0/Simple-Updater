namespace Simple_Updater
{
    partial class ExampleUpdater
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.Check_Button = new System.Windows.Forms.Button();
            this.NbrLocalFiles_Label = new System.Windows.Forms.Label();
            this.NbrServerFile_Label = new System.Windows.Forms.Label();
            this.NbrFilesToDownload_Label = new System.Windows.Forms.Label();
            this.NbrFilesDeleted_Label = new System.Windows.Forms.Label();
            this.Download_Button = new System.Windows.Forms.Button();
            this.TotalBytesToDownload_Label = new System.Windows.Forms.Label();
            this.TotalBytesDownloaded_Label = new System.Windows.Forms.Label();
            this.ProgressBarDownloading = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // Check_Button
            // 
            this.Check_Button.Location = new System.Drawing.Point(53, 33);
            this.Check_Button.Name = "Check_Button";
            this.Check_Button.Size = new System.Drawing.Size(129, 42);
            this.Check_Button.TabIndex = 0;
            this.Check_Button.Text = "Check files from server and delete old";
            this.Check_Button.UseVisualStyleBackColor = true;
            this.Check_Button.Click += new System.EventHandler(this.Check_Button_Click);
            // 
            // NbrLocalFiles_Label
            // 
            this.NbrLocalFiles_Label.AutoSize = true;
            this.NbrLocalFiles_Label.Location = new System.Drawing.Point(50, 89);
            this.NbrLocalFiles_Label.Name = "NbrLocalFiles_Label";
            this.NbrLocalFiles_Label.Size = new System.Drawing.Size(111, 13);
            this.NbrLocalFiles_Label.TabIndex = 1;
            this.NbrLocalFiles_Label.Text = "Number of local Files :";
            // 
            // NbrServerFile_Label
            // 
            this.NbrServerFile_Label.AutoSize = true;
            this.NbrServerFile_Label.Location = new System.Drawing.Point(50, 116);
            this.NbrServerFile_Label.Name = "NbrServerFile_Label";
            this.NbrServerFile_Label.Size = new System.Drawing.Size(118, 13);
            this.NbrServerFile_Label.TabIndex = 2;
            this.NbrServerFile_Label.Text = "Number of server Files :";
            // 
            // NbrFilesToDownload_Label
            // 
            this.NbrFilesToDownload_Label.AutoSize = true;
            this.NbrFilesToDownload_Label.Location = new System.Drawing.Point(50, 142);
            this.NbrFilesToDownload_Label.Name = "NbrFilesToDownload_Label";
            this.NbrFilesToDownload_Label.Size = new System.Drawing.Size(144, 13);
            this.NbrFilesToDownload_Label.TabIndex = 3;
            this.NbrFilesToDownload_Label.Text = "Number of files to download :";
            // 
            // NbrFilesDeleted_Label
            // 
            this.NbrFilesDeleted_Label.AutoSize = true;
            this.NbrFilesDeleted_Label.Location = new System.Drawing.Point(50, 168);
            this.NbrFilesDeleted_Label.Name = "NbrFilesDeleted_Label";
            this.NbrFilesDeleted_Label.Size = new System.Drawing.Size(121, 13);
            this.NbrFilesDeleted_Label.TabIndex = 4;
            this.NbrFilesDeleted_Label.Text = "Number of files deleted :";
            // 
            // Download_Button
            // 
            this.Download_Button.Location = new System.Drawing.Point(300, 33);
            this.Download_Button.Name = "Download_Button";
            this.Download_Button.Size = new System.Drawing.Size(129, 42);
            this.Download_Button.TabIndex = 5;
            this.Download_Button.Text = "Download";
            this.Download_Button.UseVisualStyleBackColor = true;
            this.Download_Button.Click += new System.EventHandler(this.Download_Button_Click);
            // 
            // TotalBytesToDownload_Label
            // 
            this.TotalBytesToDownload_Label.AutoSize = true;
            this.TotalBytesToDownload_Label.Location = new System.Drawing.Point(297, 89);
            this.TotalBytesToDownload_Label.Name = "TotalBytesToDownload_Label";
            this.TotalBytesToDownload_Label.Size = new System.Drawing.Size(126, 13);
            this.TotalBytesToDownload_Label.TabIndex = 6;
            this.TotalBytesToDownload_Label.Text = "Total bytes to download :";
            // 
            // TotalBytesDownloaded_Label
            // 
            this.TotalBytesDownloaded_Label.AutoSize = true;
            this.TotalBytesDownloaded_Label.Location = new System.Drawing.Point(297, 116);
            this.TotalBytesDownloaded_Label.Name = "TotalBytesDownloaded_Label";
            this.TotalBytesDownloaded_Label.Size = new System.Drawing.Size(126, 13);
            this.TotalBytesDownloaded_Label.TabIndex = 7;
            this.TotalBytesDownloaded_Label.Text = "Total bytes downloaded :";
            // 
            // ProgressBarDownloading
            // 
            this.ProgressBarDownloading.Location = new System.Drawing.Point(300, 158);
            this.ProgressBarDownloading.Name = "ProgressBarDownloading";
            this.ProgressBarDownloading.Size = new System.Drawing.Size(129, 23);
            this.ProgressBarDownloading.TabIndex = 8;
            // 
            // ExampleUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 201);
            this.Controls.Add(this.ProgressBarDownloading);
            this.Controls.Add(this.TotalBytesDownloaded_Label);
            this.Controls.Add(this.TotalBytesToDownload_Label);
            this.Controls.Add(this.Download_Button);
            this.Controls.Add(this.NbrFilesDeleted_Label);
            this.Controls.Add(this.NbrFilesToDownload_Label);
            this.Controls.Add(this.NbrServerFile_Label);
            this.Controls.Add(this.NbrLocalFiles_Label);
            this.Controls.Add(this.Check_Button);
            this.Name = "ExampleUpdater";
            this.Text = "Example Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Check_Button;
        private System.Windows.Forms.Label NbrLocalFiles_Label;
        private System.Windows.Forms.Label NbrServerFile_Label;
        private System.Windows.Forms.Label NbrFilesToDownload_Label;
        private System.Windows.Forms.Label NbrFilesDeleted_Label;
        private System.Windows.Forms.Button Download_Button;
        private System.Windows.Forms.Label TotalBytesToDownload_Label;
        private System.Windows.Forms.Label TotalBytesDownloaded_Label;
        private System.Windows.Forms.ProgressBar ProgressBarDownloading;
    }
}

