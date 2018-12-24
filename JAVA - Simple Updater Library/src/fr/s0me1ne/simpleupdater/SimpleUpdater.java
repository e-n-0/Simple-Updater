package fr.s0me1ne.simpleupdater;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class SimpleUpdater {

	// Public vars declaration
	
	private String server_url;
    private String installation_path;
    private Boolean canDownload;
    private Boolean canCheck;
    private long numberOfBytesToDownload;
    private int statusCode;

    public String getServerUrl()
    {
    	return this.server_url;
    }
    
    public String getInstallationPath()
    {
    	return this.installation_path;
    }
    
    public Boolean CanDownload()
    {
    	return this.canDownload;
    }
    
    public Boolean CanCheck()
    {
    	return this.canCheck;
    }
    
    public long getNumberOfBytesToDownlaod()
    {
    	return this.numberOfBytesToDownload;
    }
    
    public int getStatusCode()
    {
    	return this.statusCode;
    }
	
    
    // Var - Global of the Updater (Check + Download)
    private Map<String, File> server_files;
    private List<SimpleUpdaterEvents> listeners = new ArrayList<SimpleUpdaterEvents>();
    Checker checker;
    Downloader downloader;
    GetEvent getEvents = new GetEvent(this);
	
    public SimpleUpdater(String server_url, String installation_path)
    {
        this.server_url = server_url;
        this.installation_path = installation_path;
        this.canDownload = false;
        this.canCheck = true;
        this.statusCode = 0;
        addListener(getEvents);
    }
    
    public void addListener(SimpleUpdaterEvents e)
    {
    	this.listeners.add(e);
    }
    
    public void CheckFilesFromServerAndDeleteOutdated()
    {
        if(!canCheck)
        {
            //throw new Exception("Check not authorized. Are you downloading ?");
        	System.out.println("Check not authorized. Are you downloading ?");
        	return;
        }

        server_files = new HashMap<String, File>();

        this.canCheck = false;
        this.canDownload = false;

        checker = new Checker(this.installation_path, this.server_url, this.listeners);
        Thread threadChecker = new Thread(checker);
        threadChecker.start();
    }
    
    public void DownloadFiles()
    {
        if(!canDownload || server_files == null)
        {
            //throw new Exception("Download not authorized. Have you checked out server files ?");
        	System.out.println("Download not authorized. Have you checked out server files ?");

        	return;
        }

        this.canDownload = false;
        this.canCheck = false;
        downloader = new Downloader(this.installation_path, this.server_url, checker.server_files, checker.numberOfBytesToDownload, this.listeners);
        downloader.DownloadFiles_Downloader();
    }
    
    void setCanCheckAndDownloadTrue()
    {
    	this.canCheck = true;
    	this.canDownload = true;
    }

}

class GetEvent implements SimpleUpdaterEvents
{
	SimpleUpdater updater;
	
	GetEvent(SimpleUpdater updater)
	{
		this.updater = updater;
	}

	@Override
	public void OnCheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted,
			long numberOfBytesToDownload) {
		updater.setCanCheckAndDownloadTrue();		
	}

	@Override
	public void OnDownloadFinished(long totalbytesdownloaded) {
		updater.setCanCheckAndDownloadTrue();
	}

	@Override
	public void OnDownloadProgressChanged(long totalbytesdownloaded, float percent) {}

	@Override
	public void OnStatusChanged(int code) {}
	
}
