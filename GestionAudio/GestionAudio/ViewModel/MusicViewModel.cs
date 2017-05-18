using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BLL;
using DTO;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using Presentation.View.List;
using Shared;

namespace Presentation.ViewModel
{
    public class MusicViewModel : MainViewModel
    {

        public ObservableCollection<Track> Tracks { get; set; } = new ObservableCollection<Track>();
        public ObservableCollection<Artist> Artists { get; set; }
        public ObservableCollection<Album> Albums { get; set; }
        public ObservableCollection<Genre> Genres { get; set; }
        public ObservableCollection<Track> Favorites { get; set; } = new ObservableCollection<Track>();
        public RelayCommand OnClickElement { get; set; }
        public RelayCommand OnRightClickTrack { get; set; }
        public RelayCommand OnCreatingPlaylist { get; set; }
        public RelayCommand<Playlist> OnAddingToPlaylist { get; set; }
        public RelayCommand<Playlist> OnRemovingFromPlaylist { get; set; }


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

        private bool _isRightClick;

        /// <summary>
        /// When the user right click, set the rightclick variable on
        /// </summary>
        private void RightClickTrack()
        {
            _isRightClick = true;
        }

        /// <summary>
        /// If the user choose the create Playlist option in the context menu
        /// </summary>
        private async void CreatingPlaylist()
        {
            var playlistViewModel = new PlaylistViewModel();
            var playlist=await playlistViewModel.ClickAdd();
            if (playlist == null) return;

            AddRightClickedItemToPlaylist(playlist);
            ContextMenu.ElementAt(0).SubItems.Add(GetContextMenuOfPlaylist(playlist));
        }

        /// <summary>
        /// When the user right click, set the rightclicked object and remove the selection
        /// </summary>
        /// <param name="track"></param>
        private void RightClick(object track)
        {
            if (SelectedItem is Track)
                _rightClickedItem= track as Track;

            _isRightClick = false;

            //Remove the selection
            Task.Run(() => { Thread.Sleep(10); SelectedItem = null; });

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
        /// Add rightclicked item to playlist
        /// </summary>
        /// <param name="playlist"></param>
        private void AddRightClickedItemToPlaylist(Playlist playlist)
        {
             playlist.Tracks.Add(_rightClickedItem);
             playlist.AddOrUpdatePlayist();
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
                Header = "Enlever de la playlist "+playlist.Name,
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
                if(track.IsFavorite)
                    Favorites.Add(track);
            }
            
        }

        public void DblClickTrack()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When an user click on an element, execute an action
        /// </summary>
        /// <param name="element"></param>
        public void ClickElement(object element)
        {
            MainWindowViewModel.Main.IsFlyoutMusicOpen = false;
            if (element is Track)
            {
                Helper.Context.PlayNewSong((Track) element);
                MainWindowViewModel.Main.ReadingList.Clear();
                MainWindowViewModel.Main.ReadingList.AddRang(Tracks.ToList());
                MainWindowViewModel.Main.IsFlyoutRunningOpen = true;
            }
            else if (element is Album)
            {
                MainWindowViewModel.Main.IsFlyoutMusicOpen = true;
                var view = new TrackListView
                {
                    DataContext = new MusicViewModel(new ObservableCollection<Track>(((Album) element).Tracks))
                };
                ((MusicViewModel) MainWindowViewModel.Main.ActualView.DataContext).MusicFlyoutView = view;
                ((MusicViewModel) MainWindowViewModel.Main.ActualView.DataContext).Type = "Morceaux";
            }
            else if (element is Artist)
            {
                MainWindowViewModel.Main.IsFlyoutMusicOpen = true;
                MusicFlyoutView = new AlbumListView();
                ((AlbumListView)MusicFlyoutView).DataContext = new MusicViewModel(new ObservableCollection<Album>(((Artist)element).Albums));
                Type = "Albums";
            }
            else if(element is Genre)
            {
                MainWindowViewModel.Main.IsFlyoutMusicOpen = true;
                MusicFlyoutView = new TrackListView();
                ((TrackListView)MusicFlyoutView).DataContext = new MusicViewModel(new ObservableCollection<Track>(((Genre)element).Tracks));
                Type = "Morceaux";
            }

            //remove the selection of the item selected
            Task.Run(() => {Thread.Sleep(10); SelectedItem = null; });
            
        }

        private void OpenFlyoutMusic()
        {
            throw new NotImplementedException();
        }

        private  object _musicFlyoutView;
        public object MusicFlyoutView
        {
            get { return _musicFlyoutView; }
            set
            {
                _musicFlyoutView = value;
                RaisePropertyChanged();
            }
        }

        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged();
            }
        }

        

        private List<ContextMenu> _contextMenu= new List<ContextMenu>();
        public List<ContextMenu> ContextMenu
        {
            get { return _contextMenu; }
            set
            {
                _contextMenu = value;
                RaisePropertyChanged();
            }
        }

        private Track _rightClickedItem;

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if(_isRightClick)
                    RightClick(_selectedItem);

                if(_selectedItem != null && !(SelectedItem is Track))
                    ClickElement(_selectedItem);
                RaisePropertyChanged();
            }
        }

    }
}
