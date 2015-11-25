<#
.Synopsis
	OneVersion script used to version a non .net project using the same convention as .net based one.

.Description
	OneVersion is a modular set of convention based .NET solution build scripts written in PowerShell.
#>

[CmdletBinding()]
param(
	$buildCounter = "999"
)

$DebugPreference = "Continue"

$major = $null
$minor = $null
$BuildRoot = $null
$versionNumberFileName = "VersionNumber.xml"

if ((Test-Path -path "$BuildRoot\tools\powershell\modules" ) -eq $True)
{
	$baseModulePath = "$BuildRoot\tools\powershell\modules"
}else{
	#We order descending so that we can easily drop in a locally built version of OneBuild with a later version number (i.e. with a high buildCounter value) for testing.
	$baseModulePath = Get-ChildItem .\packages -Recurse | Where-Object {$_.Name -like 'Collinson.OneBuild.*' -and $_.PSIsContainer -eq $True} | Sort-Object $_.FullName -Descending | Select-Object FullName -First 1 | foreach {$_.FullName}
	$baseModulePath = "$baseModulePath\tools\powershell\modules"
	$BuildRoot = Get-Location
}

function Set-VersionNumber() {
	Import-Module "$baseModulePath\Set-BuildNumberWithGitCommitDetail.psm1"
	Set-BuildNumberWithGitCommitDetail -major $major -minor $minor -buildCounter $buildCounter
	Remove-Module Set-BuildNumberWithGitCommitDetail
}	

function Read-MajorMinorVersionNumber() {

	if (Test-Path "$BuildRoot\$($versionNumberFileName)")
	{
		#Retrieve the [major] and [minor] version numbers from the $($versionNumberFileName) file
		[xml]$x = Get-Content "$BuildRoot\$($versionNumberFileName)"
		Write-Warning "$($versionNumberFileName) file found, reading to set [major] and [minor] version numbers."
		$script:major = $x.version.major
		Write-Warning "Setting [major] version number to: $($script:major)."
		$script:minor = $x.version.minor
		Write-Warning "Setting [minor] version number to: $($script:minor)."

	}else{
		Write-Error "No $BuildRoot\$($versionNumberFileName) file found. Maybe you've forgotten to check it in?"
	}
}

Read-MajorMinorVersionNumber
Set-VersionNumber