using System.Runtime.InteropServices;

namespace ResEx.Common
{
    internal static class NativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int PathCompactPathEx(string pszOut, string pszSrc, int cchMax, int dwFlags);
    }
}