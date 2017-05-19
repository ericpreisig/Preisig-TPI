using BLL;
using DTO;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using Presentation.View.List;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.ViewModel
{
    public class MusicViewModel : MainViewModel
    {
        #region Private Fields

        private List<ContextMenu> _contextMenu = new List<ContextMenu>();
        private bool _isRightClick;
        private object _musicFlyoutView;
        private Track _rightClickedItem;
        private object _selectedItem;
        private string _type;

        #endregion Private Fields

        #region Public Constructors

        public MusicViewModel()
        {
            Init();
            Tracks = new ObservableCollection<Track>(TrackData.GetTracks());
            Artists = new ObservableCollection<Artist>(ArtistData.GetArtists());
            Albums = new ObservableCollection<Album>(AlbumData.GetAlbums());
            Genres = new ObservableCollection<Genre>(GeneralData.GetGenres());
            SetFavorite();
            CreateContextMenu();
        }

        public MusicViewModel(bool fromSearch)
        {
            Init();
        }

        /// <summary>
        /// When the view displays only tracks
        /// </summary>
        /// <param name="tracks"></param>
        public MusicViewModel(ObservableCollection<Track> tracks)
        {
            Init();
            Tracks = new ObservableCollection<Track>(tracks);
        }

        /// <summary>
        /// When the view is opened by the playlist viewmodel
        /// </summary>
        /// <param name="tracks"></param>
        /// <param name="playlist"></param>
        public MusicViewModel(ObservableCollection<Track> tracks, Playlist playlist)
        {
            Init();

            //add context menu to remove from playlist
            ContextMenu.Add(new ContextMenu
            {
                Header = "Enlever de la playlist " + playlist.Name,
                IsEnable = true,
                Command = OnRemovingFromPlaylist,
                CommandParameter = playlist,
            });

            Tracks = new ObservableCollection<Track>(new List<Track>(tracks));
        }

        /// <summary>
        /// When the view only display albums
        /// </summary>
        /// <param name="albums"></param>
        public MusicViewModel(ObservableCollection<Album> albums)
        {
            Init();
            Albums = new ObservableCollection<Album>(albums);
        }

        /// <summary>
        /// When the view only display artists
        /// </summary>
        /// <param name="albums"></param>
        public MusicViewModel(ObservableCollection<Artist> artists)
        {
            Init();
            Artists = new ObservableCollection<Artist>(artists);
        }


        #endregion Public Constructors

        #region Public Properties

        public ObservableCollection<Album> Albums { get; set; }
        public ObservableCollection<Artist> Artists { get; set; }

        public List<ContextMenu> ContextMenu
        {
            get { return _contextMenu; }
            set
            {
                _contextMenu = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Track> Favorites { get; set; } = new ObservableCollection<Track>();
        public ObservableCollection<Genre> Genres { get; set; }

        public object MusicFlyoutView
        {
            get { return _musicFlyoutView; }
            set
            {
                _musicFlyoutView = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<Playlist> OnAddingToPlaylist { get; set; }
        public RelayCommand OnClickElement { get; set; }
        public RelayCommand OnCreatingPlaylist { get; set; }
        public RelayCommand<Playlist> OnRemovingFromPlaylist { get; set; }
        public RelayCommand OnRightClickTrack { get; set; }
        public RelayCommand<Album> OnClickAlbumCover { get; set; }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_isRightClick)
                    RightClick(_selectedItem);

                if (_selectedItem != null && !(SelectedItem is Track))
                    ClickElement(_selectedItem);
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// When an user click on an element, execute an action
        /// </summary>
        /// <param name="element"></param>
        public void ClickElement(object element)
        {
            MainWindowViewModel.Main.IsFlyoutMusicOpen = false;
            if (element is Track)
            {
                Helper.Context.PlayNewSong((Track)element);
                MainWindowViewModel.Main.ReadingList.Clear();
                MainWindowViewModel.Main.ReadingList.AddRang(Tracks.ToList());
                MainWindowViewModel.Main.IsFlyoutRunningOpen = true;
            }
            else if (element is Album)
            {
                MainWindowViewModel.Main.IsFlyoutMusicOpen = true;
                var view = new TrackListView
                {
                    DataContext = new MusicViewModel(new ObservableCollection<Track>(((Album)element).Tracks))
                };
                MainWindowViewModel.Main.MusicViewModel.MusicFlyoutView = view;
                MainWindowViewModel.Main.MusicViewModel.Type = "Morceaux";

            }
            else if (element is Artist)
            {
                MainWindowViewModel.Main.IsFlyoutMusicOpen = true;

                var view = new AlbumListView
                {
                    DataContext = new MusicViewModel(new ObservableCollection<Album>(((Artist)element).Albums))
                };
                MainWindowViewModel.Main.MusicViewModel.MusicFlyoutView = view;
                MainWindowViewModel.Main.MusicViewModel.Type = "Albums";
            }
            else if (element is Genre)
            {
                MainWindowViewModel.Main.IsFlyoutMusicOpen = true;
                var view = new TrackListView
                {
                    DataContext = new MusicViewModel(new ObservableCollection<Track>(((Genre)element).Tracks))
                };
                MainWindowViewModel.Main.MusicViewModel.MusicFlyoutView = view;
                MainWindowViewModel.Main.MusicViewModel.Type = "Morceaux";            
            }

            //remove the selection of the item selected
            Task.Run(() => { Thread.Sleep(10); SelectedItem = null; });
        }

        /// <summary>
        /// Create the context menu playlists
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<ContextMenu> CreatePlaylistMenu()
        {
            var menu = new ObservableCollection<ContextMenu>
            {
                //newPlaylistCommand
                new ContextMenu
                {
                    Header = "Ajouter dans une nouvelle playlist ...",
                    Command = OnCreatingPlaylist,
                }
            };

            //list all playlits
            foreach (var playlist in PlaylistData.GetPlaylists())
                menu.Add(GetContextMenuOfPlaylist(playlist));

            return menu;
        }


        /// <summary>
        /// Remove a playlist
        /// </summary>
        /// <param name="playlist"></param>
        public void RemovingFromPlaylist(Playlist playlist)
        {
            playlist.Tracks.Remove(_rightClickedItem as Track);

            //update playlist
            (MainWindowViewModel.Main.ActualView.DataContext as PlaylistViewModel).UpdatePlaylist();
        }

        /// <summary>
        /// Set the list of all favorites musics
        /// </summary>
        public void SetFavorite()
        {
            Favorites.Clear();
            foreach (var track in Tracks)
            {
                if (track.IsFavorite)
                    Favorites.Add(track);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Add rightclicked item to playlist
        /// </summary>
        /// <param name="playlist"></param>
        private void AddRightClickedItemToPlaylist(Playlist playlist)
        {
            playlist.Tracks.Add(_rightClickedItem);
            playlist.AddOrUpdatePlayist();
        }

        /// <summary>
        /// Create the right click menu
        /// </summary>
        private void CreateContextMenu()
        {
            ContextMenu.Clear();
            ContextMenu.Add(new ContextMenu
            {
                Header = "Ajouter à une playlist",
                IsEnable = true,
                SubItems = CreatePlaylistMenu()
            });
        }

        /// <summary>
        /// If the user choose the create Playlist option in the context menu
        /// </summary>
        private async void CreatingPlaylist()
        {
            var playlistViewModel = new PlaylistViewModel();
            var playlist = await playlistViewModel.ClickAdd();
            if (playlist == null) return;

            AddRightClickedItemToPlaylist(playlist);
            ContextMenu.ElementAt(0).SubItems.Add(GetContextMenuOfPlaylist(playlist));
        }

        /// <summary>
        /// Get a context out of a playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        private ContextMenu GetContextMenuOfPlaylist(Playlist playlist)
        {
            return new ContextMenu
            {
                Header = playlist.Name,
                Command = OnAddingToPlaylist,
                CommandParameter = playlist
            };
        }

        /// <summary>
        /// setuup all RelayCommand
        /// </summary>
        private void Init()
        {
            OnClickElement = new RelayCommand(() => ClickElement(_selectedItem));
            OnAddingToPlaylist = new RelayCommand<Playlist>(AddRightClickedItemToPlaylist);
            OnRightClickTrack = new RelayCommand(RightClickTrack);
            OnCreatingPlaylist = new RelayCommand(CreatingPlaylist);
            OnRemovingFromPlaylist = new RelayCommand<Playlist>(RemovingFromPlaylist);
            OnClickAlbumCover = new RelayCommand<Album>(ClickElement);
        }

        /// <summary>
        /// When the user right click, set the rightclicked object and remove the selection
        /// </summary>
        /// <param name="track"></param>
        private void RightClick(object track)
        {
            if (SelectedItem is Track)
                _rightClickedItem = track as Track;

            _isRightClick = false;

            //Remove the selection
            Task.Run(() => { Thread.Sleep(10); SelectedItem = null; });
        }

        /// <summary>
        /// When the user right click, set the rightclick variable on
        /// </summary>
        private void RightClickTrack()
        {
            _isRightClick = true;
        }

        #endregion Private Methods
    }
}