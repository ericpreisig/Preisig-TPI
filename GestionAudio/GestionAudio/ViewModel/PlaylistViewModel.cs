/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : All action 
*********************************************************************************/

using BLL;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.ViewModel
{
    /// <summary>
    /// This class contain the logic for the playlistview
    /// </summary>
    public class PlaylistViewModel : MainViewModel
    {
        #region Private Fields

        private MusicViewModel _musicViewModel;
        private Playlist _selectedItem;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Set playlists
        /// </summary>
        public PlaylistViewModel()
        {
            Playlists = new ObservableCollection<Playlist>(PlaylistData.GetPlaylists());
            if (Playlists.Any())
                SelectedItem = Playlists.FirstOrDefault();

            OnClickAdd = new RelayCommand(async () => await ClickAdd());
            OnClickRemove = new RelayCommand(ClickRemove);
        }

        #endregion Public Constructors

        #region Public Properties

        public MusicViewModel MusicViewModel
        {
            get { return _musicViewModel; }
            set
            {
                _musicViewModel = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OnClickAdd { get; set; }
        public RelayCommand OnClickRemove { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }

        public Playlist SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                MusicViewModel = new MusicViewModel(new ObservableCollection<Track>(_selectedItem.Tracks.ToList()), _selectedItem);

                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// When the user click to add a playlist
        /// </summary>
        public async Task<Playlist> ClickAdd()
        {
            Tuple<string, bool> text = await GeneralHelper.ShowPrompt("Playlist", "Veuillez nommer cette playlist", MessageDialogStyle.AffirmativeAndNegative);

            if (!text.Item2) return null;
            var newPlaylist = new Playlist { Name = text.Item1 };
            PlaylistData.CreatePlaylist(newPlaylist);
            Playlists.Clear();
            Playlists.AddRang(PlaylistData.GetPlaylists());
            return newPlaylist;
        }

        /// <summary>
        /// When the user click to remove a playlist
        /// </summary>
        public void ClickRemove()
        {
            if (SelectedItem == null) return;
            PlaylistData.RemovePlaylist(SelectedItem);
            Playlists.Remove(SelectedItem);
        }

        /// <summary>
        /// Update all the playlists elements
        /// </summary>
        public void UpdatePlaylist()
        {
            var selectedItemId = _selectedItem.ID;
            Playlists.Clear();
            Playlists.AddRang(PlaylistData.GetPlaylists());
            SelectedItem = Playlists.FirstOrDefault(a => a.ID == selectedItemId);
        }

        #endregion Public Methods
    }
}