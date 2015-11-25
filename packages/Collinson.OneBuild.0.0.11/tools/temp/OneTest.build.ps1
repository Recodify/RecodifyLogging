<#
.Synopsis
	OneTest build script invoked by Invoke-Build.

.Description
	OneTest is a modular set of convention based .NET solution build scripts written in PowerShell, relying on Invoke-Build for task automation. See https://github.com/lholman/OneBuild form more details.
#>

[CmdletBinding()]
param(
	[string] $integrationSearchString = "Tests.Integration",
	[string] $endToEndSearchString = "Tests.EndToEnd",
	[string] $testResultsOutputDirectory = "",
	[string] $excludedCategories = ""
)

$DebugPreference = "Continue"

if ((Test-Path -path "$BuildRoot\tools\powershell\modules" ) -eq $True)
{
	$baseModulePath = "$BuildRoot\tools\powershell\modules"
}else{
	#We order descending so that we can easily drop in a locally built version of OneBuild with a later version number (i.e. with a high buildCounter value) for testing.
	$baseModulePath = Get-ChildItem .\packages -Recurse | Where-Object {$_.Name -like 'Collinson.OneBuild.*' -and $_.PSIsContainer -eq $True} | Sort-Object $_.FullName -Descending | Select-Object FullName -First 1 | foreach {$_.FullName}
	$baseModulePath = "$baseModulePath\tools\powershell\modules"
}


function Run-Integration() {
	Import-Module "$baseModulePath\Invoke-NUnitTestsForAllProjects.psm1"
	Invoke-NUnitTestsForAllProjects -searchString $integrationSearchString -testResultsOutputDirectory $testResultsOutputDirectory -excludedCategories $excludedCategories
	Remove-Module Invoke-NUnitTestsForAllProjects
}	

function Run-EndToEnd() {
	Import-Module "$baseModulePath\Invoke-NUnitTestsForAllProjects.psm1"
	Invoke-NUnitTestsForAllProjects -searchString $endToEndSearchString -testResultsOutputDirectory $testResultsOutputDirectory -excludedCategories $excludedCategories
	Remove-Module Invoke-NUnitTestsForAllProjects
}

Run-Integration
Run-EndToEnd