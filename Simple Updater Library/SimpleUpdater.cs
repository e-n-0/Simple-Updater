using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_Updater_Library
{
    public class SimpleUpdater
    {
        private string server_url;
        public string Server_Url
        {
            get => this.server_url;
        }

        private string installation_path;
        public string Installation_path
        {
            get => this.installation_path;
        }

        private bool canDownload;
        public bool CanDownload
        {
            get { return canDownload; }
        }

        private bool canCheck;
        public bool CanCheck
        {
            get { return canCheck; }
        }

        private long numberOfBytesToDownload;
        public long NumberOfBytesToDownload
        {
            get => this.numberOfBytesToDownload;
        }

        public delegate void CheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload);
        public event CheckFinished OnCheckFinished;

        public delegate void DownloadFinished(long totalbytesdownloaded);
        public event DownloadFinished OnDownloadFinished;

        public delegate void DownloadProgressChanged(long totalbytesdownloaded, float percent);
        public event DownloadProgressChanged OnDownloadProgressChanged;

        // Vars
        private Dictionary<string, File> server_files;
        private Queue<string[]> file_and_url_to_download;
        private long bytesdownloaded;
        private bool completed;

        public SimpleUpdater(string server_url, string installation_path)
        {
            this.server_url = server_url;
            this.installation_path = installation_path;
            canDownload = false;
            canCheck = true;

            /*OnCheckFinished += SimpleUpdater_OnCheckFinished;
            OnDownloadFinished += SimpleUpdater_OnDownloadFinished;
            OnDownloadProgressChanged += SimpleUpdater_OnDownloadProgressChanged;*/
        }

        /*private void SimpleUpdater_OnDownloadProgressChanged(long totalbytesdownloaded, float percent)
        {
            Debug.WriteLine(totalbytesdownloaded + " - " + percent);
        }*/

        /*private void SimpleUpdater_OnCheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload)
        {
            Debug.WriteLine("nbrLocal: " + nbrLocalFiles + " - nbrServer: " + nbrServerFiles + " - Dl: " + nbrFilesToDownload + " - Deleted: " + nbrFilesDeleted + " - BytesToDl: " + numberOfBytesToDownload);
        }*/

        /*private void SimpleUpdater_OnDownloadFinished(long bytesdownloaded)
        {
            Debug.WriteLine("Download finished - Bytes:" + bytesdownloaded);
        }*/

        public void DownloadProgress(long totalbytesdownloaded)
        {
            float percent = (float)(totalbytesdownloaded) / (float)numberOfBytesToDownload;
            OnDownloadProgressChanged(totalbytesdownloaded, percent * 100);
        }

        public void CheckFilesFromServerAndDeleteOutdated()
        {
            if(!canCheck)
            {
                throw new Exception("Check not authorized at this point");
            }

            server_files = new Dictionary<string, File>();

            int _nbrLocalFiles = 0;
            int _nbrServerFiles = 0;
            int _nbrFilesToDownload = 0;
            int _nbrFilesDeleted = 0;

            this.canCheck = false;
            this.canDownload = false;

            Thread checker = new Thread(() => Checker.CheckFilesFromServerAndDeleteOutdated(this, this.server_url, this.installation_path, ref server_files,
                ref _nbrLocalFiles, ref _nbrServerFiles, ref _nbrFilesToDownload, ref _nbrFilesDeleted, ref numberOfBytesToDownload));
            checker.Start();
        }

        public void check_finished(ref int nbrLocalFiles, ref int nbrServerFiles, ref int nbrFilesToDownload, ref int nbrFilesDeleted, ref long numberOfBytesToDownload)
        {
            this.canCheck = true;
            this.canDownload = true;
            this.numberOfBytesToDownload = numberOfBytesToDownload;

            this.OnCheckFinished(nbrLocalFiles, nbrServerFiles, nbrFilesToDownload, nbrFilesDeleted, numberOfBytesToDownload);
        }


        public void DownloadFiles()
        {
            if(!canDownload || server_files == null)
            {
                throw new Exception("Download not authorized at this point");
            }

            completed = false;
            bytesdownloaded = 0;
            file_and_url_to_download = new Queue<string[]>();

            this.canDownload = false;
            this.canCheck = false;
            Downloader.DownloadFiles(this, server_files, file_and_url_to_download, installation_path, server_url, ref bytesdownloaded, ref this.numberOfBytesToDownload, ref completed);
        }

        public void Download_Finished(ref long bytesdownloaded)
        {
            this.canDownload = true;
            this.canCheck = true;
            this.OnDownloadFinished(bytesdownloaded);
        }


    }
}
