# requires java runtime
# found at https://www.java.com/en/download/
java -jar .\antlr-4.7.2-complete.jar -visitor -package ChessLib.Parse.PGN.Parser.BaseClasses -o ..\BaseClasses -Dlanguage=CSharp .\PGN.g4 
$csharpFiles =  $csharpFiles = Get-ChildItem ..\ -Recurse | where {$_.extension -eq ".cs"} | % {
     $content = Get-Content $_.FullName -Raw
     $orig = $content | select -First 10
	  Write-Host '*********************************'
	 Write-Host $_.FullName
	 Write-Host 'Original:'
	 Write-Host ($orig | select -First 10)
     $content = $content -replace 'public class','internal class'
	 $content = $content -replace 'public partial class','internal partial class' 
     $content = $content -replace 'public sealed class','internal sealed class' 
     $content = $content -replace 'public interface','internal interface' 
	 Write-Host 'Modified:'
	 Write-Host ($content | select -First 10)
	 Write-Host '*********************************'
    Set-Content -Path $_.FullName -Value $content
}