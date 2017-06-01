using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BLL;
using DTO.Entity;
using MahApps.Metro.Controls.Dialogs;
using Presentation.View;
using Shared;

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
            KeyWord = text;
            var numberTry = 0;
            
            //let 3 tries in case the thread is busy
            while (numberTry<3)
            {
                try
                {
                    SearchInMusic();
                    SearchInRadios();
                    return;
                }
                catch (Exception e)
                {
                    Thread.Sleep(100*numberTry);
                    numberTry++;
                }
            }

            //if the search failed
            Shared.GeneralHelper.ShowMessage("Erreur", "La recherche a échouée",MessageDialogStyle.Affirmative);
        }

        /// <summary>
        /// Search a keyword on all the tracks/artists/albums
        /// </summary>
        private void SearchInMusic()
        {
          
                MusicViewModel = new MusicViewModel(true);
                _allTracks = _allTracks ?? TrackData.GetTracks();
                _allAlbums = _allAlbums ?? _allTracks.Select(a => a.Album).Distinct().ToList();
                _allArtists = _allArtists ?? _allAlbums.Select(a => a.Artist).Distinct().ToList();
                MusicViewModel.Tracks.AddRang(_allTracks.Where(a => Look(a.Name) || a.Genres.Any(b => Look(b.Name) || Look(a.Album.Name) || Look(a.Album.Artist.Name))).ToList());
                MusicViewModel.Albums.AddRang(_allAlbums.Where(a => Look(a.Name)).ToList());
                MusicViewModel.Artists.AddRang(_allArtists.Where(a => Look(a.Name)).ToList());
              
           }

        /// <summary>
        /// empty all buffer list (fired after the sync)
        /// </summary>
        public static void EmptyBuffer()
        {
            _allTracks=null;
            _allArtists = null;
            _allAlbums = null;
        }

        /// <summary>
        /// Check if the is a string into a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool Look(string value) => value.ToLower().Contains(KeyWord.ToLower());

        private Thread _searchRadioThread;

        /// <summary>
        /// Search keyword on all radio
        /// </summary>
        private void SearchInRadios()
        {
            if (_searchRadioThread != null && _searchRadioThread.IsAlive)
                _searchRadioThread.Abort();

            //Trigger the search
            _searchRadioThread = new Thread(() =>
            {
                if (KeyWord.Length >= 3)
                    RadioViewModel = new RadioViewModel(RadioData.GetRadioByKeyWord(KeyWord.ToLower()));
            });
            _searchRadioThread.Start();
           
        }

    }
}
