package fr.s0me1ne.simpleupdater;

public interface SimpleUpdaterEvents {
    void OnCheckFinished(int nbrLocalFiles, int nbrServerFiles, int nbrFilesToDownload, int nbrFilesDeleted, long numberOfBytesToDownload);
    void OnDownloadFinished(long totalbytesdownloaded);
    void OnDownloadProgressChanged(long totalbytesdownloaded, float percent);
    void OnStatusChanged(int code);
}
