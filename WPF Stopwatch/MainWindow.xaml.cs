using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfStopwatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        #region Declarations; initialization

        private bool IsRunning { get; set; }

        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private string TimeElapsed {
            get { return _stopwatch.Elapsed.ToString(_elapsedTimeFormat, InvariantCulture); }
        }
        private string _elapsedTimeFormat = @"hh\:mm\:ss\.fff";

        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private readonly DispatcherTimer _textBoxTimer = new DispatcherTimer();
        private readonly DispatcherTimer _longIntervalPassTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            _textBoxTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _textBoxTimer.Tick += TextBoxTimer_Tick;

            _longIntervalPassTimer.Interval = new TimeSpan(1, 0, 0, 0, 0);
            _longIntervalPassTimer.Tick += LongIntervalPassTimer_Tick;
        }

        #endregion

        #region Event handlers

        private void TextBoxTimer_Tick(object sender, EventArgs e)
        {
            TextBoxTime.Text = TimeElapsed;
        }

        private void LongIntervalPassTimer_Tick(object sender, EventArgs e)
        {
            _elapsedTimeFormat = @"d\.hh\:mm\:ss\.fff";
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRunning) {
                _stopwatch.Start();
                _longIntervalPassTimer.Start();
                _textBoxTimer.Start();

                ButtonStartStop.Content = "Stop";
                ButtonReset.Content = "Restart";
                ButtonReset.IsEnabled = true;
                TextBoxTime.IsTabStop = false;
                IsRunning = true;

            } else {
                _stopwatch.Stop();
                _textBoxTimer.Stop();
                _longIntervalPassTimer.Stop();

                ButtonStartStop.Content = "Start";
                ButtonReset.Content = "Clear";
                TextBoxTime.IsTabStop = true;
                IsRunning = false;
            }

            Keyboard.ClearFocus();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunning) {
                _stopwatch.Restart();
                _longIntervalPassTimer.Stop();
                _longIntervalPassTimer.Start();

            } else {
                _stopwatch.Reset();
                _longIntervalPassTimer.Stop();
                _longIntervalPassTimer.Start();
            }

            _elapsedTimeFormat = @"hh\:mm\:ss\.fff";
            TextBoxTime.Text = "00:00:00.000";

            Keyboard.ClearFocus();
            if (!IsRunning) {
                ButtonReset.IsEnabled = false;
            }
        }

        private void TextBoxTime_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsRunning) return;

            TextBoxTime.Focus();
            if (TextBoxTime.SelectionLength != 0) return;
            TextBoxTime.SelectAll();
        }

        private void TextBoxTime_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsRunning) return;
            Keyboard.ClearFocus();
        }

        #endregion

        private void TextBoxTime_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (IsRunning && TextBoxTime.SelectionLength != 0) {
                TextBoxTime.Select(0, 0);
            }
        }
    }
}
