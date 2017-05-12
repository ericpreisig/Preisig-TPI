using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using BLL;
using DTO;
using DTO.Entity;
using MahApps.Metro.Controls;
using Presentation.Helper;
using Presentation.View;

namespace Presentation.ViewModel
{
    /// <summary>
    /// ViewModel to MainWindow
    /// </summary>
    public class MainWindowViewModel : MainViewModel
    {
        public static MetroWindow MetroWindow = (System.Windows.Application.Current.MainWindow as MetroWindow);
        public static MainWindowViewModel Main;
        public static ObservableCollection<Track> ReadingList { get; set; }

        public MainWindowViewModel()
        {
            if (TrackData.CheckIfDatabaseEmpty())
                MusicSync.NoMusic();

            Main = this;
            ActualView = new MusicView { DataContext = new MusicViewModel() };
        }

        public void RestoreContext()
        {
            throw new NotImplementedException();
        }

        public void WriteSearchBar()
        {
            throw new NotImplementedException();
        }

        public void ClickMusic()
        {
            throw new NotImplementedException();
        }

        public void ClickPlaylist()
        {
            throw new NotImplementedException();
        }

        public void ClickRadio()
        {
            throw new NotImplementedException();
        }

        public void ClickSetting()
        {
            throw new NotImplementedException();
        }
        public void OpenFlyoutReading()
        {
            throw new NotImplementedException();
        }
        private void OpenFlyoutMusic(MusicViewModel musicViewModel)
        {
            throw new NotImplementedException();
        }

        private void OpenFlyoutPlayer(Audio audio)
        {
            throw new NotImplementedException();
        }

        private void OpenFlyoutProperty(Track track)
        {
            throw new NotImplementedException();
        }

        public static PlayerViewModel PlayerViewModel {get; set; }= new PlayerViewModel();


        
        private bool _isFlyoutReadingListOpen;
        public bool IsFlyoutReadingListOpen
        {
            get { return _isFlyoutReadingListOpen; }
            set
            {
                _isFlyoutReadingListOpen = value;
                RaisePropertyChanged();
            }
        }

        private bool _isFlyoutRunningOpen;
        public bool IsFlyoutRunningOpen
        {
            get { return _isFlyoutRunningOpen; }
            set
            {
                _isFlyoutRunningOpen = value;
                RaisePropertyChanged();
            }
        }

        private bool _isFlyoutMusicOpen;
        public bool IsFlyoutMusicOpen
        {
            get { return _isFlyoutMusicOpen; }
            set
            {
                _isFlyoutMusicOpen = value;
                RaisePropertyChanged();
            }
        }

        private UserControl _actualView;

        public UserControl ActualView
        {
            get { return _actualView; }
            set
            {
                _actualView=value; 
                RaisePropertyChanged();
            }
        }


        private string _analyseStatus;

        public string AnalyseStatus
        {
            get { return _analyseStatus; }
            set
            {
                _analyseStatus = value;
                RaisePropertyChanged();
            }
        }
    }
}
