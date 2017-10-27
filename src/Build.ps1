$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$solutionPath = Join-Path $scriptPath "UnityFx.sln"
$configuration = $args[0]
$packagesPath = Join-Path $scriptPath "..\temp\BuildTools"
$binPath = Join-Path $scriptPath "..\bin"
$assetsPath35 = Join-Path $scriptPath "..\AssetStore\UnityPackage35\Assets\UnityFx"
$assetsPath46 = Join-Path $scriptPath "..\AssetStore\UnityPackage46\Assets\UnityFx"
$assetsPathTests = Join-Path $scriptPath "..\Tests\UnityTests\Assets\UnityFx"
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

if (!(Test-Path $binPath)) {
	New-Item $binPath -ItemType Directory
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
& $nugetPath restore $solutionPath
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
$filesToPublish =
	(Join-Path $scriptPath (Join-Path "bin" (Join-Path $configuration "net46\UnityFx.Purchasing.dll"))),
	(Join-Path $scriptPath (Join-Path "bin" (Join-Path $configuration "net46\UnityFx.Purchasing.xml")))

Copy-Item -Path $filesToPublish -Destination $binPath -Force
