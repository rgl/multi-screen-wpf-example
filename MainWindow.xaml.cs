using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using WpfScreenHelper;

namespace MultiScreenWpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Dictionary<string, ScreenWindow> _screenWindows = new Dictionary<string, ScreenWindow>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // se SystemEvents.DisplaySettingsChanged at https://github.com/dotnet/runtime/blob/329f0dbfa3a164ee2e3bf9f75b7641165995c799/src/libraries/Microsoft.Win32.SystemEvents/src/Microsoft/Win32/SystemEvents.cs#L118-L128
            // see WM_DISPLAYCHANGE message at https://docs.microsoft.com/en-us/windows/win32/gdi/wm-displaychange
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;

            UpdateScreenWindows();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        }

        void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdateScreenWindows();
        }

        void UpdateScreenWindows()
        {
            var screenWindows = Screen.AllScreens
                .Select(
                    screen =>
                    {
                        Debug.WriteLine(
                            "{0} bounds={1} workingArea={2} isPrimaryScreen={3}",
                            screen.DeviceName,
                            screen.Bounds,
                            screen.WorkingArea,
                            screen.Primary);
                        var screenWindow = _screenWindows.GetValueOrDefault(screen.DeviceName) ?? new ScreenWindow();
                        screenWindow.Owner = this;
                        screenWindow.ScreenDeviceName = screen.DeviceName;
                        screenWindow.ScreenBounds = screen.Bounds;
                        screenWindow.Top = screen.WorkingArea.Top;
                        screenWindow.Left = screen.WorkingArea.Left;
                        screenWindow.Show();
                        return screenWindow;
                    }
                )
                .ToDictionary(screen => screen.ScreenDeviceName);

            foreach (var deviceName in _screenWindows.Keys.Except(screenWindows.Keys))
            {
                _screenWindows[deviceName].Close();
            }

            _screenWindows = screenWindows;
        }
    }
}
