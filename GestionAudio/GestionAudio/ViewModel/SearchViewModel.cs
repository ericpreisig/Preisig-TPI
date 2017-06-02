/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Search elements by keyword
*********************************************************************************/

using System;
using BLL;
using DTO.Entity;
using MahApps.Metro.Controls.Dialogs;
using Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Presentation.ViewModel
{
    /// <summary>
    /// This class contain the logic for the searchview
    /// </summary>
    public class SearchViewModel : MainViewModel
    {
        #region Private Fields

        private static List<Album> _allAlbums;

        private static List<Artist> _allArtists;

        private static List<Track> _allTracks;

        private static List<Genre> _allGenres;

        private MusicViewModel _musicViewModel;

        private RadioViewModel _radioViewModel1;

        private Thread _searchRadioThread;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Launch the search in radios and tracks
        /// </summary>
        /// <param name="text"></param>
        public SearchViewModel(string text)
        {
            KeyWord = text??"";
            var numberTry = 0;

            //let 3 tries in case the thread is busy
            while (numberTry < 3)
            {
                try
                {
                    SearchInMusic();
                    SearchInRadios();
                    return;
                }
                catch(Exception)
                {
                    Thread.Sleep(100 * numberTry);
                    numberTry++;
                }
            }

            //if the search failed
            Shared.GeneralHelper.ShowMessage("Erreur", "La recherche a échouée", MessageDialogStyle.Affirmative);
        }

        #endregion Public Constructors

        #region Public Properties

        public string KeyWord { get; set; }

        public MusicViewModel MusicViewModel
        {
            get { return _musicViewModel; }
            set
            {
                _musicViewModel = value;
                RaisePropertyChanged();
            }
        }

        public RadioViewModel RadioViewModel
        {
            get { return _radioViewModel1; }
            set
            {
                _radioViewModel1 = value;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// empty all buffer list (fired after the sync)
        /// </summary>
        public static void EmptyBuffer()
        {
            _allTracks = null;
            _allArtists = null;
            _allAlbums = null;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Check if the is a string into a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool Look(string value) => value.ToLower().Contains(KeyWord.ToLower());

        /// <summary>
        /// Search a keyword on all the tracks/artists/albums
        /// </summary>
        private void SearchInMusic()
        {
            MusicViewModel = new MusicViewModel(true);
            _allTracks = _allTracks ?? TrackData.GetTracks();
            _allAlbums = _allAlbums ?? AlbumData.GetAlbums();
            _allArtists = _allArtists ?? ArtistData.GetArtists();
            _allGenres = _allGenres ?? GeneralData.GetGenres();

            MusicViewModel.Tracks.AddRang(_allTracks.Where(a => Look(a.Name)|| Look(a.Album.Name) || Look(a.Album.Artist.Name)).ToList());
            MusicViewModel.Albums.AddRang(_allAlbums.Where(a => Look(a.Name)).ToList());
            MusicViewModel.Artists.AddRang(_allArtists.Where(a => Look(a.Name)).ToList());

            //Search in genre without triggering the lazyloading
            var trackGenres = _allGenres.Where(a => Look(a.Name)).Select(a => a.Tracks).FirstOrDefault();
            if (trackGenres != null) MusicViewModel.Tracks.AddRang(trackGenres.ToList());
        }

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
                try
                {
                    if (KeyWord.Length >= 3)
                        RadioViewModel = new RadioViewModel(RadioData.GetRadioByKeyWord(KeyWord.ToLower()));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
              
            });
            _searchRadioThread.Start();
        }

        #endregion Private Methods
    }
}