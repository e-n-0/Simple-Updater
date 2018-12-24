package fr.s0me1ne.example;

import fr.s0me1ne.simpleupdater.SimpleUpdater;
import fr.s0me1ne.simpleupdater.SimpleUpdaterEvents;

public class MainExample {

	public static void main(String[] args) 
	{
		// Create the Simple Updater Object
		SimpleUpdater updater = new SimpleUpdater("http://localhost", "C:\\Users\\S0me1ne\\Desktop\\testinstall");
		
		// Create Object to receive event of Simple Updater
		ReceiveEvents event = new ReceiveEvents(updater);
		updater.addListener(event);
		
		// Call method to Check files
		System.out.println("--- Start checking files ---\n");
		updater.CheckFilesFromServerAndDeleteOutdated();
	}

}

class ReceiveEvents implements SimpleUpdaterEvents
{
	SimpleUpdater updater;
	ReceiveEvents(SimpleUpdater updater)
	{
		this.updater = updater;
	}
	
	@Override
	public void OnCheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted,
			long numberOfBytesToDownload) {
		System.out.println("\n=== Checking Files Finished ===");
		System.out.println("Number of local Files: " + nbrLocalFiles);
		System.out.println("Number of server Files: " + nbrServerFiles);
		System.out.println("Number of files to download: " + nbrFilesToDownload);
		System.out.println("Number of files deleted: " + nbrFilesDeleted);
		System.out.println("Total bytes to download : " + numberOfBytesToDownload);
		System.out.println("=== ========= ===\n");

		// Example: When files check finished, download files
		System.out.println("--- Start Download ---\n");
		updater.DownloadFiles();

	}

	@Override
	public void OnDownloadFinished(long totalbytesdownloaded) {
		System.out.println("\n=== Download Finished ! (Total bytes: " + totalbytesdownloaded + ") ===");
		
	}

	@Override
	public void OnDownloadProgressChanged(long totalbytesdownloaded, float percent) {
		System.out.println("Download progress: Bytes: " + totalbytesdownloaded + " - " + percent + "%");
		
	}

	@Override
	public void OnStatusChanged(int code) {
		switch (code)
        {
            case 0: System.out.println("Status: Waiting for action"); return;
            case 1: System.out.println("Status: Contacting server for files to parse"); return;
            case 2: System.out.println("Status: Search and delete local files"); return;
            case 3: System.out.println("Status: Check finished"); return;
            case 4: System.out.println("Status: Download started"); return;
            case 5: System.out.println("Status: A file has been successfully downloaded"); return;
            case 6: System.out.println("Status: An error occurered when downloading a file"); return;
            case 7: System.out.println("Status: An error occured will trying to access the server"); return;
            case 8: System.out.println("Status: Download finished"); return;
            default: return;
        }		
	}
	
}