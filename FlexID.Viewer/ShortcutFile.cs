using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace FlexID.Viewer
{
    static class CLSID
    {
        /// <summary>
        /// Windowsシステムが提供するShellLinkオブジェクトのCLSID
        /// </summary>
        public static readonly Guid ShellLink = new Guid("00021401-0000-0000-C000-000000000046");

        public static readonly Type ShellLinkType = Type.GetTypeFromCLSID(ShellLink);
    }

    [ComImport]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IShellLink
    {
        void GetPath([Out] StringBuilder pszFile, int cch, [In, Out] IntPtr pfd, int fFlags);
        //void GetPath([Out] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, int fFlags);

        void GetIDList([Out] IntPtr ppidl);
        void SetIDList([In] IntPtr pidl);
        void GetDescription([Out] StringBuilder pszName, int cch);
        void SetDescription([In] string pszName);
        void GetWorkingDirectory([Out] StringBuilder pszDir, int cch);
        void SetWorkingDirectory([In] string pszDir);
        void GetArguments([Out] StringBuilder pszArgs, int cch);
        void SetArguments([In] string pszArgs);
        void GetHotkey([Out] out ushort pwHotkey);
        void SetHotkey([In] ushort wHotkey);
        void GetShowCmd([Out] out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out] StringBuilder pszIconPath, int cch, [Out] out int piIcon);
        void SetIconLocation([In] string pszIconPath, int iIcon);
        void SetRelativePath([In] string pszPathRel, int dwReserved);
        void Resolve([In] IntPtr hwnd, uint fFlags);
        void SetPath([In] string pszFile);
    }

    public static class ShortcutFile
    {
        const uint SLR_NO_UI = 0x0001;
        const uint SLR_ANY_MATCH = 0x0002;
        const uint SLR_UPDATE = 0x0004;
        const uint SLR_NOUPDATE = 0x0008;
        const uint SLR_NOSEARCH = 0x0010;
        const uint SLR_NOTRACK = 0x0020;
        const uint SLR_NOLINKINFO = 0x0040;
        const uint SLR_INVOKE_MSI = 0x0080;
        const uint SLR_NO_UI_WITH_MSG_PUMP = 0x0101;
        const uint SLR_OFFER_DELETE_WITHOUT_FILE = 0x0200;
        const uint SLR_KNOWNFOLDER = 0x0400;
        const uint SLR_MACHINE_IN_LOCAL_TARGET = 0x0800;
        const uint SLR_UPDATE_MACHINE_AND_SID = 0x1000;

        /// <summary>
        /// pathがショートカットファイル(*.lnk)の場合に、リンク先のフルパスを返す。
        /// </summary>
        /// <param name="path">リンク解決対象のファイルパス。</param>
        /// <returns>リンク解決済みのファイルパス。</returns>
        public static string Resolve(string path)
        {
            if (!Path.GetExtension(path).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
                return path;

            IShellLink shellLink = null;
            IPersistFile persistFile = null;
            try
            {
                // CoCreateInstance ではなく Activator で RCW を作成
                shellLink = (IShellLink)Activator.CreateInstance(CLSID.ShellLinkType);
                persistFile = shellLink as IPersistFile;

                persistFile.Load(path, /*STGM_READ*/0);

                // リンクの解決を試みる
                var resolveFlags = SLR_NO_UI | (500 << 16) | SLR_NOUPDATE;
                shellLink.Resolve(/*this.Handle*/IntPtr.Zero, resolveFlags);

                var sb = new StringBuilder(/*MAX_PATH*/260);

                //var data = new WIN32_FIND_DATAW();
                //shellLink.GetPath(sb, sb.Capacity, out data, 0);
                shellLink.GetPath(sb, sb.Capacity, IntPtr.Zero, 0);

                return sb.ToString();
            }
            finally
            {
                if (persistFile != null)
                    Marshal.ReleaseComObject(persistFile);
                if (shellLink != null)
                    Marshal.ReleaseComObject(shellLink);
            }
        }
    }
}
