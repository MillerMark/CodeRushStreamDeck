Write-Host "Gathering deployment items..."

Write-Host "Script root: $PSScriptRoot`n"

$basePath = $PSScriptRoot

if ($PSSCriptRoot.Length -eq 0) {
  $basePath = $PWD.Path;
}


# Load and parse the plugin project file
$pluginProjectFile = "$basePath\CodeRushStreamDeck.csproj"
$projectContent = Get-Content $pluginProjectFile | Out-String;
$projectXML = [xml]$projectContent;

$buildConfiguration = "Debug"

# Get the target .net core framework
$targetFrameworkName = $projectXML.Project.PropertyGroup.TargetFramework;

# Set local path references
$streamDeckExePath = "$($ENV:ProgramFiles)\Elgato\StreamDeck\StreamDeck.exe"

# For now, this PS script will only be run on Windows.
$bindir = "$basePath\bin\Debug\$targetFrameworkName\win-x64"

# Make sure we actually have a directory/build to deploy
If (-not (Test-Path $bindir)) {
  Write-Error "The output directory `"$bindir`" was not found.`n You must first build the `"CodeRushStreamDeck`" project before calling this script.";
  exit 1;
}

# Load and parse the plugin's manifest file
$manifestFile = $bindir +"\manifest.json"
$manifestContent = Get-Content $manifestFile | Out-String
$json = ConvertFrom-JSON $manifestcontent

$uuidAction = $json.Actions[0].UUID

#$pluginID = $uuidAction.substring(0, $uuidAction.Length - ".action".Length)
$pluginID = "com.devexpress.coderush"
$destDir = "$($env:APPDATA)\Elgato\StreamDeck\Plugins\$pluginID.sdPlugin"

$pluginName = Split-Path $basePath -leaf

# Find the Stream Deck process and stop it!
Get-Process -Name ("StreamDeck", $pluginName) -ErrorAction SilentlyContinue | Stop-Process –force -ErrorAction SilentlyContinue

# Delete the target directory, make sure the deployment/copy is clean
If (Test-Path $destDir) {
  # Manually remove the individual folders to keep The Visual Studio icons there (saves 20 seconds per deploy)
  Remove-Item -Force -Path "$destDir\*.*"
  Remove-Item -Recurse -Force -Path "$destDir\log" -ErrorAction SilentlyContinue 
  Remove-Item -Recurse -Force -Path "$destDir\property_inspector"
  Remove-Item -Force -Path "$destDir\images\*.*"
  Remove-Item -Recurse -Force -Path "$destDir\images\actions"
  Remove-Item -Recurse -Force -Path "$destDir\images\category"
  Remove-Item -Recurse -Force -Path "$destDir\images\symbols"
  #Remove-Item -Recurse -Force | -Path "$destDir"
}

# Then copy all deployment items to the plugin directory
New-Item -Type Directory -Path $destDir -ErrorAction SilentlyContinue # | Out-Null
$bindir = $bindir +"\*"
Copy-Item -Path $bindir -Destination $destDir -Recurse -ErrorAction SilentlyContinue


Write-Host "Deployment complete. Restarting the Stream Deck desktop application..."
Start-Process $streamDeckExePath
exit 0
