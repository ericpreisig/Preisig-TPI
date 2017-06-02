/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Execute action triggered by the big and small player
*********************************************************************************/

using DTO.Entity;
using NAudio.Wave;
using Presentation.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Presentation.Helper
{
    /// <summary>
    /// Handle all action made by the small and big player, it can interact with the view and/or take user inputs
    /// </summary>
    public static class MusicPlayer
    {
        #region Public Fields

        public static readonly WaveOutEvent Player = new WaveOutEvent();

        #endregion Public Fields

        #region Private Fields

        private static Thread _eachSecond;
        private static WaveStream _playerStream;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Chnage the actual time of the music
        /// </summary>
        /// <param name="timeInPourcent"></param>
        public static void ChangeTime(int timeInPourcent)
        {
            if (Context.ActualContext.Track == null) return;
            var newPosition = (long)(1.0 * Context.ActualContext.Track.Duration * (timeInPourcent * 1.0 / 100));
            _playerStream.CurrentTime = TimeSpan.FromMilliseconds(newPosition);
        }

        /// <summary>
        /// Update the music timers
        /// </summary>
        public static void EachSecondAction()
        {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                try
                {
                    if (Context.ActualContext.Track == null) break;
                    Context.ActualContext.ActualTime = (int)_playerStream.CurrentTime.TotalMilliseconds;
                    MainWindowViewModel.PlayerViewModel.ChangeTimeToSlider((int)_playerStream.CurrentTime.TotalMilliseconds, Context.ActualContext.Track.Duration);
                    Thread.Sleep(100);

                    //go to next music if ended
                    if ((int)_playerStream.CurrentTime.TotalMilliseconds >= Context.ActualContext.Track.Duration)
                        Application.Current.Dispatcher.Invoke(Next);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //Error the this thread wasn't updated syncronisously with the main thread
                }
            }
        }

        /// <summary>
        /// Play imediatly a music
        /// </summary>
        public static void NewPlay()
        {
            if (Context.ActualContext.ActualAudio == null) return;
            Player.Stop();
            _playerStream = Context.ActualContext.ActualAudio.File;
            _playerStream.Position = 0;
            WaveChannel32 volumeStream = new WaveChannel32(_playerStream);
            Player.Init(volumeStream);
            Play();
            MainWindowViewModel.PlayerViewModel.VolumeValue = Player.Volume;
            MainWindowViewModel.PlayerViewModel.InitPlayer(Context.ActualContext.ActualAudio);
        }

        /// <summary>
        /// Change to the next song
        /// </summary>
        public static void Next()
        {
            if (Context.ActualContext.Track == null) return;
            Pause();

            Track newTrackToPlay;

            //if random enable
            if (Context.ActualContext.IsRandom)
            {
                //Take a random number between all the tracks in the reading list
                var rand = new Random();
                var randomIndex = rand.Next(0, MainWindowViewModel.Main.ReadingList.Count);
                newTrackToPlay = MainWindowViewModel.Main.ReadingList.ElementAt(randomIndex);
            }
            else
            {
                //Or just take the next one
                newTrackToPlay = MainWindowViewModel.Main.ReadingList.SkipWhile(a => a != Context.ActualContext.Track).Skip(1).FirstOrDefault();
            }

            //1 means lopping the reading list, 2 means repeat one
            switch (Context.ActualContext.IsLooping)
            {
                case 1:
                    if (newTrackToPlay == null)
                        newTrackToPlay = MainWindowViewModel.Main.ReadingList.FirstOrDefault();
                    break;

                case 2:
                    newTrackToPlay = Context.ActualContext.Track;
                    break;
            }

            if (newTrackToPlay == null)
                return;

            Context.ActualContext.Track = newTrackToPlay;
            NewPlay();
        }

        /// <summary>
        /// Pause the current music
        /// </summary>
        public static void Pause()
        {
            if (Context.ActualContext.ActualAudio == null) return;
            Context.ActualContext.IsMusicPlaying = false;
            Player.Pause();
            _eachSecond.Abort();
            MainWindowViewModel.PlayerViewModel.PlaylerStatus = true;
            MainWindowViewModel.Main.ReadingList = new ObservableCollection<Track>(MainWindowViewModel.Main.ReadingList);
        }

        /// <summary>
        /// Play actual music the music
        /// </summary>
        public static void Play()
        {
            if (Context.ActualContext.ActualAudio == null) return;
            Context.ActualContext.IsMusicPlaying = true;
            Player.Play();
            _eachSecond = new Thread(EachSecondAction);
            _eachSecond.Start();
            MainWindowViewModel.PlayerViewModel.PlaylerStatus = false;
            MainWindowViewModel.Main.ReadingList = new ObservableCollection<Track>(MainWindowViewModel.Main.ReadingList);
        }

        /// <summary>
        /// Change to the previous song
        /// </summary>
        public static void Previous()
        {
            if (Context.ActualContext.Track == null) return;
            Pause();
            var listTrackReversed = MainWindowViewModel.Main.ReadingList.ToList();
            listTrackReversed.Reverse();

            var newTrackToPlay = listTrackReversed.SkipWhile(a => a != Context.ActualContext.Track).Skip(1).FirstOrDefault();

            if (newTrackToPlay == null) return;
            Context.ActualContext.Track = newTrackToPlay;
            NewPlay();
        }

        #endregion Public Methods
    }
}