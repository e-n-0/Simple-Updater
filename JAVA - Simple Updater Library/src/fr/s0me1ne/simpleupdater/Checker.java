package fr.s0me1ne.simpleupdater;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URL;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import javax.script.ScriptEngine;
import javax.script.ScriptEngineManager;
import javax.script.ScriptException;

class Checker implements Runnable {
	
	long numberOfBytesToDownload;
	String installation_path;
	String server_url;
	List<SimpleUpdaterEvents> listeners;
	Map<String, File> server_files;
	private List<String> ignore_list_files;
    private Map<String, Boolean> ignore_list_folders;
    int nbrLocalFiles;
    int nbrFilesDeleted;
    
    
    public Checker(String installation_path, String server_url, List<SimpleUpdaterEvents> listeners)
    {
    	this.server_url = server_url;
    	this.installation_path = installation_path;
    	this.listeners = listeners;
    }
	
	private void Check()
    {
        // Check if installation_path folder exists, otherwise create it
		Path install_path = Paths.get(this.installation_path);
        if (!Files.exists(install_path) && Files.isDirectory(install_path))
        {
        	try 
        	{
				Files.createDirectory(install_path);
			} 
        	catch (IOException e)
        	{
        		// Failed to create the directory
				e.printStackTrace();
			}
        }

        // Get server files
        ThrowEvent_StatusCodeChanged(1);
        this.server_files = parseFileServer();
        int nbrServerFiles = this.server_files.size();
        this.nbrFilesDeleted = 0;
        this.nbrLocalFiles = 0;

        // Check local files with server files
        ThrowEvent_StatusCodeChanged(2);
        SearchLocalFiles(this.installation_path, false, false);
        nbrLocalFiles = nbrLocalFiles - nbrFilesDeleted;
        int nbrFilesToDownload = this.server_files.size();

        // Get number of bytes to download
        numberOfBytesToDownload = 0;
        for(Map.Entry<String, File> entry : this.server_files.entrySet())
        {
            this.numberOfBytesToDownload += entry.getValue().filesize;
        }
        
        ThrowEvent_CheckFinished(this.nbrLocalFiles, nbrServerFiles, nbrFilesToDownload, this.nbrFilesDeleted, this.numberOfBytesToDownload);
        

    }
	
	@SuppressWarnings("unchecked")
	private Map<String, File> parseFileServer()
    {
        String jsonString = "";
        URL url = null;
        BufferedReader reader;
        String finalString = "";
        
        // Try to get the JSON result of the server
		try 
		{
			url = new URL(this.server_url);
			reader = new BufferedReader(new InputStreamReader(url.openStream()));
			
	        // Get string		
	        while ((jsonString = reader.readLine()) != null)
	        	finalString += jsonString + '\n';
	        	
	        reader.close();
		} 
		catch (Exception e) 
		{
			//An error occured will trying to access the server
			ThrowEvent_StatusCodeChanged(7);
			e.printStackTrace();
			return new HashMap<String, File>();
		}
		
        
		// Parse the JSON to JAVA Object (using javascript engine)
		if(finalString != null && !finalString.isEmpty())
		{
			ScriptEngine engine;
			ScriptEngineManager sem = new ScriptEngineManager();
	        engine = sem.getEngineByName("javascript");
	        
	        String script = "Java.asJSONCompatible(" + finalString + ")";
	        Object result = null;
			try 
			{
				result = engine.eval(script);
			} 
			catch (ScriptException e) 
			{
				// Failed to parse json (server error ?)
				e.printStackTrace();
			}
			
			Map<String, File> files_arr_tmp = new HashMap<String, File>();
			Map<String, Object> ServerResponse = (Map<String, Object>) result;
			
			// Get files array
			List<Object> filesList = (List<Object>)ServerResponse.get("files");
			for(Object file : filesList)
			{
				Map<String, Object> f = (Map<String, Object>) file;
				File newFile = new File((String)f.get("filename"), (String)f.get("md5"), ((Number)f.get("filesize")).intValue());
				files_arr_tmp.put(newFile.filename, newFile);
			}
			
			// Get Ignore list
			Map<String, Object> ignoreList = (Map<String, Object>)ServerResponse.get("ignore");
			
			this.ignore_list_files = new ArrayList<String>();
			List<Object> ignoreFilesList = (List<Object>) ignoreList.get("files");
			for(Object ignoredFile : ignoreFilesList)
			{
				this.ignore_list_files.add((String)ignoredFile);
			}
			
			this.ignore_list_folders = new HashMap<String, Boolean>();
			List<Object> ignoreFolderList = (List<Object>) ignoreList.get("folders");
			for(Object ignoredFolder : ignoreFolderList)
			{
				Map<String, Object> f = (Map<String, Object>) ignoredFolder;
				this.ignore_list_folders.put((String)f.get("folder_path"), (Boolean)f.get("all_subfolders"));
			}
			
			return files_arr_tmp;
		}
		else
		{
			// Any json result from the server, maybe a bad config of the server (blank page)
			return new HashMap<String, File>();
		}
        
    }
	
