using System;
using System.Runtime.InteropServices;

namespace KeyStrokeAutomator.WinApi
{
	public enum ShowWindowEnum
	{
		Hide = 0,
		ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
		Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
		Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
		Restore = 9, ShowDefault = 10, ForceMinimized = 11
	};

	public static class NativeApi
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

		[DllImport("user32.dll")]
		public static extern int SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();

		[DllImport("user32.dll")]
		public static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

		[DllImport("User32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		[DllImport("user32.dll")]
		public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCursorPos(out POINT lpPoint);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
	}
}
