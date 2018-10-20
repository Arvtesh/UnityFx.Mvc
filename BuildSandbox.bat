powershell .\src\Build.ps1 Release
rd unity\Sandbox\Assets\Plugins\UnityFx.AppStates /S /Q
xcopy bin\AssetStore\net35 unity\Sandbox /S