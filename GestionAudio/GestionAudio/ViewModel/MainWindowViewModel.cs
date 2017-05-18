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
using GalaSoft.MvvmLight.Command;
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

        public RelayCommand OnClickMusic { get; set; }
        public RelayCommand OnClickPlaylist { get; set; }
        public RelayCommand OnClickRadio { get; set; }

        private ObservableCollection<Track> _readingList = new ObservableCollection<Track>();

        public ObservableCollection<Track> ReadingList
        {
            get { return _readingList; }
            set
            {
                _readingList=value;
                RaisePropertyChanged();
            }
        }


        public MainWindowViewModel()
        {
            if (TrackData.CheckIfDatabaseEmpty())
                MusicSync.NoMusic();

            OnClickMusic = new RelayCommand(ClickMusic);
            OnClickPlaylist = new RelayCommand(ClickPlaylist);
            OnClickRadio = new RelayCommand(ClickRadio);

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

        /// <summary>
        /// Open the music view
        /// </summary>
        public void ClickMusic()
        {
            ActualView = new MusicView { DataContext = new MusicViewModel() };
        }

        /// <summary>
        /// Open the playlist view
        /// </summary>
        public void ClickPlaylist()
        {
            ActualView = new PlaylistView { DataContext = new PlaylistViewModel() };
        }

        /// <summary>
        /// Open the radio view
        /// </summary>
        public void ClickRadio()
        {
            ActualView = new RadioView { DataContext = new RadioViewModel() };
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
