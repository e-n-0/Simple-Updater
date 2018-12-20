using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Simple_Updater_Library
{
    partial class SimpleUpdater
    {
        private void Check()
        {
            // Check if installation_path folder exists, otherwise create it
            if (!Directory.Exists(this.installation_path))
            {
                Directory.CreateDirectory(this.installation_path);
            }

            // Get server files
            this.server_files = parseFileServer(this.server_url);
            int nbrServerFiles = this.server_files.Count;
            int nbrFilesDeleted = 0;
            int nbrLocalFiles = 0;

            // Check local files with server files
            SearchLocalFiles(this.installation_path, this.installation_path, this.server_files, ref nbrLocalFiles, ref nbrFilesDeleted);
            nbrLocalFiles = nbrLocalFiles - nbrFilesDeleted;
            int nbrFilesToDownload = this.server_files.Count;

            // Get number of bytes to download
            numberOfBytesToDownload = 0;
            foreach (KeyValuePair<string, File> entry in this.server_files)
            {
                this.numberOfBytesToDownload += entry.Value.filesize;
            }

            Check_Finished(nbrLocalFiles, nbrServerFiles, nbrFilesToDownload, nbrFilesDeleted);
        }

        private static bool SearchLocalFiles(string installation_path, string dir, Dictionary<string, File> server_files, ref int nbrFilesLocal, ref int nbrFilesDeleted)
        {
            bool checkFolderEmpty = false;

            try
            {
                // Add all files from current folder
                foreach (string file in Directory.GetFiles(dir))
                {
                    string md5 = getMD5(file);
                    string file_path = file.Substring(installation_path.Length + 1);
                    nbrFilesLocal++;

                    if (server_files.ContainsKey(file_path) && server_files[file_path].md5 == md5)
                    {
                        // So the file is correct - delete it from server dictionary
                        server_files.Remove(file_path);
                    }
                    else
                    {
                        // The server do not contains this file so delete it in local file
                        System.IO.File.Delete(file);
                        checkFolderEmpty = true;
                        nbrFilesDeleted++;
                    }
                }

                // Delete the directory if it's empty
                if (!Directory.EnumerateFileSystemEntries(dir).Any())
                    Directory.Delete(dir);
                else
                {
                    // Search and add all files from directories
                    foreach (string directory in Directory.GetDirectories(dir))
                    {
                        if (SearchLocalFiles(installation_path, directory, server_files, ref nbrFilesLocal, ref nbrFilesDeleted))
                        {
                            // Delete the directory if it's empty
                            if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                            {
                                Directory.Delete(dir);
                                checkFolderEmpty = true;
                            }
                        }
                    }
                }

                return checkFolderEmpty;
            }
            catch
            {
                return false;
            }
        }

        private static string getMD5(string filename)
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

        private static Dictionary<string, File> parseFileServer(string server_url)
        {
            List<File> files_server = new List<File>();
            string jsonFile = "";
            using (var client = new System.Net.WebClient())
            {
                try
                {
                    jsonFile = client.DownloadString(server_url);
                }
                catch
                {
                    throw new Exception("An error occured will trying to access the server");
                }
            }

            if (!string.IsNullOrEmpty(jsonFile))
            {
                JObject json_file_object = JObject.Parse(jsonFile);
                JToken token = json_file_object.GetValue("files");
                Dictionary<string, File> files_arr_tmp = new Dictionary<string, File>();

                for (int i = 0; i < token.Count(); i++)
                {
                    File file = new File();
                    file = JsonConvert.DeserializeObject<File>(token[i].ToString());

                    files_arr_tmp.Add(file.filename, file);
                }

                return files_arr_tmp;
            }
            else
            {
                // Any files in the server
                return new Dictionary<string, File>();
            }
        }
    }
}
