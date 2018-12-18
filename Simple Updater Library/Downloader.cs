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
    internal class Downloader
    {
        public static void DownloadFiles(SimpleUpdater upt, Dictionary<string, File> files_server, Queue<string[]> file_and_url_to_download, string installation_path, string url_server, ref long bytesdownloaded, ref long totalbytestoreceive, ref bool completed)
        {
            // Initialize vars
            long _bytesdownloaded = bytesdownloaded;
            long _totalbytestoreceive = totalbytestoreceive;
            bool _completed = completed;

            foreach (KeyValuePair<string, File> entry in files_server)
            {
                string dest_path_file = Path.Combine(installation_path, entry.Value.filename);

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

                Uri uri = new Uri(url_server + "/files/" + entry.Value.filename);
                file_and_url_to_download.Enqueue(new string[] { url_server + "/files/" + entry.Value.filename, dest_path_file });
            }

            Thread download = new Thread(() => DownloadFile(upt, file_and_url_to_download, ref _bytesdownloaded, ref _totalbytestoreceive, ref _completed));
            download.Start();
        }

        static void DownloadFile(SimpleUpdater upt, Queue<string[]> file_and_url_to_download, ref long bytesdownloaded, ref long totalbytestoreceive, ref bool completed)
        {
            if (file_and_url_to_download.Any())
            {
                WebClient client = new WebClient();

                long _bytesdownloaded = bytesdownloaded;
                long _totalbytestoreceive = totalbytestoreceive;
                bool _completed = completed;

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) => client_DownloadProgressChanged(sender, e, upt, ref _bytesdownloaded, ref _totalbytestoreceive, ref _completed));
                client.DownloadFileCompleted += new AsyncCompletedEventHandler((sender, e) => client_DownloadFileCompleted(sender, e, upt, file_and_url_to_download, ref _bytesdownloaded, ref _totalbytestoreceive, ref _completed));

                bytesdownloaded = _bytesdownloaded;
                totalbytestoreceive = _totalbytestoreceive;
                completed = _completed;

                string[] item = (string[])(file_and_url_to_download.Dequeue());

                client.DownloadFileAsync(new Uri(item[0]), item[1]);
            }
            else
            {
                // Download finished
                upt.Download_Finished(ref bytesdownloaded);
            }
        }

        private static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, SimpleUpdater upt, Queue<string[]> _file_and_url_to_download, ref long bytesdownloaded, ref long totalbytestoreceive, ref bool completed)
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

            bytesdownloaded += totalbytestoreceive;

            totalbytestoreceive = 0;
            completed = false;

            DownloadFile(upt, _file_and_url_to_download, ref bytesdownloaded, ref totalbytestoreceive, ref completed);
        }

        private static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e, SimpleUpdater upt, ref long bytesdownloaded, ref long totalbytestoreceive, ref bool completed)
        {
            totalbytestoreceive = e.TotalBytesToReceive;

            if (!completed && e.ProgressPercentage == 100)
            {
                completed = true;
                upt.DownloadProgress(bytesdownloaded + e.BytesReceived);
            }
            else if (!completed)
            {
                upt.DownloadProgress(bytesdownloaded + e.BytesReceived);
            }
        }
    }
}
