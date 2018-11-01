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

namespace Tomato
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        private uint _blurBackgroundColor = 0xffffff; /* BGR color format */
        private uint _blurOpacity = 0x40;

        void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            accent.GradientColor = (_blurOpacity << 24) | (_blurBackgroundColor & 0xFFFFFF);

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

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
            this.Background = Brushes.Transparent;
            EnableBlur();
        }



        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
