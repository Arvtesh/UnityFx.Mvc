$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$configuration = $args[0]
$packagesPath = Join-Path $scriptPath "..\temp\BuildTools"
$nugetPath = Join-Path $packagesPath "nuget.exe"
$xunitPath = Join-Path $packagesPath "xunit.runner.console\tools\net452\xunit.console.exe "
$testDllPath = Join-Path $scriptPath (Join-Path "UnityFx.AppStates.Tests\bin" (Join-Path $configuration "\net46\UnityFx.AppStates.Tests.dll"))

# create output folders if needed
if (!(Test-Path $packagesPath)) {
	New-Item $packagesPath -ItemType Directory
}

# download nuget.exe if not present
if (!(Test-Path $nugetPath)) {
	Write-Host "Install NuGet" -Foreground Blue
	Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -Outfile $nugetPath
}

# install & run xunit
Write-Host "Install/update XUnit.net" -Foreground Blue
& $nugetPath install -excludeversion xunit.runner.console -outputdirectory $packagesPath
& $xunitPath $testDllPath