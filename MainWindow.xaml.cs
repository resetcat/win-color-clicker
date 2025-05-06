using ColorClickerApp.services;
using System.Drawing;
using System.Windows;
using win_mouse_macro.models;

namespace ColorClickerApp {
    public partial class MainWindow : Window {
        private ColorClicker _clicker;

        public MainWindow() {
            InitializeComponent();
        }

        private void StartClick(object sender, RoutedEventArgs e) {
            try {
                var color = ColorTranslator.FromHtml(ColorInput.Text);
                var parts = AreaInput.Text.Split(',');
                var area = new Rectangle(
                    int.Parse(parts[0]),
                    int.Parse(parts[1]),
                    int.Parse(parts[2]),
                    int.Parse(parts[3])
                );
                var interval = int.Parse(IntervalInput.Text);
                var tolerance = int.Parse(ToleranceInput.Text);

                _clicker = new ColorClicker(color, area, tolerance, interval);
                _clicker.Start();
            } catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void StopClick(object sender, RoutedEventArgs e) {
            _clicker?.Dispose();
            _clicker = null;
        }

        private async void PickColorClick(object sender, RoutedEventArgs e) {
            // minimise so we don’t cover the pixel you want to pick
            this.Hide();

            try {
                var color = await ScreenColorPicker.PickAsync();
                System.Diagnostics.Debug.WriteLine("Picked " + color);
                ColorInput.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            } finally {
                this.Show();
                this.Activate();
            }
        }
    }
}