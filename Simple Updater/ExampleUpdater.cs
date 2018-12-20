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
            progressBar1.Maximum = 100;

            updater = new SimpleUpdater(urlServer, installation_path);
            updater.OnCheckFinished += Updater_OnCheckFinished;
            updater.OnDownloadFinished += Updater_OnDownloadFinished;
            updater.OnDownloadProgressChanged += Updater_OnDownloadProgressChanged;
        }

        private void Updater_OnDownloadProgressChanged(long totalbytesdownloaded, float percent)
        {
            this.Invoke((MethodInvoker)delegate
            {
                label6.Text = "Total bytes downloaded : " + totalbytesdownloaded + " - " + Math.Round(percent) + "%";
                progressBar1.Value = (int)Math.Round(percent);
            });
        }

        private void Updater_OnDownloadFinished(long totalbytesdownloaded)
        {
            this.Invoke((MethodInvoker)delegate
            {
                label6.Text = "Total bytes downloaded : " + totalbytesdownloaded + " - 100%";
            });
            MessageBox.Show("Download finished !");
        }

        private void Updater_OnCheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload)
        {
            this.Invoke((MethodInvoker)delegate
            {
                label1.Text = "Number of local Files : " + nbrLocalFiles;
                label2.Text = "Number of server Files : " + nbrServerFiles;
                label3.Text = "Number of files to download : " + nbrFilesToDownload;
                label4.Text = "Number of files deleted : " + nbrFilesDeleted;
                label5.Text = "Total bytes to download : " + numberOfBytesToDownload;
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updater.CheckFilesFromServerAndDeleteOutdated();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            updater.DownloadFiles();
        }
    }
}
