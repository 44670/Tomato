using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;

namespace Tomato
{


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer secondTimer;
        Stopwatch stopWatch = new Stopwatch();

        const string LogPath = "Log";

        public MainWindow()
        {
            InitializeComponent();
            secondTimer = new DispatcherTimer();
            secondTimer.IsEnabled = true;
            secondTimer.Interval = TimeSpan.FromSeconds(1);
            secondTimer.Tick += SecondPassed;
            Directory.CreateDirectory(LogPath);
        }

        long GetTimerRemainMs()
        {
            long timeLimit = 25 * 60 * 1000;
            if (IsTimerStopped())
            {
                return timeLimit;
            }
            return (timeLimit - stopWatch.ElapsedMilliseconds);
        }
        void ResetTimer()
        {
            stopWatch.Reset();
        }

        void HandleTimerTimeout()
        {
            stopWatch.Reset();
        }

        bool IsTimerStopped()
        {
            return !stopWatch.IsRunning;
        }

        void StartTimer()
        {
            stopWatch.Start();
        }

        int colorState = 0;
        Color[] colorTable =
        {
            Color.FromArgb(0x80, 0xFF, 0xFF, 0),
            Color.FromArgb(0x20, 0, 0, 0),
        };
        void UpdateTimer()
        {

            long timeRemain = GetTimerRemainMs() / 1000;
            if (timeRemain < 0)
            {
                HandleTimerTimeout();
            }
            if (IsTimerStopped())
            {

                labelTime.Content = "DONE";
                this.Background = new SolidColorBrush(colorTable[colorState]);
                this.Height = 180;
                colorState++;
                if (colorState >= colorTable.Length)
                {
                    colorState = 0;
                }
            } else
            {
                this.Background = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0x00));
                if (this.IsMouseOver)
                {
                    labelTime.Content = String.Format("{0:00}:{1:00}", timeRemain / 60, timeRemain % 60);
                }
                else
                {
                    labelTime.Content = String.Format("{0:00}", timeRemain / 60);
                }
                this.Height = 90;
            }
        }

        void SecondPassed(object sender, EventArgs e)
        {
            UpdateTimer();
        }


        void AppendTodayLog(string line)
        {
            string fileName = GetLogFileNameByDate(DateTime.Now);
            if (!File.Exists(fileName))
            {
                File.AppendAllText(fileName, "Time,Type,Duration\n");
            }
            File.AppendAllText(fileName, line + "\n");
        }

        string GetLogFileNameByDate(DateTime date)
        {
            return "Log\\log-" + date.ToString("yyyyMMdd") + ".csv";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTimer();

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch(Exception)
            {

            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ResetTimer();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ResetTimer();
            StartTimer();
            UpdateTimer();
            string type = comboTimeUsedFor.Text;
            AppendTodayLog(DateTime.Now.ToString("HH:mm") + "," + type + "," + "25");
        }
    }
}
