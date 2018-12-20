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
    public partial class SimpleUpdater
    {
        #region Public vars Declaration

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

        #endregion

        #region Events Declaration

        public delegate void CheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload);
        public event CheckFinished OnCheckFinished;

        public delegate void DownloadFinished(long totalbytesdownloaded);
        public event DownloadFinished OnDownloadFinished;

        public delegate void DownloadProgressChanged(long totalbytesdownloaded, float percent);
        public event DownloadProgressChanged OnDownloadProgressChanged;

        #endregion

        #region Variables Declaration

        // Vars - Global of the Updater (Check + Download)
        private Dictionary<string, File> server_files;
        private Queue<string[]> file_and_url_to_download;

        #endregion

        #region Constructor

        public SimpleUpdater(string server_url, string installation_path)
        {
            this.server_url = server_url;
            this.installation_path = installation_path;
            canDownload = false;
            canCheck = true;

            // Example of Event Subscription
            /*OnCheckFinished += SimpleUpdater_OnCheckFinished;
            OnDownloadFinished += SimpleUpdater_OnDownloadFinished;
            OnDownloadProgressChanged += SimpleUpdater_OnDownloadProgressChanged;*/
        }

        #endregion

        #region Event Subscription Examples

        /*
        private void SimpleUpdater_OnDownloadProgressChanged(long totalbytesdownloaded, float percent)
        {
            Debug.WriteLine(totalbytesdownloaded + " - " + percent);
        }

        private void SimpleUpdater_OnCheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload)
        {
            Debug.WriteLine("nbrLocal: " + nbrLocalFiles + " - nbrServer: " + nbrServerFiles + " - Dl: " + nbrFilesToDownload + " - Deleted: " + nbrFilesDeleted + " - BytesToDl: " + numberOfBytesToDownload);
        }

        private void SimpleUpdater_OnDownloadFinished(long bytesdownloaded)
        {
            Debug.WriteLine("Download finished - Bytes:" + bytesdownloaded);
        }
        */

        #endregion



        public void CheckFilesFromServerAndDeleteOutdated()
        {
            if(!canCheck)
            {
                throw new Exception("Check not authorized. Are you downloading ?");
            }

            server_files = new Dictionary<string, File>();

            /*int _nbrLocalFiles = 0;
            int _nbrServerFiles = 0;
            int _nbrFilesToDownload = 0;
            int _nbrFilesDeleted = 0;*/

            this.canCheck = false;
            this.canDownload = false;

            Thread checker = new Thread(() => Check());
            checker.Start();
        }

        public void DownloadFiles()
        {
            if(!canDownload || server_files == null)
            {
                throw new Exception("Download not authorized. Have you checked server files ?");
            }

            this.canDownload = false;
            this.canCheck = false;
            DownloadFiles_Downloader();
        }

        #region Events Redirection

        private void Check_Finished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted)
        {
            this.canCheck = true;
            this.canDownload = true;

            this.OnCheckFinished(nbrLocalFiles, nbrServerFiles, nbrFilesToDownload, nbrFilesDeleted, this.numberOfBytesToDownload);
        }

        private void Download_Finished()
        {
            this.canDownload = true;
            this.canCheck = true;
            this.OnDownloadFinished(this.bytesdownloaded);
        }

        private void DownloadProgress_Changed(long totalbytesdownloaded)
        {
            float percent = (float)(totalbytesdownloaded) / (float)numberOfBytesToDownload;
            OnDownloadProgressChanged(totalbytesdownloaded, percent * 100);
        }

        #endregion
    }
}
