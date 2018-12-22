<?php

/**
 * Server - Simple Updater
 * 
 * Author : S0me1ne - Flavien Darche
 * GitHub: https://github.com/S0me1ne/Simple-Updater
 * Website: https://s0me1ne.fr
 * 
 * Date: 2018
 */


/**** Configuration ****/

// Enable json cache
$GLOBALS['json_cache'] = true;
// If disabled you can safely delete the lastCheck and cachedJsonResult file if they exist
// NOTE: You can delete lastCheck and cachedJsonResult file to force refresh manualy (even if json_cache is set to true)

// Time of the cache (without refreshing json result in the server)
$GLOBALS['cache_time'] = 5 * 60; // 5 minutes

// Name of the cached json file
$GLOBALS['json_name_file'] = "cachedJsonResult";

// Enable ignore list
// NOTE: That will not ignore files on the server but ignore files on client install folder.
// Ex: If the folder '/testFolder' is ignored, all files in that folder will not be delete nor replaced with files from the server.
//     Otherwise files that are not (or not anymore) in the server but in the ignored folder will not be deleted but files that are on the server but not in the ignored folder will be downloaded. 
//     Same if just ignore a single file
//
// Check 'ignoreList.json' file to setup files or folders you want to ignore
// NOTE: Subfolders will be ignored if setting 'all_subfolders' is set to true in folders ignore list config
$GLOBALS['ignore'] = true;

/****  ****/

// Set the header
header('Content-type: application/json');

// Vars
$GLOBALS['arr_files'] = [];
$GLOBALS['total_files'] = 0;
$GLOBALS['total_bytes'] = 0;

/****  ****/

/**** Functions ****/

// Scan all directories to find all files 
function scanDirectory($target) {

    if(is_dir($target)){

        $files = glob($target . '*', GLOB_MARK ); //GLOB_MARK adds a slash to directories returned

        foreach($files as $file)
        {
            scanDirectory($file);
        }
    } 
    else
    {
        $temp_byte = filesize($target);
        $GLOBALS['total_bytes'] += $temp_byte;
        $GLOBALS['total_files'] += 1;

        $json_data_item = array('filename' => substr($target, 6),
                            'md5' => md5_file($target),
                            'filesize' => $temp_byte);

        array_push($GLOBALS['arr_files'], $json_data_item);
    }
}

function save_json_cache($json)
{
    // Save result to cache file
    $json_cached = fopen($GLOBALS['json_name_file'], "w") or die("Unable to open file!");
    fwrite($json_cached, $json);
    fclose($json_cached);
}

function printallfilesJson()
{
    $date = new DateTime();
    $timestamp = $date->getTimestamp();

    if($GLOBALS['json_cache'] && file_exists("lastCheck"))
    {

        $lastCheck_file = fopen("lastCheck", "r") or die("Unable to open file!");
        $timestamp_val = fread($lastCheck_file, filesize("lastCheck"));

        if(($timestamp - intval($timestamp_val)) > $GLOBALS['cache_time'])
        {
            // So recheck all
            scanDirectory("files");

            // Get ignore liste
            $ignore_list = "";
            if($GLOBALS['ignore'])
            {
                $ignore_list = json_decode(file_get_contents("ignoreList.json"));
            }

            $json_result = (object)array();
            $json_result->files = $GLOBALS['arr_files'];
            $json_result->total_bytes = $GLOBALS['total_bytes'];
            $json_result->total_files = $GLOBALS['total_files'];
            $json_result->ignore = $ignore_list;

            $json_result = json_encode($json_result, JSON_PRETTY_PRINT);
            
            save_json_cache($json_result);

            // Close file
            fclose($lastCheck_file);

            // Reopen it to write inside the new timestamp
            $lastCheck_file = fopen("lastCheck", "w") or die("Unable to open file!");
            fwrite($lastCheck_file, $timestamp);
            fclose($lastCheck_file);

            echo $json_result;
        }
        else
        {
            // Print cached json file
            $json_cached = fopen($GLOBALS['json_name_file'], "r") or die("Unable to open file!");
            $json = fread($json_cached, filesize($GLOBALS['json_name_file']));
            fclose($json_cached);

            echo $json;
        }
    }
    else if($GLOBALS['json_cache'])
    {
        // Json cache enabled but lastCheck file doesn't exist, so create it and recall the function
        $lastCheck_file = fopen("lastCheck", "w") or die("Unable to open file!");;
        fwrite($lastCheck_file, '0');
        fclose($lastCheck_file);

        printallfilesJson();
    }
    else
    {
        // Json cache not enabled
        scanDirectory("files");

        // Get ignore liste
        $ignore_list = "";
        if($GLOBALS['ignore'])
        {
            $ignore_list = json_decode(file_get_contents("ignoreList.json"));
        }
                
        $json_result = (object)array();
        $json_result->files = $GLOBALS['arr_files'];
        $json_result->total_bytes = $GLOBALS['total_bytes'];
        $json_result->total_files = $GLOBALS['total_files'];
        $json_result->ignore = $ignore_list;

        $json_result = json_encode($json_result, JSON_PRETTY_PRINT);
        echo $json_result;
    }
}

/****  ****/

// Call the function
printallfilesJson();

?>
