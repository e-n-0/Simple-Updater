# <div align=center><img src="https://i.imgur.com/1gvVedX.png" /><br>[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/dwyl/esta/issues) [![HitCount](http://hits.dwyl.io/S0me1ne/Simple-Updater.svg)](http://hits.dwyl.io/S0me1ne/Simple-Updater) [![Docs](https://inch-ci.org/github/dwyl/hapi-auth-jwt2.svg)](https://github.com)</div>


**Simple Updater** is a library created to help you building an Updater in few lines of code (for your game launcher or other project).
Just download the library, reference it to your project and you are good to go !
For now, it's only usable with **.NET** *≥ 4.6*
It's developped in **C#**. Check the documention bellow to start using it !  *(An example project is provided in the source)*

### Features:
* Easy integration
* Keep the client updated with the server
* Delete old or unwanted files that doesn't match with the server
* Ignore list 
* Webclient configuration
* Cache of the server result *(Optional - can have a long time response if disabled with a lot of files)*
* Configure the time of the cache
* Trigger event for each step (Checking files, downloading, ...)
* Status code
* Download progression
* Asynchronous download
* Example project fully commented

***Any contribution is welcome !***

## **Table of Contents**
* Presentation
* [Table of contents](#table-of-contents)
* [Installation](#installation)
	* [Web server](#web-server)
	* [Client integration](#client-integration)
* [Code documentation](#code-documentation)
	* [Create the Simple Updater object](#create-the-simple-updater-object.)
	* Function: Check files from the server and delete outdated files
	* Function: Download files
	* [Attributes of SimpleUpdater object](#attributes-of-simpleupdater-object)
	* [Events](#events)
	* [Status code](#status-code)
* [Example Project](#example-project)
* [Credit](#credit)
* [Licence](#licence)

## Installation

### Web Server
You will need a webserver to host files that the updater will download.

1. Clone/Download the project
2. Upload files that are in **'PHP - Server files'** to your webserver
3. Check the **index.php** for configuration
4. Create a folder named **'files'** in the same directory of the **index.php** file
5. Place all files that you want to download in that **'files'** folder

### Client integration
1. Go to the [release](https://github.com/S0me1ne/Simple-Updater/releases) page and download the **Simple Updater** library.
2. Reference it in your project ([Microsoft tutorial](https://msdn.microsoft.com/en-us/library/7314433t(VS.71).aspx))
3. Use the Object **SimpleUpdater** *(see the documentation bellow)*

## Code documentation

### Create the Simple Updater object.
```csharp
// We will keep 'updater' on all the documentation
SimpleUpdater updater = new SimpleUpdater(server_url, installation_path);
```

|Params          |Type        |Description|
|----------------|------------|-----------|
|server_url		 |`string`    | The url of the server that files will be download
| installation_path | `string` | The path where files will be downloaded **(be carefull if not set correctly it can delete bad files)**

### Function: Check files from the server and delete outdated files (keep updated)

```csharp
updater.CheckFilesFromServerAndDeleteOutdated();
```

**Description:**
This function will contact the server to get all files and search through all local files and directories wich files need to be deleted, downloaded or ignored.
Files that needs to be deleted will be automaticaly deleted.
> This function must be called before downloading files.


### Function: Download files

```csharp
updater.DownloadFiles();
```

**Description:**
This will download all files that needs to be downloaded to the `installation_path` provided with the creation of the **SimpleUpdater** object.

### Attributes of SimpleUpdater object

|Name 	 |Type 	|Description
|------- |------|-------
|Server_Url | <div color="red">`string`</div> | The url of the server of files you want to download
| Installation_path | `string` | The path where files will be download
| CanDownload | `bool` | Check if you can start the download of files
| CanCheck | `bool` | Check if you can start the verification process of local files
| NumberOfBytesToDownload | `long` | Get the number of bytes that will be downloaded
| StatusCode | `uint` | Get the status code

### Events

|Name | Args| Description
|--------|------|---
|OnCheckFinished | `int nbrLocalFiles`<br>`int nbrServerFiles`<br>`int nbrFilesToDownload`<br>`int nbrFilesDeleted`<br>`long numberOfBytesToDownload` | Event triggered when the verification process ends
|OnDownloadFinished | `long TotalBytesDownloaded` | Event triggered when the download ends
|OnDownloadProgressChanged | `long TotalBytesDownloaded`<br>`float percent`| Event triggered each time the download progress changed
| OnStatusChanged | `uint code` | Event triggered when the status of the updater change

### Status code

| Code | Description
|-|-
| 0 | Waiting for action
| 1 | Contacting server for files to parse
| 2 | Search and delete local files
| 3 | Check finished<br>Triggered after Check_Finished(...)
| 4 | Download started
| 5 | A file has been successfully downloaded
| 6 | An error occurered when downloading a file
| 7 | Download cancelled
| 8 | Download finished<br>Triggered after Download_Finished(...)

## Example Project

The example project is fully commented. You can find it in **[C# - Example Project Simple Updater](https://github.com/S0me1ne/Simple-Updater/tree/master/C%23%20-%20Example%20Project%20Simple%20Updater "C# - Example Project Simple Updater")**.

![Screen](https://i.imgur.com/VEIsWeq.png)

## Credit
* [S0me1ne](https://github.com/S0me1ne) - Flavien Darche _ [[Twitter](https://twitter.com/_S0me1ne) - [Website]((https://s0me1ne.fr))]

## Licence

[`MIT License`](https://github.com/S0me1ne/Simple-Updater/LICENCE.md)