using BLL;
using DTO.Entity;
using Presentation.ViewModel;
using System;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using Shared;

namespace Presentation.Helper
{
    /// <summary>
    /// Class the modifies the context, this class is called whenever a new music is clicked on. it's set the reading list too
    /// </summary>
    public static class Context
    {
        #region Public Fields

        public static DTO.Entity.Context ActualContext = GeneralData.LoadContext();

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Clear the reading list and play a new song
        /// </summary>
        /// <param name="radio"></param>
        public static void PlayNewRadio(Radio radio)
        {
            radio.File = null;
            if (radio.File == null)
            {
                MusicPlayer.Player.Stop();
                return;
            }
            ActualContext.Radio = radio;
            ActualContext.Track = null;
            MusicPlayer.NewPlay();
            MainWindowViewModel.Main.RaisePropertyChanged("ReadingList");
        }

        /// <summary>
        /// Clear the reading list and play a new song
        /// </summary>
        /// <param name="track"></param>
        public static void PlayNewSong(Track track)
        {
            //check that the file exist
            if (!File.Exists(track.Path))
            {
                GeneralHelper.ShowMessage("Erreur", "Le fichier est introuvable, il va donc être supprimé de la base de données",
                    MessageDialogStyle.Affirmative);
                
                track.RemoveTrack();
                MainWindowViewModel.Main.ReadingList.Remove(track);

                //Send a message to update lists from all MusicViewModels and ReadingList
                Messenger.Default.Send("UpdateList");

                return;
            }
            MusicFile.StopRadio();
            ActualContext.Track = track;
            ActualContext.Radio = null;
            MusicPlayer.NewPlay();
            MainWindowViewModel.Main.RaisePropertyChanged("ReadingList");
        }

        /// <summary>
        /// Removes a track from the reaing list if it's on there
        /// </summary>
        /// <param name="track"></param>
        public static void RemoveFromReadingList(Track track)
        {
            if (MainWindowViewModel.Main.ReadingList.Any(a => a == track))
            {
                MainWindowViewModel.Main.ReadingList.Remove(track);
            }
        }


        #endregion Public Methods
    }
}