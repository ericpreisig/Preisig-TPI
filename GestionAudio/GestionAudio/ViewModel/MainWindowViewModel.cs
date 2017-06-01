using BLL;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using Presentation.Helper;
using Presentation.View;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Presentation.ViewModel
{
    /// <summary>
    /// ViewModel to MainWindow
    /// </summary>
    public class MainWindowViewModel : MainViewModel
    {
        #region Public Fields

        public static MainWindowViewModel Main;
        public static MetroWindow MetroWindow = (System.Windows.Application.Current.MainWindow as MetroWindow);

        #endregion Public Fields

        #region Private Fields

        private UserControl _actualView;
        private string _analyseStatus;
        private bool _isFlyoutMusicOpen;
        private bool _isFlyoutReadingListOpen;
        private bool _isFlyoutRunningOpen;
        private SearchViewModel _searchViewModel;
        private ObservableCollection<Track> _readingList = new ObservableCollection<Track>();
        public RelayCommand OnClose { get; set; }

        #endregion Private Fields

        #region Public Constructors

        public MainWindowViewModel()
        {
            if (TrackData.CheckIfDatabaseEmpty())
                MusicSync.NoMusic();

            OnOpenSettingFlyout = new RelayCommand(() =>
            {
                IsFlyoutSettingOpen = true;
                SettingFlyoutViewModel = new SettingFlyoutViewModel();
            });
            OnClickMusic = new RelayCommand(ClickMusic);
            OnClickPlaylist = new RelayCommand(ClickPlaylist);
            OnClickRadio = new RelayCommand(ClickRadio);
            OnSearch = new RelayCommand(Search);
            OnClose = new RelayCommand(SaveContext);
            OnClickElement = new RelayCommand(() => Helper.Context.PlayNewSong(_selectedItem));
            Main = this;

            ActualView = new MusicView { DataContext = MusicViewModel = new MusicViewModel() };
            RestoreContext();
        }

        /// <summary>
        /// Constructor used by unity tests
        /// </summary>
        public MainWindowViewModel(bool test)
        {
            
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// When the user press enter, search if the word on the search box changed
        /// </summary>
        private void Search()
        {
            if(_searchViewModel==null || _searchViewModel.KeyWord!=SearchText)
                Application.Current.Dispatcher.Invoke(() => ActualView = new SearchView { DataContext = _searchViewModel= new SearchViewModel(SearchText) });
        }


        public static PlayerViewModel PlayerViewModel { get; set; } = new PlayerViewModel();

        private MusicViewModel _musicViewModel;
        public MusicViewModel MusicViewModel
        {
            get { return _musicViewModel; }
            set
            {
                _musicViewModel = value;
                RaisePropertyChanged();
            }
        }
        public UserControl ActualView
        {
            get { return _actualView; }
            set
            {
                _actualView = value;
                RaisePropertyChanged();
            }
        }

        private SettingFlyoutViewModel _settingFlyoutViewModel;

        public SettingFlyoutViewModel SettingFlyoutViewModel
        {
            get { return _settingFlyoutViewModel; }
            set
            {
                _settingFlyoutViewModel = value;
                RaisePropertyChanged();
            }
        }

        private Thread _searchThread;

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;              
                RaisePropertyChanged();
            }
        }
        
        public string AnalyseStatus
        {
            get { return _analyseStatus; }
            set
            {
                _analyseStatus = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFlyoutMusicOpen
        {
            get { return _isFlyoutMusicOpen; }
            set
            {
                _isFlyoutMusicOpen = value;
                RaisePropertyChanged();
            }
        }

        private bool _isFlyoutSettingOpen;
        private Track _selectedItem;

        public bool IsFlyoutSettingOpen
        {
            get { return _isFlyoutSettingOpen; }
            set
            {
                _isFlyoutSettingOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFlyoutReadingListOpen
        {
            get { return _isFlyoutReadingListOpen; }
            set
            {
                _isFlyoutReadingListOpen = value;
                RaisePropertyChanged();
            }
        }

        public bool IsFlyoutRunningOpen
        {
            get { return _isFlyoutRunningOpen; }
            set
            {
                _isFlyoutRunningOpen = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OnClickMusic { get; set; }
        public RelayCommand OnClickElement { get; set; }
        public RelayCommand OnClickPlaylist { get; set; }
        public RelayCommand OnSearch { get; set; }
        public RelayCommand OnClickRadio { get; set; }
        public RelayCommand OnOpenSettingFlyout { get; set; }

        /// <summary>
        /// The selected track of the reading list
        /// </summary>
        public Track SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Track> ReadingList
        {
            get { return _readingList; }
            set
            {
                _readingList = value;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Open the music view
        /// </summary>
        public void ClickMusic()
        {
            ActualView = new MusicView { DataContext = MusicViewModel=new MusicViewModel() };
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

        /// <summary>
        /// Save the actual context
        /// </summary>
        public void SaveContext()
        {
            Helper.Context.ActualContext.SaveContext();
        }

        /// <summary>
        /// Set back the last context
        /// </summary>
        private static void RestoreContext()
        {
            var context = GeneralData.LoadContext();
            var isMusicPlaying = context.IsMusicPlaying;
            try
            {
                if (context.Track != null)
                {
                    //launch track
                    var actualTime = (int)Math.Round(1.0 * context.ActualTime / context.Track.Duration * 100);
                    Helper.Context.PlayNewSong(context.Track);
                    MusicPlayer.ChangeTime(actualTime);
                }
                if (context.Radio != null)
                {
                    //Launch a radio 
                    Helper.Context.PlayNewRadio(context.Radio);
                }
                if (!context.IsMusicPlayingOnStart || !isMusicPlaying)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        MusicPlayer.Pause();
                    } );
                }
            }
            catch (Exception e)
            {
                context.Track = null;
                context.Radio = null;
                context.SaveContext();
                Shared.GeneralHelper.ShowMessage("Erreur", "Impossible de récupérer le contexte", MessageDialogStyle.Affirmative);
            }
        }


        #endregion Public Methods

    }
}