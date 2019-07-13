param (
    [string]$version)

function Get-Version
{
    param(
    [Parameter(Position=0, Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]
    $version
        )
    $versionArr = $version.Split('.')
    $versionMajor = $versionArr[0] -as [int]
    $versionMid = $versionArr[1] -as [int]
    $versionMinor = $versionArr[2] -as [int]
    
    if($versionMinor -eq  99 )
    {

        $versionMinor = 0
        if($versionMid -eq  99 )
        {
            $versionMid = "0";
            $versionMajor = ($versionMajor +1) -as [string]
        }
        else
        {
            $versionMid = $versionMid + 1
        }
    }
    else
    {    
        $versionMinor = ($versionMinor + 1) -as [string]
    }
    $version = "$versionMajor.$versionMid.$versionMinor"
    
    return $version
}


$base = $(Get-Location).Path
$package="ChessLib"
$installDir = "C:\Users\Wes\nuget"
$projFile = $base + "\ChessLib.Data\ChessLib.Data.csproj"
$nuspecFile = $base + "\ChessLib.nuspec"
if(!$version)
{
    [xml]$myXML = Get-Content $projFile
    $version = Get-Version -version $myXML.Project.PropertyGroup.Version
    Write-Output $version
}
 
 

 [xml]$nuXML = Get-Content $nuspecFile
 $nuXML.package.metadata.version = $version
 $myXML.Project.PropertyGroup.Version = $version

dotnet build -c Release
nuget pack -Build -Properties Configuration=Release -Version ${version}
$command = "nuget add $package.${version}.nupkg -Source $installDir"
Write-Output $command
Invoke-Expression -Command $command
$myXML.Save("$projFile")
$nuXML.Save($nuspecFile)