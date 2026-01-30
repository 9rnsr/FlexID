$SolutionDir = Join-Path $PSScriptRoot ""
$OutputDir   = Join-Path $SolutionDir "out"
$PublishDir  = Join-Path $OutputDir "publish"

# クリーンアップ
dotnet msbuild -t:Clean -p:Configuration=Release

# 発行
dotnet publish -c Release -p:PublishDir=$PublishDir

# アーカイブ
Compress-Archive -Path $PublishDir/* -DestinationPath $OutputDir/FlexID.zip
