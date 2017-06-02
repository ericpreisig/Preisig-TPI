/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Setting view model that link with all interaction from settingsFlyoutView
*********************************************************************************/

using BLL;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using Presentation.Helper;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace Presentation.ViewModel
{
    /// <summary>
    /// This class contain the logic for the setting flyout
    /// </summary>
    public class SettingFlyoutViewModel : MainViewModel
    {
        #region Private Fields

        private bool _launchMusicOnStart;

        #endregion Private Fields

        #region Public Methods

        private IncludeFolder _selectedItem;

        /// <summary>
        /// init RelayCommand
        /// </summary>
        public SettingFlyoutViewModel()
        {
            OnClickAddFolder = new RelayCommand(ClickAddFolder);
            OnClickRemoveFolder = new RelayCommand(ClickRemoveFolder);
            OnClickSync = new RelayCommand(ClickSync);
            IncludeFolders = new ObservableCollection<IncludeFolder>(GeneralData.GetIncludedFolder());
        }

        public ObservableCollection<IncludeFolder> IncludeFolders { get; set; }

        public bool LaunchMusicOnStart
        {
            get { return Helper.Context.ActualContext.IsMusicPlayingOnStart; }
            set
            {
                _launchMusicOnStart = value;
                Helper.Context.ActualContext.IsMusicPlayingOnStart = _launchMusicOnStart;
                Helper.Context.ActualContext.SaveContext();
                RaisePropertyChanged();
            }
        }

        public RelayCommand OnClickAddFolder { get; set; }
        public RelayCommand OnClickRemoveFolder { get; set; }
        public RelayCommand OnClickSync { get; set; }

        public IncludeFolder SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// launch the folder picker to get a folder
        /// </summary>
        public void ClickAddFolder()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK || GeneralData.GetIncludedFolder().Any(a => a.Path.ToLower() == folderDialog.SelectedPath.ToLower())) return;

                var folder = new IncludeFolder { Path = folderDialog.SelectedPath };
                folder.AddIncludeFolder();
                IncludeFolders.Add(folder);
            }
        }

        /// <summary>
        /// remove the selected folder from the list
        ///</summary>
        public void ClickRemoveFolder()
        {
            if (SelectedItem == null) return;
            SelectedItem.RemoveIncludeFolder();
            IncludeFolders.Remove(SelectedItem);
        }

        /// <summary>
        /// Launch a sync
        /// </summary>
        public void ClickSync()
        {
            MusicSync.SyncAllFolders();
            MainWindowViewModel.Main.IsFlyoutSettingOpen = false;
        }

        #endregion Public Methods
    }
}