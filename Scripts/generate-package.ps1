# CM server endpoint whick is used to generate package
$url = "https://cm.sitecore-instace.com/api/packages/Generate"

# Derictory where a packge will be uploaded to.
$folderForPackageUplpad = '.\downloads'

# Create the upload derictory if it is not exists.
if (-Not (Test-Path -Path $folderForPackageUplpad)) { 
	try {
        New-Item -Path $folderForPackageUplpad -ItemType Directory -ErrorAction Stop | Out-Null #-Force
    }
    catch {
        Write-Error -Message "Unable to create directory '$folderForPackageUplpad'. Error was: $_" -ErrorAction Stop
    }
    "Successfully created directory '$folderForPackageUplpad'."
}

# Path to the manifest.json file which defines a package content.
$manifestJson = Get-Content '.\manifest.json'

$jsonObject = $manifestJson | ConvertFrom-Json
$packageName = $jsonObject.PackageName
$packageVersion = $jsonObject.Version

$output = "{0}\{1}-{2}.zip" -f $folderToDownload, $packageName, $packageVersion

$respons = Invoke-RestMethod -Method POST  -ContentType 'application/json' -Uri $url -body $manifestJson -OutFile $output
