﻿namespace GW_Launcher.Utilities;

sealed class NotifyIconHelper
{

    public static Rectangle GetIconRect(NotifyIcon icon)
    {
        var notifyIcon = new NOTIFYICONIDENTIFIER();
        notifyIcon.cbSize = Marshal.SizeOf(notifyIcon);
        notifyIcon.hWnd = GetHandle(icon);
        notifyIcon.uID = GetId(icon);

        Shell_NotifyIconGetRect(ref notifyIcon, out var rect);

        return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public readonly int left;
        public readonly int top;
        public readonly int right;
        public readonly int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NOTIFYICONIDENTIFIER
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uID;
        private readonly Guid guidItem;
    }

    [DllImport("shell32.dll", SetLastError = true)]
    private static extern int Shell_NotifyIconGetRect([In] ref NOTIFYICONIDENTIFIER identifier, [Out] out RECT iconLocation);

    private static readonly FieldInfo? WindowField = typeof(NotifyIcon).GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);
    private static IntPtr GetHandle(NotifyIcon icon)
    {
        if (WindowField == null) throw new InvalidOperationException("Can't find property window of NotifyIcon");
        NativeWindow? window = WindowField.GetValue(icon) as NativeWindow;

        if (window == null) throw new InvalidOperationException("[Useful error message]");  // should not happen?
        return window.Handle;
    }

    private static readonly FieldInfo? IdField = typeof(NotifyIcon).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
    private static int GetId(NotifyIcon icon)
    {
        if (IdField == null) throw new InvalidOperationException("Can't find property id of NotifyIcon");
        return Convert.ToInt32(IdField.GetValue(icon));
    }

}