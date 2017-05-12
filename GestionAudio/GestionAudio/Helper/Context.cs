using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using DTO;
using DTO.Entity;
using Presentation.ViewModel;

namespace Presentation.Helper
{
    public static class Context
    {
        public static DTO.Entity.Context ActualContext = GeneralData.GetContext();

        /// <summary>
        /// Clear the reading list and play a new song
        /// </summary>
        /// <param name="track"></param>
        public static void PlayNewSong(Track track)
        {
            ActualContext.Track = track;
            MusicPlayer.NewPlay();
        }

        public static void AddToReadingList(Track track)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a track from the reaing list if it's on there
        /// </summary>
        /// <param name="track"></param>
        public static void RemoveFromReadingList(Track track)
        {
            if (MainWindowViewModel.ReadingList.Any(a => a == track))
            {
                MainWindowViewModel.ReadingList.Remove(track);
            }
        }

        public static void SetConext()
        {
            throw new NotImplementedException();
        }

        public static void SaveConext()
        {
            throw new NotImplementedException();
        }
    }
}
