function global:ScriptAllTables-WithData([string] $server, [string] $database, [string] $output_folder)
{
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null

    $srv = New-Object "Microsoft.SqlServer.Management.SMO.Server" $server
    $db = New-Object ("Microsoft.SqlServer.Management.SMO.Database")
    $tbl = New-Object ("Microsoft.SqlServer.Management.SMO.Table")

    # Get the database object
    $db = $srv.Databases[$database]

    new-item -type directory -name "Data"-path "$output_folder"
    $output_folder += "data\"
    
    # Output the script
    foreach($tbl in $db.Tables)
    {
        $scripter = New-Object ("Microsoft.SqlServer.Management.SMO.Scripter") ($server)

        # Set scripter options to ensure  data is scripted
        $scripter.Options.ScriptSchema = $false;
        $scripter.Options.ScriptData = $true;
        $scripter.Options.ToFileOnly = $true
        $scripter.Options.IncludeHeaders = $false # headers add a timestamp line, rubbish for version control!

        #Exclude GOs after every line
        $scripter.Options.NoCommandTerminator = $true;

        $scripter.Options.Filename = "$output_folder$tbl.sql"
        
        write-host $scripter.Options.Filename
        write-host $tbl
        
        # Output the script
        foreach ($script in $scripter.EnumScript($tbl))
        {
            write-host $script
        }
    }
}


function global:Script-DBObjectsIntoFolders([string]$server, [string]$dbname, [string]$savepath)
{
    #Write-Host "Called Script-DBObjectsIntoFolders for server $server, DB $dbname, save path $savepath"
    
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | out-null
    $SMOserver = New-Object ('Microsoft.SqlServer.Management.Smo.Server') -argumentlist $server
    $db = $SMOserver.databases[$dbname]

    $Objects = $db.Tables
    $Objects += $db.Views
    $Objects += $db.StoredProcedures
    $Objects += $db.UserDefinedFunctions
    $Objects += $db.Users
    $Objects += $db.Schemas
    $Objects += $db.Synonyms

 
    foreach ($ScriptThis in $Objects | where {!($_.IsSystemObject)})
    {
        #Need to Add Some mkDirs for the different $Fldr=$ScriptThis.GetType().Name

        $scriptr = new-object ('Microsoft.SqlServer.Management.Smo.Scripter') ($SMOserver)
        $scriptr.Options.AppendToFile = $True
        $scriptr.Options.AllowSystemObjects = $False
        $scriptr.Options.ClusteredIndexes = $True
        $scriptr.Options.DriAll = $True
        $scriptr.Options.ScriptDrops = $False
        $scriptr.Options.IncludeHeaders = $False
        $scriptr.Options.ToFileOnly = $True
        $scriptr.Options.Indexes = $True
        $scriptr.Options.Permissions = $True
        $scriptr.Options.WithDependencies = $False


        <#Script the Drop too#>
        $ScriptDrop = new-object ('Microsoft.SqlServer.Management.Smo.Scripter') ($SMOserver)
        $ScriptDrop.Options.AppendToFile = $True
        $ScriptDrop.Options.AllowSystemObjects = $False
        $ScriptDrop.Options.ClusteredIndexes = $True
        $ScriptDrop.Options.DriAll = $True
        $ScriptDrop.Options.ScriptDrops = $True
        $ScriptDrop.Options.IncludeHeaders = $False
        $ScriptDrop.Options.ToFileOnly = $True
        $ScriptDrop.Options.Indexes = $True
        $ScriptDrop.Options.WithDependencies = $False

        write-host "SavePath=$SavePath"
        write-host "ScriptThis=$ScriptThis"

        echo $ScriptThis.GetType().Name

        $TypeFolder=$ScriptThis.GetType().Name
        if ((Test-Path -Path "$SavePath\$TypeFolder") -eq "true") 
        {
            "Scripting Out $TypeFolder $ScriptThis"
        } 
        else
        {
            new-item -type directory -name "$TypeFolder"-path "$SavePath"
        }

        $ScriptFile = $ScriptThis -replace "\[|\]"
        $ScriptDrop.Options.FileName = "" + $($SavePath) + "\" + $($TypeFolder) + "\" + $($ScriptFile) + ".SQL"
        $scriptr.Options.FileName = "$SavePath\$TypeFolder\$ScriptFile.SQL"

        #This is where each object actually gets scripted one at a time.
        foreach ($script in $scriptDrop.EnumScript($scriptThis))
        {
            write-host $script
        }
        foreach ($script in $scriptr.EnumScript($scriptThis))
        {
            write-host $script
        }

        #$ScriptDrop.Script($ScriptThis) #This version includes timestamps when the script was generated - useless for version control
        #$scriptr.Script($ScriptThis)
    }
} 


function global:Script-EntireDB([string]$server, [string]$savepath, [string[]]$dbnames, [bool[]]$data)
{
    #Build this portion of the directory structure out here in case scripting takes more than one minute.
    $SavePath += "\DB-" + $(get-date -format yyyyMMddHHmmss)
    $cnt=0
            
    foreach($db in $dbnames)
    {
        new-item -type directory -name "$db"-path "$SavePath"
    
        write-host "New folder $SavePath created in $db"
     
        $sp = "$SavePath\$db"    
     
        Script-DBObjectsIntoFolders $server $db $sp
        if($data[$cnt])
        {
            ScriptAllTables-WithData $server $db "$sp\Table\"
        }

        #Now do a backup - can't easily restore from the .SQL files, can't diff the .BAK        
        #$timestamp = Get-Date -format yyyyMMddHHmmss;
        #$Dest = $Dest + "\" + $db + "_full_" + $timestamp + ".bak", "File";
        #$BackupSetName = "Full backup of " + $db + " " + $timestamp;
        #Backup-SqlDatabase -ServerInstance $Server -Database $db -BackupFile $Dest -CopyOnly -Initialize -Checksum -CompressionOption On -BackupSetName $BackupSetName

        $cnt+=1
    }
}


Script-EntireDB "(local)\SQLExpress" "C:\repo\SBN" @("SBN_App") @($true)
Script-EntireDB "(local)\SQLExpress" "C:\repo\SBN" @("SBN_Data") @($true)
Script-EntireDB "(local)\SQLExpress" "C:\repo\SBN" @("SBN_Documents") @($true)
Script-EntireDB "(local)\SQLExpress" "C:\repo\SBN" @("SBN_Payments") @($true)
Script-EntireDB "(local)\SQLExpress" "C:\repo\SBN" @("SBN_Setup") @($true)
Script-EntireDB "(local)\SQLExpress" "C:\repo\SBN" @("SBN_Site") @($true)