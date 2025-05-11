using ColorClickerApp.services;
using System.Drawing;
using System.Windows;
using win_mouse_macro.models;

namespace ColorClickerApp {
    public partial class MainWindow : Window {
        private ColorClicker? _clicker;

        public MainWindow() {
            InitializeComponent();
        }

        private void StartClick(object sender, RoutedEventArgs e) {
            try {
                // 1 -- validate user inputs ------------------------
                var color = ColorTranslator.FromHtml(ColorInput.Text);

                var areaParts = AreaInput.Text
                                .Split(',')
                                .Select(p => int.Parse(p.Trim()))
                                .ToArray();
                if (areaParts.Length != 4)
                    throw new FormatException("Area must contain exactly four integers.");

                var area = new Rectangle(areaParts[0], areaParts[1],
                                              areaParts[2], areaParts[3]);

                var interval = int.Parse(IntervalInput.Text);
                var tolerance = int.Parse(ToleranceInput.Text);

                // 2 -- start clicker -------------------------------
                _clicker = new ColorClicker(color, area, tolerance, interval);
                _clicker.Start();
            } catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void StopClick(object sender, RoutedEventArgs e) {
            _clicker?.Dispose();
            _clicker = null!;
        }

        private async void PickColorClick(object sender, RoutedEventArgs e) {
            /* minimise so we don’t cover the pixel you want to pick */
            Hide();
            try {
                var color = await ScreenColorPicker.PickAsync();
                ColorInput.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            } finally {
                Show();
                Activate();
            }
        }
    }
}
