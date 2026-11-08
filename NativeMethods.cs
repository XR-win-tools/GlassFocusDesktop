using System;
using System.Runtime.InteropServices;

namespace GlassFocusDesktop;

internal static class NativeMethods
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    public const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    public const int DWMWCP_ROUND = 2;
    public const int DWMWCP_ROUNDSMALL = 3;
}