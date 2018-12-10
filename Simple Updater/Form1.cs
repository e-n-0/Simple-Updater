using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_Updater
{
    public partial class Form1 : Form
    {

        // Configuration
        private string installation_path = Path.Combine(Application.StartupPath, "files");
        private string urlServer = "http://localhost";

        // Vars
        Dictionary<string, string> files_local;
        Dictionary<string, File> files_server;
        List<File> files_to_download;
        List<string> files_to_delete;
        ulong bytesdownloaded = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public void VerificationFiles()
        {
            // Reset / Set lists and dictionary
            files_local = new Dictionary<string, string>();
            files_server = new Dictionary<string, File>();
            files_to_download = new List<File>();
            files_to_delete = new List<string>();

            // Check if installation_path folder exists, otherwise create it
            if (!Directory.Exists(installation_path))
            {
                Directory.CreateDirectory(installation_path);
            }

            // Check all files
            addFilesAndMd5(installation_path, files_local);  // Update dictionary local files
            files_server = parseFileServer();

            // Check diff files from server
            foreach (KeyValuePair<string, File> entry in files_server)
            {
                if(files_local.ContainsKey(entry.Key) && files_local[entry.Key] == entry.Value.md5)
                {
                    // This file is ok
                }
                else if(!files_local.ContainsKey(entry.Key))
                {
                    files_to_download.Add(entry.Value);
                }
                else
                {
                    files_to_delete.Add(entry.Key);
                    files_to_download.Add(entry.Value);
                }
            }

            // Check files that doesn't exist anymore in the server but still in local
            foreach(KeyValuePair<string, string> entry in files_local)
            {
                if(!files_server.ContainsKey(entry.Key))
                {
                    files_to_delete.Add(entry.Key);
                }
            }

            // Set labels infos
            label1.Text = "Number of local Files : " + files_local.Count;
            label2.Text = "Number of server Files : " + files_server.Count;
            label3.Text = "Number of files to download : " + files_to_download.Count;
            label4.Text = "Number of files to delete : " + files_to_delete.Count;

            ulong bytestodownload = 0;
            foreach(File file in files_to_download)
            {
                bytestodownload += (ulong)file.filesize;
            }
            label5.Text = "Total bytes to download : " + bytestodownload;
        }

        void DeleteEmptyDirectorys_recursive(string path)
        {
            // If empty
            if(!Directory.EnumerateFileSystemEntries(path).Any())
            {
                Directory.Delete(path);
                DeleteEmptyDirectorys_recursive(System.IO.Directory.GetParent(path).FullName);
            }
        }

        public void DownloadFiles()
        {
            // Delete files before downloading new files
            foreach(string file in files_to_delete)
            {
                string filepath = System.IO.Path.Combine(installation_path, file);
                System.IO.File.Delete(filepath);
                DeleteEmptyDirectorys_recursive(Path.GetDirectoryName(filepath));
            }

            // Download all files
            foreach (File file in files_to_download)
            {
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    label6.Text = bytesdownloaded.ToString();


                    string[] pathDirectory = file.filename.Split('\\');
                    string pathD = file.filename.Replace(pathDirectory[pathDirectory.Length - 1], "");

                    // Create the directory
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(installation_path, pathD));
                    }
                    catch
                    {
                        // Already exists
                    }

                    Uri uri = new Uri(urlServer + "/files/" + file.filename);
                    string path_file_downloaded = Path.Combine(installation_path, file.filename);
                    client.DownloadFileAsync(uri, path_file_downloaded);
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            bytesdownloaded += (ulong)e.BytesReceived;

            this.BeginInvoke((MethodInvoker)delegate {
                label6.Text = bytesdownloaded + e.BytesReceived.ToString();
            });
        }

        Dictionary<string, File> parseFileServer()
        {
            List<File> files_server = new List<File>();
            string jsonFile = "";
            using (var wc = new System.Net.WebClient())
            {
                try
                {
                    jsonFile = wc.DownloadString(urlServer);
                }
                catch
                {
                    MessageBox.Show("An error occured when trying to access to the server");
                }
            }

            if(!string.IsNullOrEmpty(jsonFile))
            {
                JObject json_file_object = JObject.Parse(jsonFile);
                JToken token = json_file_object.GetValue("files");
                Dictionary<string, File> files_arr_tmp = new Dictionary<string, File>();

                for(int i = 0; i < token.Count(); i++)
                {
                    File file = new File();
                    file = JsonConvert.DeserializeObject<File>(token[i].ToString());

                    files_arr_tmp.Add(file.filename, file);
                }

                return files_arr_tmp;
            }
            else
            {
                throw new Exception("Any files");
            }


        }

        void addFilesAndMd5(string sDir, Dictionary<string, string> dic_files)
        {
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                        dic_files.Add(f.Substring(installation_path.Length+1), getMD5(f));
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            addFiles_Directory_AndMd5(sDir, dic_files);
        }

        void addFiles_Directory_AndMd5(string sDir, Dictionary<string, string> dic_files)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        dic_files.Add(f.Substring(installation_path.Length+1), getMD5(f));
                    }
                    addFilesAndMd5(d, dic_files);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VerificationFiles();
        }

        private string getMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DownloadFiles();
        }
    }
}
