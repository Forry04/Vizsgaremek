#if UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;

public class WindowUtils
{
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    const int GWL_STYLE = -16;
    const uint WS_BORDER = 0x00800000;
    const uint WS_CAPTION = 0x00C00000;

    public static void RemoveWindowBorder()
    {
        IntPtr hWnd = GetActiveWindow();
        uint style = GetWindowLong(hWnd, GWL_STYLE);
        style &= ~WS_BORDER;
        style &= ~WS_CAPTION;
        SetWindowLong(hWnd, GWL_STYLE, style);
    }
}
#endif