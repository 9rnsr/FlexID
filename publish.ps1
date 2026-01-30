$SolutionDir = Join-Path -path $PSScriptRoot ""
$PublishDir = Join-Path -path $SolutionDir "out/publish"

# クリーンアップ
dotnet msbuild -t:Clean -p:Configuration=Release

# 発行
dotnet publish -c Release -p:PublishDir=$PublishDir
