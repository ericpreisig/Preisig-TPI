/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Starting point of the app, get used dll
*********************************************************************************/

using Presentation.ViewModel;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Public Constructors

        /// <summary>
        /// Set the good dll of C++runtime
        /// </summary>
        public MainWindow()
        {
            //load the c++ runtime (check windows version 64 or 32 bits)
            var appFolder = Path.GetDirectoryName(new Uri(typeof(MainWindow).Assembly.CodeBase).LocalPath);
            var is64 = IntPtr.Size == 8;
            var subfolder = is64 ? "\\x64\\" : "\\x86\\";
            LoadLibrary(appFolder + subfolder + "vcruntime140.dll");

            InitializeComponent();
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Launch the data context on start, the main purpose is to make usable the metrodialogues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel();
        }

        #endregion Private Methods
    }
}