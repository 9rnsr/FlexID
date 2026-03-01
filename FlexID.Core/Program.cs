using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FlexID.Core.Tests")]

namespace FlexID;

internal class Program
{
    public static Exception Error(string msg)
    {
        return new ApplicationException(string.Format(msg));
    }
}
