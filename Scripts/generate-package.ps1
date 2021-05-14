$url = "https://sc101.local/api/packages/Generate"
$folderToDownload = '.\downloads'

if (-Not (Test-Path -Path $folderToDownload)) { 
	try {
        New-Item -Path $folderToDownload -ItemType Directory -ErrorAction Stop | Out-Null #-Force
    }
    catch {
        Write-Error -Message "Unable to create directory '$folderToDownload'. Error was: $_" -ErrorAction Stop
    }
    "Successfully created directory '$folderToDownload'."
}

$json = Get-Content '.\manifest.json'
$jsonObject = $json | ConvertFrom-Json
$output = "$folderToDownload\$($jsonObject.PackageName)-$($jsonObject.Version).zip"

$respons = Invoke-RestMethod -Method POST  -ContentType 'application/json' -Uri "$($url)" -body $json -OutFile $output