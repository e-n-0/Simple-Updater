using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple_Updater_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_Updater
{
    public partial class ExampleUpdater : Form
    {

        // Configuration
        private string installation_path = Path.Combine(Application.StartupPath, "files");
        private string urlServer = "http://localhost";

        // Var
        SimpleUpdater updater;

        public ExampleUpdater()
        {
            InitializeComponent();

            ProgressBarDownloading.Maximum = 100;

            // Initialize the Simple Updater
            updater = new SimpleUpdater(urlServer, installation_path);

            // Initalize Events of the SimpleUpdater
            updater.OnCheckFinished += Updater_OnCheckFinished;
            updater.OnDownloadFinished += Updater_OnDownloadFinished;
            updater.OnDownloadProgressChanged += Updater_OnDownloadProgressChanged;
        }

        // Event triggered when the download progress changed
        private void Updater_OnDownloadProgressChanged(long totalbytesdownloaded, float percent)
        {
            this.Invoke((MethodInvoker)delegate
            {
                TotalBytesDownloaded_Label.Text = "Total bytes downloaded : " + totalbytesdownloaded + " - " + Math.Round(percent) + "%";
                ProgressBarDownloading.Value = (int)Math.Round(percent);
            });
        }

        // Event triggered when the download finished
        private void Updater_OnDownloadFinished(long totalbytesdownloaded)
        {
            this.Invoke((MethodInvoker)delegate
            {
                TotalBytesDownloaded_Label.Text = "Total bytes downloaded : " + totalbytesdownloaded + " - 100%";
            });
            MessageBox.Show("Download finished !");
        }

        // Event triggered when the check from the server finished
        private void Updater_OnCheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload)
        {
            this.Invoke((MethodInvoker)delegate
            {
                NbrLocalFiles_Label.Text = "Number of local Files : " + nbrLocalFiles;
                NbrServerFile_Label.Text = "Number of server Files : " + nbrServerFiles;
                NbrFilesToDownload_Label.Text = "Number of files to download : " + nbrFilesToDownload;
                NbrFilesDeleted_Label.Text = "Number of files deleted : " + nbrFilesDeleted;
                TotalBytesToDownload_Label.Text = "Total bytes to download : " + numberOfBytesToDownload;
            });
        }

        private void Check_Button_Click(object sender, EventArgs e)
        {
            updater.CheckFilesFromServerAndDeleteOutdated();
        }

        private void Download_Button_Click(object sender, EventArgs e)
        {
            updater.DownloadFiles();
        }
    }
}
