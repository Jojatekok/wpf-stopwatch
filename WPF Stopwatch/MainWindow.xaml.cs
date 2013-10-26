using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfStopwatch
{
    partial class MainWindow
    {

        #region Declarations; initialization
        
        bool IsRunning { get; set; }
        bool DisplayTotalTime { get; set; }

        uint CurrentLapNumber { get; set; }

        ObservableCollection<Lap> LapTimesCollection { get; set; }

        private TimeSpan _previousLapsTimeElapsed;
        TimeSpan TotalTimeElapsed {
            get { return _previousLapsTimeElapsed.Add(_stopwatch.Elapsed); }
        }

        string ElapsedTimeString
        {
            get { return TimeSpanToString(DisplayTotalTime ?
                                          TotalTimeElapsed :
                                          _stopwatch.Elapsed); }
        }
        private string _elapsedTimeFormat = TimeFormats.Default;

        private readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        private readonly DispatcherTimer _textBoxTimer = new DispatcherTimer();
        private readonly DispatcherTimer _longIntervalPassTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            CurrentLapNumber = 1;
            LapTimesCollection = new ObservableCollection<Lap>();
            DataGridLapTimes.ItemsSource = LapTimesCollection;

            _textBoxTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _textBoxTimer.Tick += TextBoxTimer_Tick;

            _longIntervalPassTimer.Interval = new TimeSpan(1, 0, 0, 0, 0);
            _longIntervalPassTimer.Tick += LongIntervalPassTimer_Tick;
        }

        #endregion

        #region Event handlers

        private void TextBoxTimer_Tick(object sender, EventArgs e)
        {
            TextBoxTime.Text = ElapsedTimeString;
        }

        private void LongIntervalPassTimer_Tick(object sender, EventArgs e)
        {
            _elapsedTimeFormat = TimeFormats.Long;
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
                _stopwatch.Restart();

                _previousLapsTimeElapsed = _previousLapsTimeElapsed.Add(timeElapsed);
                LapTimesCollection.Insert(0, new Lap(CurrentLapNumber, TimeSpanToString(timeElapsed)));
                CurrentLapNumber += 1U;

                if (!DisplayTotalTime) {
                    _longIntervalPassTimer.Stop();
                    _longIntervalPassTimer.Start();

                    _elapsedTimeFormat = TimeFormats.Default;
                    TextBoxTime.Text = "00:00:00.000";
                }

            } else { // Clear
                _stopwatch.Reset();
                _longIntervalPassTimer.Interval = new TimeSpan(1, 0, 0, 0, 0);
                _longIntervalPassTimer.Stop();

                Keyboard.ClearFocus();
                ButtonReset.IsEnabled = false;

                LapTimesCollection.Clear();
                _previousLapsTimeElapsed = default(TimeSpan);
                CurrentLapNumber = 1;

                _elapsedTimeFormat = TimeFormats.Default;
                TextBoxTime.Text = "00:00:00.000";
            }
        }

        private void ButtonStartStop_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ButtonStartStop.FontSize = Math.Min((e.NewSize.Height - 4D) / 1.5,
                                                (e.NewSize.Width - 10D) / 3);
            ButtonReset.FontSize = ButtonStartStop.FontSize;
        }

        private void TextBoxTime_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBoxTime.FontSize = Math.Min((e.NewSize.Height - 2D) / 1.5,
                                            (e.NewSize.Width - 10D) / 5.5);
        }

        private void TextBoxTime_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayTotalTime = !DisplayTotalTime;

            var timeElapsed = DisplayTotalTime ? TotalTimeElapsed : _stopwatch.Elapsed;
            var oneDay = new TimeSpan(1, 0, 0, 0, 0);

            if (timeElapsed < oneDay) {
                _elapsedTimeFormat = TimeFormats.Default;
                _longIntervalPassTimer.Interval = oneDay.Subtract(timeElapsed);
            } else {
                _elapsedTimeFormat = TimeFormats.Long;
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
