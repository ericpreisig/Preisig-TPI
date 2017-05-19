using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BLL;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using Presentation.Helper;
using Context = DTO.Entity.Context;

namespace Presentation.ViewModel
{
    public class SettingFlyoutViewModel : MainViewModel
    {
        private bool _launchMusicOnStart;

        #region Public Methods

        public RelayCommand OnClickRemoveFolder { get; set; }
        public RelayCommand OnClickAddFolder { get; set; }
        public RelayCommand OnClickSync { get; set; }

        public ObservableCollection<IncludeFolder> IncludeFolders { get; set; }
        private IncludeFolder _selectedItem;

        public SettingFlyoutViewModel()
        {
            OnClickAddFolder = new RelayCommand(ClickAddFolder);
            OnClickRemoveFolder=new RelayCommand(ClickRemoveFolder);
            OnClickSync = new RelayCommand(ClickSync);
            IncludeFolders = new ObservableCollection<IncludeFolder>(GeneralData.GetIncludedFolder());
        }

        /// <summary>
        /// launch the folder picker to get a folder
        /// </summary>
        public void ClickAddFolder()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK || GeneralData.GetIncludedFolder().Any(a => a.Path.ToLower() == folderDialog.SelectedPath.ToLower())) return;

                var folder = new IncludeFolder {Path = folderDialog.SelectedPath};
                folder.AddIncludeFolder();
                IncludeFolders.Add(folder);
            }        
        }

        /// <summary>
        /// remove the selected folder from the list
        ///</summary>
        public void ClickRemoveFolder()
        {
            if(SelectedItem==null) return;
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


        public IncludeFolder SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public bool LaunchMusicOnStart
        {
            get { return Helper.Context.ActualContext.IsMusicPlayingOnStart; }
            set
            {
                _launchMusicOnStart = value;
                Helper.Context.ActualContext.IsMusicPlayingOnStart = _launchMusicOnStart;
                Helper.Context.ActualContext.SetContext();
                RaisePropertyChanged();
            }
        }

        #endregion Public Methods
    }
}