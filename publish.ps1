$SolutionDir = Join-Path $PSScriptRoot ""
$OutputDir   = Join-Path $SolutionDir "out"
$PublishDir  = Join-Path $OutputDir "publish"

$BuildOpts = @(
  "--configuration", "Release",
  "-p:SolutionDir=$SolutionDir"
)

# クリーンアップ
dotnet clean FlexID\FlexID.csproj @BuildOpts
dotnet clean FlexUI\FlexUI.csproj @BuildOpts

$PublishOpts = @(
  "--output", "$PublishDir"
)

# 発行
dotnet publish FlexID\FlexID.csproj @BuildOpts @PublishOpts
dotnet publish FlexUI\FlexUI.csproj @BuildOpts @PublishOpts

# アーカイブ
Compress-Archive -Path $PublishDir/* -DestinationPath $OutputDir/FlexID.zip -Force
