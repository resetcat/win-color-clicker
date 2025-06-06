using System;
using System.Runtime.InteropServices;

namespace ColorClickerApp.services
{
    public class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public static void MouseEvent(MouseEventFlags value, int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event((int)value, x, y, 0, 0);
        }
    }
}