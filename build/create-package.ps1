$fileVersion = (ls ..\bin\net40\Caliburn.Micro.Extras.dll | % versioninfo).FileVersion
..\.nuget\NuGet.exe pack Caliburn.Micro.Extras.nuspec -Version $fileVersion
