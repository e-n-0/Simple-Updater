package fr.s0me1ne.simpleupdater;

class File
{
    public String filename;
    public String md5;
    public long filesize;
    
    public File(String filename, String md5, long filesize)
    {
    	this.filename = filename;
    	this.md5 = md5;
    	this.filesize = filesize;
    }
}