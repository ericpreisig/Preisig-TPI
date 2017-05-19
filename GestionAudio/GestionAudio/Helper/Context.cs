using BLL;
using DTO.Entity;
using Presentation.ViewModel;
using System;
using System.Linq;

namespace Presentation.Helper
{
    public static class Context
    {
        #region Public Fields

        public static DTO.Entity.Context ActualContext = GeneralData.GetContext();

        #endregion Public Fields

        #region Public Methods

        public static void AddToReadingList(Track track)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clear the reading list and play a new song
        /// </summary>
        /// <param name="radio"></param>
        public static void PlayNewRadio(Radio radio)
        {
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
            Shared.MusicFile.StopRadio();
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

        public static void SaveConext()
        {
            throw new NotImplementedException();
        }

        public static void SetConext()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}