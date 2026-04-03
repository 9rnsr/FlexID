if ($args.Length -ne 2) {
    Write-Host "First argument should be an output json file path."
    exit 1
}

$SolutionFile = $args[0]
$OutputJsonFile = $args[1]

dotnet restore $SolutionFile

dotnet tool run nuget-license `
  --input $SolutionFile `
  --ignored-packages "Microsoft.WindowsAppSDK;Microsoft.Windows.SDK.BuildTools;Microsoft.NET.ILLink.Tasks" `
  --exclude-projects-matching "*Tests*" `
  --output JsonPretty `
  --file-output $OutputJsonFile
