package fr.s0me1ne.simpleupdater;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.FileOutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.nio.file.Path;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Queue;

class Downloader {

	String installation_path;
	String server_url;
	List<SimpleUpdaterEvents> listeners;
	
	 // Private vars for the Downloader
    private Queue<String> file_to_download;
    private long bytesdownloaded;
    private long numberOfBytesToDownload;
    Map<String, File> server_files;
	
    public Downloader(String installation_path, String server_url, Map<String, File> server_files, long numberOfBytesToDownload, List<SimpleUpdaterEvents> listeners)
    {
    	this.server_url = server_url;
    	this.installation_path = installation_path;
    	this.listeners = listeners;
    	this.server_files = server_files;
    	this.numberOfBytesToDownload = numberOfBytesToDownload;
    }
   
    void DownloadFiles_Downloader()
    {   	
        // Initialize vars
        bytesdownloaded = 0;
        file_to_download = new LinkedList<String>();

        for(Map.Entry<String, File> entry : this.server_files.entrySet())
        {
            // Create the directory
            try
            {
            	Path dest_path_file = java.nio.file.Paths.get(this.installation_path, entry.getValue().filename);
            	new java.io.File(dest_path_file.getParent().toString()).mkdirs();
            }
            catch (Exception e)
            {
                // Already exists
            }

            // Enqueue all files to the queue and all files will be processed one by one 
            this.file_to_download.add(entry.getValue().filename);
        }

        // Don't block the main thread => Async thread
        //Status_Changed(4);
        Thread download = new Thread() {
			@Override
			public void run()
			{
				try 
				{
					ThrowEvent_StatusCodeChanged(4);
					DownloadFile();
				} 
				catch (Exception e) 
				{
					ThrowEvent_StatusCodeChanged(6);
					e.printStackTrace();
				}
			}
        };
        download.start();
    }
    
    private Runnable DownloadProgress_Changed(long bytesreceived, long numberOfBytesToDownload)
    {
    	float percent;
        // Division by 0 ? Don't know if it can occur
        try
        {
            percent = (float)(bytesreceived) / (float)numberOfBytesToDownload;
        }
        catch (Exception e)
        {
            percent = 100;
        }
        
        for(SimpleUpdaterEvents e : this.listeners)
		{
			e.OnDownloadProgressChanged(bytesreceived, percent*100);
		}

		return null;
    }
    
    private void DownloadFile() throws Exception
    {
        // Check if there is any file in the queue
        if (!this.file_to_download.isEmpty())
        {
        	String filename = file_to_download.poll();
        	
        	URL website = new URL(this.server_url + "/files/" + URLEncoder.encode(filename, "UTF-8").replaceAll("\\+", "%20").replaceAll("%5C", "/"));
        	
        	HttpURLConnection httpConnection = (HttpURLConnection) (website.openConnection());
        	//this.totalbytestoreceive = httpConnection.getContentLength();

            BufferedInputStream in = new BufferedInputStream(httpConnection.getInputStream());
            FileOutputStream fos = new FileOutputStream(java.nio.file.Paths.get(this.installation_path, filename).toString());
            BufferedOutputStream bout = new BufferedOutputStream(fos, 2048);
            
            byte[] data = new byte[2048];
            int x = 0;
            while ((x = in.read(data, 0, 2048)) >= 0) 
            {
            	this.bytesdownloaded += x;

            	// Create new thread to trigger event download progress
            	new Thread(DownloadProgress_Changed(this.bytesdownloaded, this.numberOfBytesToDownload)).start();
            	
            	bout.write(data, 0, x);
            }
            bout.close();
            in.close();
            ThrowEvent_StatusCodeChanged(5);
            DownloadFile();
        }
        else
        {
            // Download finished
            Download_Finished();
        }
    }
    
    private void Download_Finished()
    {
    	for(SimpleUpdaterEvents e : this.listeners)
		{
			e.OnDownloadFinished(this.bytesdownloaded);
			e.OnStatusChanged(8);
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
