if ($args.Length -ne 1) {
    Write-Host "First argument should be an output json file path."
    exit 1
}

$SolutionDir = Join-Path $PSScriptRoot ""
$OutputJsonFile = $args[0]

dotnet tool run nuget-license `
  --input $SolutionDir\FlexID.slnx `
  --ignored-packages "Microsoft.WindowsAppSDK;Microsoft.Windows.SDK.BuildTools" `
  --exclude-projects-matching "*Tests*" `
  --output JsonPretty `
  --file-output $OutputJsonFile
