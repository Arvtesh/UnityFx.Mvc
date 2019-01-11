powershell .\src\Build.ps1 Release
rd unity\Sandbox\Assets\Plugins\UnityFx.Mvc /S /Q
xcopy bin\AssetStore\net35 unity\Sandbox /S