	private Boolean SearchLocalFiles(String dir, Boolean ignoreFilesInThisFolder, Boolean ignoreAllSubfolder)
    {
		Boolean checkFolderEmpty = false;

        try
        {       	
        	for(java.io.File file : new java.io.File(dir).listFiles(java.io.File::isFile))
            {
            	String md5 = getMD5(file);
            	
            	String file_path = file.getPath().substring(installation_path.length() + 1);
                this.nbrLocalFiles++;

                // Check the ignore list
                if(ignore_list_files.contains(file_path) || (!server_files.containsKey(file_path) && ignoreFilesInThisFolder))
                {
                    // Skip this file
                }
                else if(server_files.containsKey(file_path) && (server_files.get(file_path).md5.equals(md5) || ignoreFilesInThisFolder))
                {
                    // So the file is correct - delete it from server files list dictionary (will be used to download missing files)
                    server_files.remove(file_path);
                }
                else
                {
                    // The server do not contains this file so delete it in local file
                	file.delete();
                    checkFolderEmpty = true;
                    this.nbrFilesDeleted++;
                }
            }

            // Delete the directory if it's empty
        	java.io.File pathFolder = new java.io.File(dir);

        	if(pathFolder.listFiles().length == 0)
        	{
        		pathFolder.delete();
        	}
            else
            {
                // Search and add all files from directories
                Boolean oldValueIgnore = ignoreAllSubfolder;
                for (java.io.File directory : pathFolder.listFiles(java.io.File::isDirectory))
                {
                    // Check if this folder is ignored
                    // Do not check if a parent folder is already ignored with 'all_subfolders' set to true

                    String path_dir = directory.getCanonicalPath().substring(installation_path.length() + 1);
                    ignoreFilesInThisFolder = ignoreAllSubfolder || ignore_list_folders.containsKey(path_dir);

                    if (!ignoreAllSubfolder && ignore_list_folders.containsKey(path_dir))
                        ignoreAllSubfolder = ignore_list_folders.get(path_dir);

                    if (SearchLocalFiles(directory.getCanonicalPath(), ignoreFilesInThisFolder, ignoreAllSubfolder))
                    {
                        // Delete) the directory if it's empty
                    	if(directory.listFiles().length == 0)
                    	{
                    		directory.delete();
                    		checkFolderEmpty = true;
                    	}
                    }

                    ignoreAllSubfolder = oldValueIgnore;
                }
            }

            return checkFolderEmpty;
        }
        catch (Exception e)
        {
            return false;
        }
    }
	
	private String getMD5(java.io.File file)
	{
		try {
			
			// Get checksum
			InputStream fis = new java.io.FileInputStream(file);
			byte[] buffer = new byte[1024];
		    MessageDigest complete = MessageDigest.getInstance("MD5");
		    int numRead;
		    
		    do {
		    	numRead = fis.read(buffer);
		    	if (numRead > 0) {
		    		complete.update(buffer, 0, numRead);
		    	}
		    } while (numRead != -1);

		    fis.close();
		    byte[] b = complete.digest();
		    
		    // Convert to string
		    String result = "";
		    for (int i=0; i < b.length; i++) {
		    	result += Integer.toString( ( b[i] & 0xff ) + 0x100, 16).substring( 1 );
		    }
		    
		    return result;
		} 
		catch (NoSuchAlgorithmException | IOException e)
		{
			e.printStackTrace();
			return "";
		}
	}


	@Override
	public void run() {
		Check();
	}
	
	private void ThrowEvent_CheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload)
	{
		for(SimpleUpdaterEvents e : this.listeners)
		{
			e.OnCheckFinished(nbrLocalFiles, nbrServerFiles, nbrFilesToDownload, nbrFilesDeleted, numberOfBytesToDownload);
			e.OnStatusChanged(3);
			e.OnStatusChanged(0);
		}
	}
	
	private void ThrowEvent_StatusCodeChanged(int code)
	{
		for(SimpleUpdaterEvents e : this.listeners)
		{
			e.OnStatusChanged(code);
		}

	}
	
}
