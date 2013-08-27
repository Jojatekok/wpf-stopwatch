using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Stopwatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        #region Declarations; initialization

        private bool _running;

        private readonly ElapsedTime _timeElapsed = new ElapsedTime();

        private Thread _thread;
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(1D);

        public MainWindow()
        {
            InitializeComponent();

            _timer.Elapsed += Timer_Elapsed;
            Closing += OnWindowClosing;
        }

        #endregion

        #region Core

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => TextBoxTime.Text = _timeElapsed.ToString()));
        }

        private void StopwatchThread()
        {
            do
            {
                Thread.Sleep(1);
                _timeElapsed.AddOneMillisecond();
            } while (_running);
        }

        private void RestartStopwatchThread()
        {
            _thread = new Thread(StopwatchThread);
            _thread.Start();
        }

        #endregion

        #region Buttons

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var mySender = sender as Button;

            // ReSharper disable PossibleNullReferenceException
            if (!_running) {
                _running = true;
                mySender.Content = "Stop";
                RestartStopwatchThread();
                _timer.Start();

            } else {
                _running = false;
                mySender.Content = "Start";
                _thread.Abort();
                _timer.Stop();
            }
            // ReSharper restore PossibleNullReferenceException
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _timeElapsed.Reset();
            TextBoxTime.Text = "00:00:00.000";
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_thread != null) {
                _thread.Abort();
            }

            _timer.Dispose();
        }

        #endregion

    }
}
