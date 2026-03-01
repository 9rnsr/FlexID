$SolutionDir = Join-Path $PSScriptRoot ""
$OutputDir   = Join-Path $SolutionDir "out"
$PublishDir  = Join-Path $OutputDir "publish"

$BuildOpts = @(
  "--configuration", "Release",
  "-p:SolutionDir=$SolutionDir"
)

# クリーンアップ
dotnet clean FlexUI\FlexUI.csproj               @BuildOpts
dotnet clean ResultChecker\ResultChecker.csproj @BuildOpts

$PublishOpts = @(
  "--output", "$PublishDir"
)

# 発行
dotnet publish FlexUI\FlexUI.csproj               @BuildOpts @PublishOpts
dotnet publish ResultChecker\ResultChecker.csproj @BuildOpts @PublishOpts

# アーカイブ
Compress-Archive -Path $PublishDir/* -DestinationPath $OutputDir/FlexID.zip -Force
