using System.Reflection;
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
}
