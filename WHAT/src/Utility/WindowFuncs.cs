using System.Diagnostics;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace WHAT.Utility;

public static class WindowFuncs {
	[DllImport("user32.dll")]
	private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

	[DllImport("user32.dll")]
	static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);

	[DllImport("gdi32.dll")]
	private static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

	[DllImport("gdi32.dll")]
	private static extern bool BitBlt(
		IntPtr hdcDest, int xDest, int yDest, int width, int height,
		IntPtr hdcSrc, int xSrc, int ySrc, int rop);

	[DllImport("gdi32.dll")]
	private static extern int GetDIBits(
		IntPtr hdc,
		IntPtr hbmp,
		uint start,
		uint lines,
		byte[] bits,
		ref BITMAPINFO bmi,
		uint usage);

	[DllImport("gdi32.dll")]
	private static extern bool DeleteObject(IntPtr h);

	[DllImport("gdi32.dll")]
	private static extern bool DeleteDC(IntPtr hdc);

	[DllImport("user32.dll")]
	private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

	[DllImport("user32.dll")]
	static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

	[DllImport("user32.dll")]
	static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

	private const int SRCCOPY = 0x00CC0020;

	public static Image<Bgra32> CaptureWindow(string processName) {
		processName = Path.GetFileNameWithoutExtension(processName);

		Process proc = Process.GetProcessesByName(processName).FirstOrDefault() ?? throw new Exception("Process not found.");

		IntPtr hwnd = proc.MainWindowHandle;
		if (hwnd == IntPtr.Zero)
			throw new Exception("Window handle not found.");

		GetClientRect(hwnd, out RECT clientRect);

		POINT topLeft = new() { X = clientRect.Left, Y = clientRect.Top };
		ClientToScreen(hwnd, ref topLeft);

		int width = clientRect.Width;
		int height = clientRect.Height;

		IntPtr hdcScreen = GetDC(IntPtr.Zero);
		IntPtr hdcMem = CreateCompatibleDC(hdcScreen);
		IntPtr hBitmap = CreateCompatibleBitmap(hdcScreen, width, height);

		SelectObject(hdcMem, hBitmap);

		BitBlt(
			hdcMem,
			0, 0,
			width, height,
			hdcScreen,
			topLeft.X,
			topLeft.Y,
			SRCCOPY
		);

		byte[] buffer = new byte[width * height * 4];

		BITMAPINFO bmi = new() {
			bmiHeader = new BITMAPINFOHEADER {
				biSize = (uint)Marshal.SizeOf<BITMAPINFOHEADER>(),
				biWidth = width,
				biHeight = -height,
				biPlanes = 1,
				biBitCount = 32,
				biCompression = 0
			}
		};

		GetDIBits(hdcMem, hBitmap, 0, (uint)height, buffer, ref bmi, 0);

		Image<Bgra32> image = Image.LoadPixelData<Bgra32>(buffer, width, height);

		DeleteObject(hBitmap);
		DeleteDC(hdcMem);
		ReleaseDC(IntPtr.Zero, hdcScreen);

		return image;
	}

	private struct POINT {
		public int X;
		public int Y;
	}

	private struct RECT {
		public int Left, Top, Right, Bottom;
		public int Width => Right - Left;
		public int Height => Bottom - Top;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct BITMAPINFO {
		public BITMAPINFOHEADER bmiHeader;
		public uint bmiColors;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct BITMAPINFOHEADER {
		public uint biSize;
		public int biWidth;
		public int biHeight;
		public ushort biPlanes;
		public ushort biBitCount;
		public uint biCompression;
		public uint biSizeImage;
		public int biXPelsPerMeter;
		public int biYPelsPerMeter;
		public uint biClrUsed;
		public uint biClrImportant;
	}
}