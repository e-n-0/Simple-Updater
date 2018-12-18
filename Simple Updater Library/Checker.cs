using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Updater_Library
{
    internal class Checker
    {
        public static void CheckFilesFromServerAndDeleteOutdated(SimpleUpdater upt, string server_url, string installation_path, ref Dictionary<string, File> files_server, 
            ref int nbrLocalFiles, ref int nbrServerFiles, ref int nbrFilesToDownload, ref int nbrFilesDeleted, ref long numberOfBytesToDownload)
        {
            // Check if installation_path folder exists, otherwise create it
            if (!Directory.Exists(installation_path))
            {
                Directory.CreateDirectory(installation_path);
            }

            // Get server files
            files_server = parseFileServer(server_url);
            nbrServerFiles = files_server.Count;

            // Check local files with server files
            searchLocalFiles(installation_path, installation_path, files_server, ref nbrLocalFiles, ref nbrFilesDeleted);
            nbrLocalFiles = nbrLocalFiles - nbrFilesDeleted;
            nbrFilesToDownload = files_server.Count;

            // Get number of bytes to download
            numberOfBytesToDownload = 0;
            foreach (KeyValuePair<string, File> entry in files_server)
            {
                numberOfBytesToDownload += entry.Value.filesize;
            }

            upt.check_finished(ref nbrLocalFiles, ref nbrServerFiles, ref nbrFilesToDownload, ref nbrFilesDeleted, ref numberOfBytesToDownload);
        }

        public static bool searchLocalFiles(string installation_path, string dir, Dictionary<string, File> server_files, ref int nbrFilesLocal, ref int nbrFilesDeleted)
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
                        if (searchLocalFiles(installation_path, directory, server_files, ref nbrFilesLocal, ref nbrFilesDeleted))
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
                    throw new Exception("An error occured when trying to access the server");
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
