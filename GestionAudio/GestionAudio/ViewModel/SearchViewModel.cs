using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL;
using DTO.Entity;

namespace Presentation.ViewModel
{
    public class SearchViewModel : MainViewModel
    {
        public MusicViewModel MusicViewModel
        {
            get { return _musicViewModel; }
            set
            {
                _musicViewModel = value;
                RaisePropertyChanged();
            }
        }

        private RadioViewModel _radioViewModel1;
        private MusicViewModel _musicViewModel;

        public RadioViewModel RadioViewModel
        {
            get { return _radioViewModel1; }
            set
            {
                _radioViewModel1 = value;
                RaisePropertyChanged();
            }
        }

        public string KeyWord { get; set; }


        private static List<Track> _allTracks; 
        private static List<Album> _allAlbums; 
        private static List<Artist> _allArtists; 

        public SearchViewModel(string text)
        {
            Thread.Sleep(10);
            KeyWord = text;
            SearchInMusic();
            SearchInRadios();
        }

        /// <summary>
        /// Search a keyword on all the tracks/artists/albums
        /// </summary>
        private void SearchInMusic()
        {
            Task.Run(() =>
            {
                MusicViewModel = new MusicViewModel(true);
                _allTracks = _allTracks ?? TrackData.GetTracks();
                _allAlbums = _allAlbums ?? _allTracks.Select(a => a.Album).Distinct().ToList();
                _allArtists = _allArtists ?? _allAlbums.Select(a => a.Artist).Distinct().ToList();
                MusicViewModel.Tracks = new ObservableCollection<Track>(_allTracks.Where(a => Look(a.Name) || Look(a.Genre.Name)).ToList());
                MusicViewModel.Albums = new ObservableCollection<Album>(_allAlbums.Where(a => Look(a.Name)).ToList());
                MusicViewModel.Artists = new ObservableCollection<Artist>(_allArtists.Where(a => Look(a.Name)).ToList());
            });
          
        }

        private bool Look(string value) => value.ToLower().Contains(KeyWord.ToLower());

        /// <summary>
        /// Search keyword on all radio
        /// </summary>
        private void SearchInRadios()
        {
            Task.Run(() =>
            {
                RadioViewModel = new RadioViewModel(RadioData.GetRadioByKeyWord(KeyWord.ToLower()));
            });
        }

    }
}
