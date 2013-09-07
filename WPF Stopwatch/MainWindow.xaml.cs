using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
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
        private bool DisplayTotalTime { get; set; }

        private TimeSpan _previousLapsTimeElapsed;
        private TimeSpan TotalTimeElapsed {
            get { return _previousLapsTimeElapsed.Add(_stopwatch.Elapsed); }
        }

        private string ElapsedTimeString
        {
            get { return TimeSpanToString(DisplayTotalTime ?
                                          TotalTimeElapsed :
                                          _stopwatch.Elapsed); }
        }
        private string _elapsedTimeFormat = @"hh\:mm\:ss\.fff";

        private uint _lapNumber = 1;

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

            DataGridLapIdColumn.Binding = new Binding("Id");
            DataGridLapTimeIntervalColumn.Binding = new Binding("TimeInterval");
        }

        #endregion

        #region Event handlers

        private void TextBoxTimer_Tick(object sender, EventArgs e)
        {
            TextBoxTime.Text = ElapsedTimeString;
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
                ButtonReset.Content = "Lap";
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
            if (IsRunning) { // Lap
                var timeElapsed = _stopwatch.Elapsed;
                _previousLapsTimeElapsed = _previousLapsTimeElapsed.Add(timeElapsed);
                DataGridLapTimes.Items.Insert(0, new Lap(_lapNumber, TimeSpanToString(timeElapsed)));
                _lapNumber += 1U;

                _stopwatch.Restart();

                if (!DisplayTotalTime) {
                    _longIntervalPassTimer.Stop();
                    _longIntervalPassTimer.Start();

                    _elapsedTimeFormat = @"hh\:mm\:ss\.fff";
                    TextBoxTime.Text = "00:00:00.000";
                }

            } else { // Clear
                _stopwatch.Reset();
                _longIntervalPassTimer.Interval = new TimeSpan(1, 0, 0, 0, 0);
                _longIntervalPassTimer.Stop();

                Keyboard.ClearFocus();
                ButtonReset.IsEnabled = false;

                DataGridLapTimes.Items.Clear();
                _previousLapsTimeElapsed = default(TimeSpan);
                _lapNumber = 1;

                _elapsedTimeFormat = @"hh\:mm\:ss\.fff";
                TextBoxTime.Text = "00:00:00.000";
            }
        }

        private void TextBoxTime_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayTotalTime = !DisplayTotalTime;

            var timeElapsed = !DisplayTotalTime ? _stopwatch.Elapsed : TotalTimeElapsed;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);

            if (timeElapsed < oneDay) {
                _elapsedTimeFormat = @"hh\:mm\:ss\.fff";
                _longIntervalPassTimer.Interval = oneDay.Subtract(timeElapsed);
            } else {
                _elapsedTimeFormat = @"d\.hh\:mm\:ss\.fff";
            }

            if (!IsRunning) {
                TextBoxTime.Text = ElapsedTimeString;
            }
            TextBoxTime.Select(0, 0);
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

        private void TextBoxTime_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (IsRunning && TextBoxTime.SelectionLength != 0) {
                TextBoxTime.Select(0, 0);
            }
        }

        #endregion

        #region TimeSpan conversion

        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        private string TimeSpanToString(TimeSpan timeSpan) {
            return timeSpan.ToString(_elapsedTimeFormat, InvariantCulture);
        }

        #endregion
    }
}
