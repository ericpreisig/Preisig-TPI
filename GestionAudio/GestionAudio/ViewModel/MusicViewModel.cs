using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL;
using DTO.Entity;
using Presentation.View.List;

namespace Presentation.ViewModel
{
    public class MusicViewModel : MainViewModel
    {

        public ObservableCollection<Track> Tracks { get; set; }
        public ObservableCollection<Artist> Artists { get; set; }
        public ObservableCollection<Album> Albums { get; set; }
        public ObservableCollection<Genre> Genres { get; set; }
        public ObservableCollection<Track> Favorites { get; set; } = new ObservableCollection<Track>();

        public MusicViewModel()
        {
            Tracks = new ObservableCollection<Track>(TrackData.GetTracks());
            Artists = new ObservableCollection<Artist>(ArtistData.GetArtists());
            Albums = new ObservableCollection<Album>(AlbumData.GetAlbums());
            Genres = new ObservableCollection<Genre>(GeneralData.GetGenres());
            SetFavorite();
        }

        public MusicViewModel(ObservableCollection<Track> tracks)
        {
            Tracks = new ObservableCollection<Track>(tracks);
        }

        public MusicViewModel(ObservableCollection<Album> albums)
        {
            Albums = new ObservableCollection<Album>(albums);
        }

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
                MainWindowViewModel.ReadingList=new ObservableCollection<Track>(Tracks.ToList());
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

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_selectedItem!=null)
                    ClickElement(_selectedItem);
                RaisePropertyChanged();
            }
        }

    }
}
