using ColorClickerApp.services;
using System.Drawing;
using System.Drawing.Imaging;

namespace win_mouse_macro.models {
    public class ColorClicker : IDisposable {
        private readonly Color _targetColor;
        private readonly Rectangle _scanArea;
        private readonly int _tolerance;
        private readonly int _intervalMs;
        private Timer _timer;
        private bool _enabled;

        public ColorClicker(Color targetColor, Rectangle scanArea, int tolerance = 20, int intervalMs = 30) {
            _targetColor = targetColor;
            _scanArea = scanArea;
            _tolerance = tolerance;
            _intervalMs = intervalMs;
        }

        public void Start() {
            if (_enabled) return;
            _enabled = true;
            _timer = new Timer(Loop, null, 0, _intervalMs);
        }

        public void Stop() {
            _enabled = false;
            _timer?.Dispose();
            _timer = null;
        }

        private void Loop(object state) {
            if (!_enabled) return;

            using var bmp = new Bitmap(_scanArea.Width, _scanArea.Height, PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(_scanArea.Location, Point.Empty, _scanArea.Size);

            var hit = FindMatch(bmp);
            if (hit.HasValue) {
                int screenX = _scanArea.Left + hit.Value.X;
                int screenY = _scanArea.Top + hit.Value.Y;
                Console.WriteLine($"Match at {hit.Value.X},{hit.Value.Y} → clicking...");

                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LEFTDOWN | MouseOperations.MouseEventFlags.LEFTUP, screenX, screenY);
            }
        }

        private Point? FindMatch(Bitmap bmp) {
            for (int y = 0; y < bmp.Height; y += 2) {
                for (int x = 0; x < bmp.Width; x += 2) {
                    var c = bmp.GetPixel(x, y);
                    if (IsMatch(c)) {
                        return new Point(x, y);
                    }
                }
            }
            return null;
        }

        private bool IsMatch(Color c) {
            return Math.Abs(c.R - _targetColor.R) <= _tolerance &&
                   Math.Abs(c.G - _targetColor.G) <= _tolerance &&
                   Math.Abs(c.B - _targetColor.B) <= _tolerance;
        }

        public void Dispose() => Stop();
    }
}