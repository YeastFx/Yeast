Invoke-Expression ".\dotnet-install.ps1"

dotnet --info

$rootFolder = Split-Path -parent $script:MyInvocation.MyCommand.Definition
$nupkgsFolder = Join-Path $rootFolder "nupkgs"

$versionSuffix = "CI{0:D4}" -f [int]$env:BuildCounter

dotnet restore
dotnet build

$testsPassed = $TRUE
ForEach ($testDir in (Get-ChildItem -Path "test" -Directory)) {
	ForEach ($testProject in (Get-ChildItem -File -Path "*.csproj")) {
		dotnet test $testProject.FullName
		$testsPassed = $testsPassed -and $? 
	}
}

if(-not $testsPassed) {
	exit 1
}