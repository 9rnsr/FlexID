# クリーンアップ
dotnet msbuild -t:Clean -p:Configuration=Release

# 発行
dotnet publish -c Release -p:PublishDir=..\publish
