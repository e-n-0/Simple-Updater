using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_Updater_Library
{
    partial class SimpleUpdater
    {
        // Private vars for the Downloader
        private long bytesdownloaded;
        private long totalbytestoreceive;
        private bool completed;

        private void DownloadFiles_Downloader()
        {
            // Initialize vars
            completed = false;
            bytesdownloaded = 0;
            file_and_url_to_download = new Queue<string[]>();

            /*long _bytesdownloaded = bytesdownloaded;
            long _totalbytestoreceive = totalbytestoreceive;
            bool _completed = completed;*/

            foreach (KeyValuePair<string, File> entry in this.server_files)
            {
                string dest_path_file = Path.Combine(this.installation_path, entry.Value.filename);

                // Create the directory
                try
                {
                    string new_dir_path = Path.GetDirectoryName(dest_path_file);
                    Directory.CreateDirectory(new_dir_path);
                }
                catch
                {
                    // Already exists
                }

                Uri uri = new Uri(this.server_url + "/files/" + entry.Value.filename);
                this.file_and_url_to_download.Enqueue(new string[] { this.server_url + "/files/" + entry.Value.filename, dest_path_file });
            }

            Thread download = new Thread(() => DownloadFile());
            download.Start();
        }

        private void DownloadFile()
        {
            if (this.file_and_url_to_download.Any())
            {
                WebClient client = new WebClient();

                //client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) => client_DownloadProgressChanged(sender, e, this.bytesdownloaded, this.totalbytestoreceive, this.completed));
                //client.DownloadFileCompleted += new AsyncCompletedEventHandler((sender, e) => client_DownloadFileCompleted(sender, e, file_and_url_to_download, ref _bytesdownloaded, ref _totalbytestoreceive, ref _completed));

                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;

                /*bytesdownloaded = _bytesdownloaded;
                totalbytestoreceive = _totalbytestoreceive;
                completed = _completed;*/

                string[] item = (string[])(file_and_url_to_download.Dequeue());

                client.DownloadFileAsync(new Uri(item[0]), item[1]);
            }
            else
            {
                // Download finished
                Download_Finished();
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // handle error scenario
                throw e.Error;
            }
            if (e.Cancelled)
            {
                // handle cancelled scenario
            }

            this.bytesdownloaded += this.totalbytestoreceive;

            this.totalbytestoreceive = 0;
            this.completed = false;

            DownloadFile();
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.totalbytestoreceive = e.TotalBytesToReceive;

            if (!this.completed && e.ProgressPercentage == 100)
            {
                this.completed = true;
                DownloadProgress_Changed(this.bytesdownloaded + e.BytesReceived);
            }
            else if (!this.completed)
            {
                DownloadProgress_Changed(this.bytesdownloaded + e.BytesReceived);
            }
        }
    }
}
