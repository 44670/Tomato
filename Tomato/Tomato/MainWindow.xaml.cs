﻿using System;
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

namespace Tomato
{


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer secondTimer;
        Stopwatch stopWatch = new Stopwatch();



        public MainWindow()
        {
            InitializeComponent();
            secondTimer = new DispatcherTimer();
            secondTimer.IsEnabled = true;
            secondTimer.Interval = TimeSpan.FromSeconds(1);
            secondTimer.Tick += SecondPassed;
        }

        long getTimeRemainMs()
        {
            long timeLimit = 25 * 60 * 1000;
            if (!stopWatch.IsRunning)
            {
                return timeLimit;
            }
            return (timeLimit - stopWatch.ElapsedMilliseconds);
        }

        void UpdateTimeDisplay()
        {
            long timeRemain = getTimeRemainMs() / 1000;
            if (this.IsMouseOver) {
                labelTime.Content = String.Format("{0:00}:{1:00}", timeRemain / 60, timeRemain % 60);
            } else
            {
                labelTime.Content = String.Format("{0:00}", timeRemain / 60);
            }
            
        }

        void ResetTimer()
        {
            stopWatch.Reset();
            UpdateTimeDisplay();
        }

        void SecondPassed(object sender, EventArgs e)
        {
            if (!stopWatch.IsRunning)
            {
                return;
            }
            UpdateTimeDisplay();
            if (getTimeRemainMs() <= 0)
            {
                stopWatch.Reset();
                labelTime.Content = "DONE";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            bool shouldStart = !stopWatch.IsRunning;
            ResetTimer();
            if (shouldStart)
            {
                stopWatch.Restart();
            }
        }
    }
}
