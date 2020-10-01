using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MultiScreenWpf
{
    public partial class ScreenWindow : Window, INotifyPropertyChanged
    {
        private string _screenDeviceName;
        private Rect _screenBounds;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ScreenDeviceName
        {
            get { return _screenDeviceName; }
            set
            {
                _screenDeviceName = value;
                OnPropertyChanged();
            }
        }
        public Rect ScreenBounds
        {
            get { return _screenBounds; }
            set
            {
                _screenBounds = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ScreenWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            SetWindowLong(
                helper.Handle,
                GWL_EXSTYLE,
                GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE
            );
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    }
}
