# nuget-licenseが出力したJSONファイルを読み込み、各NuGetパッケージの
# ライセンス情報をテキストファイルとして書き出す。

param(
    [Parameter(Mandatory=$true)]
    [string]$InputJsonFile,

    [Parameter(Mandatory=$true)]
    [string]$OutputNoticeFile
)

$packages = Get-Content $InputJsonFile | ConvertFrom-Json

$sb = New-Object System.Text.StringBuilder

$sb.AppendLine("THIRD-PARTY NOTICES") | Out-Null
$sb.AppendLine("====================") | Out-Null
$sb.AppendLine("") | Out-Null
$sb.AppendLine("This application uses third-party libraries listed below.") | Out-Null
$sb.AppendLine("") | Out-Null

foreach ($pkg in $packages) {

    $id = $pkg.PackageId
    $version = $pkg.PackageVersion
    $license = $pkg.License
    $licenseUrl = $pkg.LicenseUrl
    $copyright = $pkg.Copyright
    $authors = $pkg.Authors
    $description = $pkg.Description

    $sb.AppendLine("------------------------------------------------------------") | Out-Null
    $sb.AppendLine("$id ($version)") | Out-Null
    $sb.AppendLine("License: $license") | Out-Null

    if ($licenseUrl) {
        $sb.AppendLine("License URL: $licenseUrl") | Out-Null
    }

    if ($authors) {
        $sb.AppendLine("Authors: $authors") | Out-Null
    }

    if ($copyright) {
        $sb.AppendLine("Copyright: $copyright") | Out-Null
    }

    if ($description) {
        $sb.AppendLine("") | Out-Null
        $sb.AppendLine("Description:") | Out-Null
        $sb.AppendLine($description.Trim()) | Out-Null
    }

    $sb.AppendLine("") | Out-Null
}

$sb.ToString() | Set-Content -Encoding UTF8 $OutputNoticeFile
