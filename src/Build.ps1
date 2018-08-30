$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$solutionPath = Join-Path $scriptPath "UnityFx.AppStates.sln"
$configuration = $args[0]
$packagesPath = Join-Path $scriptPath "..\temp\BuildTools"
$binPath = Join-Path $scriptPath "..\bin"
$assetStorePath = Join-Path $binPath "AssetStore"
$msbuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe"
$nugetPath = Join-Path $packagesPath "nuget.exe"
$gitversionPath = Join-Path $packagesPath "gitversion.commandline\tools\gitversion.exe"

Write-Host "BasePath:" $scriptPath
Write-Host "PackagePath:" $packagesPath
Write-Host "BinPath:" $binPath

# create output folders if needed
if (!(Test-Path $packagesPath)) {
	New-Item $packagesPath -ItemType Directory
}

if (Test-Path $assetStorePath)
{
	Remove-Item $assetStorePath -Force -Recurse
	New-Item $assetStorePath -ItemType Directory
}
else
{
	New-Item $assetStorePath -ItemType Directory
}

# download nuget.exe if not present
if (!(Test-Path $nugetPath)) {
	Write-Host "Install NuGet" -Foreground Blue
	Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -Outfile $nugetPath
}

# install & run GitVersion
Write-Host "Install/update GetVersion" -Foreground Blue
& $nugetPath install -excludeversion gitversion.commandline -outputdirectory $packagesPath
& $gitversionPath /l console /output buildserver

# build projects
Write-Host "Building projects" -Foreground Blue
& $msbuildPath $solutionPath /m /v:m /t:Restore
& $msbuildPath $solutionPath /m /t:Build /p:Configuration=$configuration

# fail if solution build failed
if ($LastExitCode -ne 0) {
	if ($env:CI -eq 'True') {
		$host.SetShouldExit($LastExitCode)
	}
	else {
		exit($LastExitCode);
	}
}

# publish build results to .\Build\Bin
$filesToPublish = (Join-Path $scriptPath (Join-Path "UnityFx.AppStates\bin" (Join-Path $configuration "\*")))
$filesToPublish2 = (Join-Path $scriptPath (Join-Path "UnityFx.AppStates.DependencyInjection\bin" (Join-Path $configuration "\*")))
Copy-Item -Path $filesToPublish -Destination $binPath -Force -Recurse
Copy-Item -Path $filesToPublish2 -Destination $binPath -Force -Recurse

function _PublishAssetStorePackage
{
	param([string]$targetFramework)

	$changelogPath = (Join-Path $scriptPath "..\CHANGELOG.md")
	$readmePath = (Join-Path $scriptPath "UnityFx.AppStates\README.txt")
	$filesToPublish = (Join-Path $scriptPath "UnityFx.AppStates.AssetStore\Assets\*")
	$binToPublish =(Join-Path $binPath (Join-Path $targetFramework "\*")) 
	$publishPath = (Join-Path $assetStorePath (Join-Path $targetFramework "Assets"))
	$publishPath2 = (Join-Path $publishPath "Plugins\UnityFx.AppStates")
	$publishBinPath = (Join-Path $publishPath "Plugins\UnityFx.AppStates\Bin")
	
	New-Item $publishBinPath -ItemType Directory
	Copy-Item -Path $filesToPublish -Destination $publishPath -Force -Recurse
	Copy-Item -Path $binToPublish -Destination $publishBinPath -Force -Recurse
	Copy-Item -Path $changelogPath -Destination $publishPath2 -Force
	Copy-Item -Path $readmePath -Destination $publishPath2 -Force
}

_PublishAssetStorePackage "net35"
_PublishAssetStorePackage "net46"
_PublishAssetStorePackage "netstandard2.0"