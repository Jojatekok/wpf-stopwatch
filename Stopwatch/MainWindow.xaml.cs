using System;
using System.Windows;
using System.Windows.Threading;

namespace Stopwatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        #region Declarations; initialization

        private bool IsRunning { get; set; }

        private string TimeElapsed {
            get { return _stopwatch.Elapsed.ToString(_elapsedTimeFormat); }
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRunning) {
                _stopwatch.Start();
                _longIntervalPassTimer.Start();
                _textBoxTimer.Start();
                ButtonRestart.Content = "Stop";
                IsRunning = true;

            } else {
                _stopwatch.Stop();
                _textBoxTimer.Stop();
                _longIntervalPassTimer.Stop();
                ButtonRestart.Content = "Start";
                IsRunning = false;
            }
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
        }

        #endregion

    }
}
