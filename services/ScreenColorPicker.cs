using System.Drawing;
using System.Runtime.InteropServices;

namespace ColorClickerApp.services
{
    internal static class ScreenColorPicker
    {
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        private static LowLevelMouseProc? _proc;
        private static TaskCompletionSource<Color>? _tcs;
        private static nint _hook = nint.Zero;

        public static Task<Color> PickAsync()
        {
            _tcs = new TaskCompletionSource<Color>();
            _proc = HookCallback;
            _hook = SetWindowsHookEx(WH_MOUSE_LL, _proc,
                                     GetModuleHandle(null), 0);
            return _tcs.Task;
        }

        private static nint HookCallback(int nCode, nint wParam, nint lParam)
        {
            if (nCode >= 0 && wParam == WM_LBUTTONDOWN)
            {
                var info = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                var col = GetColorAt(info.pt.x, info.pt.y);

                UnhookWindowsHookEx(_hook);
                _tcs!.SetResult(col);
            }
            return CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        /* ---------- pixel sampling ---------- */

        private static Color GetColorAt(int x, int y)
        {
            nint hdc = GetDC(nint.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(nint.Zero, hdc);

            int r = (int)(pixel & 0x000000FF);
            int g = (int)(pixel & 0x0000FF00) >> 8;
            int b = (int)(pixel & 0x00FF0000) >> 16;

            return Color.FromArgb(r, g, b);
        }

        /* ---------- Win32 interop ---------- */

        private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int x, y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData, flags, time;
            public nint dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn,
                                                      nint hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll")]
        private static extern nint CallNextHookEx(nint hhk, int nCode,
                                                    nint wParam, nint lParam);

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(nint hdc, int nXPos, int nYPos);

        [DllImport("user32.dll")]
        private static extern nint GetDC(nint hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(nint hWnd, nint hDC);

        [DllImport("kernel32.dll")]
        private static extern nint GetModuleHandle(string? lpModuleName);
    }
}
