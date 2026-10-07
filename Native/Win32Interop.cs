using System;
using System.Runtime.InteropServices;

namespace Planner.Native;

/// <summary>
/// Encapsulates Win32 APIs and DWM attributes for native Windows interop.
/// </summary>
public static class Win32Interop
{
    public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    public const int DWMWA_MICA_EFFECT = 1029;
    public const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_FRAMECHANGED = 0x0020;

    public const int SW_SHOW = 5;
    public const int SW_RESTORE = 9;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(
        nint hWnd,
        nint hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(
        nint hwnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute);

    /// <summary>
    /// Sets dark mode title bar styling via DwmSetWindowAttribute.
    /// </summary>
    public static bool SetImmersiveDarkMode(nint hWnd, bool isDarkMode)
    {
        int value = isDarkMode ? 1 : 0;
        int result = DwmSetWindowAttribute(
            hWnd,
            DWMWA_USE_IMMERSIVE_DARK_MODE,
            ref value,
            sizeof(int));
        return result == 0;
    }
}
