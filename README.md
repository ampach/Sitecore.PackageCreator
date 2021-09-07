# Sitecore.PackageCreator

Package Creator is a simple Sitecore module which is served to generated Sitecore packages based on **manifest.json** file via POST request to the Package Creator API. This module will help you to automate the package creation in case of building CI/CD processes or in case you have a requirement to create packages programmatically or usnig scripts.

## manifest.json file example

```json
{
  "PackageName": "Example-Package",
  "Author": "Artsem Prashkovich",
  "Version": "1.0.0",
  "Publisher": "Artsem Prashkovich",
  "Items": [
    {
      "Name": "Content Items",
      "InstallMode": "Overwrite",
      "MergeMode": "Undefined",
      "Entries": [
        {
          "Path": "/sitecore/content",
          "IncludeChildren": true,
          "Database": "master"
        },
        {
          "Path": "/sitecore/templates/Common/Folder",
          "IncludeChildren": true,
          "Database": "master"
        }
      ]
    }
  ],
  "Files": [
    {
      "Name": "Project Files",
      "InstallMode": "Overwrite",
      "MergeMode": "Undefined",
      "Entries": [
        {
          "Path": "/bin/Sitecore.Kernel.dll",
          "IncludeChildren": false
        }
      ]
    }
  ]
}
```

Where: 
- **PackageName**, **Author**, **Version** and **Publisher** - metadata for a package.
- **Items** - list of Sitecore Item sources, where each source is characterized with the following properties:
  - **Name** - name of source;
  - **InstallMode** - installation mode which can take one of the following values: `Overwrite`, `Merge`, `Skip`, `SideBySide`, `Undefined`;
  - **MergeMode** - merge mode is applied only in case of Merge installation mode is set and can take one of the following values: `Undefined`, `Clear`, `Append`, `Merge`;
  - **Entries** - list of entries where each entry is represent the following structure:
    - **Path** - Sitecire path to the item that need to be included to the package;
    - **IncludeChildren** - identifies if the children items need to be included or just selected item itself;
    - **Database** - database that items will be picked up from;
- **Files** - list of Sitecore Item sources, where each source is characterized with the following properties:
  - **Name** - name of source;
  - **InstallMode** - installation mode which can take one of the following values: `Overwrite`, `Merge`, `Skip`, `SideBySide`, `Undefined`;
  - **MergeMode** - merge mode is applied only in case of Merge installation mode is set and can take one of the following values: `Undefined`, `Clear`, `Append`, `Merge`;
  - **Entries** - list of entries where each entry is represent the following structure:
    - **Path** - website root related path to the file or folder that need to be included to the package;
    - **IncludeChildren** - identifies if, in case of folder selected, the children files need to be included or just selected one;

Ones the manifest files is ready, it can be send to the API to generate package.

## Usage example

The Powershell script below shows how the Package Creator can be used.

```powershell
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

```
The result of execution is a Sitecore .zip package uploaded into the `.\downloads` folder.
