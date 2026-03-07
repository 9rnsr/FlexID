using System.Reflection;
using System.Text.Json;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        InitializeComponent();

        var ver = typeof(App).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!;

        GitCommitId = ver.Split('+')[1];
        PointerSize = Environment.Is64BitProcess ? "64-bit" : "32-bit";
        VersionNumber = ver.Split('+')[0];

        PackageList = LoadPackageList();
    }

    public string GitCommitId { get; }

    public string PointerSize { get; }

    public string VersionNumber { get; }

    public string MaintainerUrl => "https://github.com/9rnsr";

    public string ProjectUrl => "https://github.com/9rnsr/FlexID";

    public string SourceUrl => $"{ProjectUrl}/v{VersionNumber}";

    public string OriginalRepositoryUrl => "https://github.com/MHI-NSENG/FlexID";

    public string AESJ_Url = "https://www.aesj.net/en/";
    public string AESJ_RST_Url = "http://www.aesj.or.jp/~rst/";

    public List<PackageInfo> PackageList { get; }

    private List<PackageInfo> LoadPackageList()
    {
        var asm = Assembly.GetExecutingAssembly();
        var resourceName = asm.GetManifestResourceNames()
            .First(n => n.EndsWith("package-list.json"));

        using var stream = asm.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        return JsonSerializer.Deserialize<List<PackageInfo>>(json)!;
    }
}

public class PackageInfo
{
    public string PackageId { get; set; } = "";

    public string PackageVersion { get; set; } = "";

    public string PackageProjectUrl { get; set; } = "";

    public string Copyright { get; set; } = "";

    public string Authors { get; set; } = "";

    public string Description { get; set; } = "";

    public string License { get; set; } = "";

    public string LicenseUrl { get; set; } = "";

    public int LicenseInformationOrigin { get; set; }
}
