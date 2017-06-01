using System;
using System.IO;
using System.Runtime.InteropServices;
using Presentation.ViewModel;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Public Constructors

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        public MainWindow()
        {
            //load the c++ runtime (check windows version 64 or 32 bits)
            var appFolder = Path.GetDirectoryName(new Uri(typeof(MainWindow).Assembly.CodeBase).LocalPath);
            var is64 = IntPtr.Size == 8;
            var subfolder = is64 ? "\\x64\\" : "\\x86\\";
            LoadLibrary(appFolder + subfolder + "vcruntime140.dll");

            InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel();
        }

        #endregion Private Methods
    }
}