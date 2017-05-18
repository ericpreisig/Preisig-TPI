using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using DTO.Entity;
using ManagedBass;
using Presentation.ViewModel;
using Shared;

namespace Presentation.Helper
{
    public static class MusicPlayer
    {
        private static int _playerStream;
        static Thread _eachSecond;

        /// <summary>
        /// Play imediatly a music
        /// </summary>
        public static void NewPlay()
        {
            if (Context.ActualContext.ActualAudio == null) return;
            Bass.Stop();
            _playerStream = Context.ActualContext.ActualAudio.File;
            Bass.Init();
            Play();
            MainWindowViewModel.PlayerViewModel.InitPlayer(Context.ActualContext.ActualAudio);
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
                    MainWindowViewModel.PlayerViewModel.ChangeTimeToSlider((int)Bass.ChannelGetPosition(_playerStream), Context.ActualContext.Track.Duration);
                    Thread.Sleep(100);

                    //go to next music if ended
                    if ((int)Bass.ChannelGetPosition(_playerStream) >= Context.ActualContext.Track.Duration)
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
        /// Play actual music the music
        /// </summary>
        public static void Play()
        {
            if (Context.ActualContext.ActualAudio == null)  return;
            Bass.ChannelPlay(Context.ActualContext.ActualAudio.File);
            _eachSecond = new Thread(EachSecondAction);
            _eachSecond.Start();
            MainWindowViewModel.PlayerViewModel.PlaylerStatus = false;
            MainWindowViewModel.Main.ReadingList = new ObservableCollection<Track>(MainWindowViewModel.Main.ReadingList);

        }

        /// <summary>
        /// Pause the current music
        /// </summary>
        public static void Pause()
        {
            if (Context.ActualContext.ActualAudio == null) return;
            Context.ActualContext.IsMusicPlaying = false;
            Bass.ChannelPause(Context.ActualContext.ActualAudio.File);
            _eachSecond.Abort();
            MainWindowViewModel.PlayerViewModel.PlaylerStatus = true;
            MainWindowViewModel.Main.ReadingList= new ObservableCollection<Track>(MainWindowViewModel.Main.ReadingList);
        }

        /// <summary>
        /// Change to the next song
        /// </summary>
        public static void Next()
        {
            if (Context.ActualContext.Track == null) return;
            Pause();

            Track newTrackToPlay;

            //îf random enable
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

            switch (Context.ActualContext.IsLooping)
            {
                case 1: //if the user is on loop mode, take the first music back and last track of the readinfg list
                    if(newTrackToPlay == null)
                        newTrackToPlay = MainWindowViewModel.Main.ReadingList.FirstOrDefault();
                    break;
                case 2://if the user is on one only mode, take the same
                    newTrackToPlay = Context.ActualContext.Track;
                    break;
            }
            
            Context.ActualContext.Track = newTrackToPlay;
            NewPlay();
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

        /// <summary>
        /// Chnage the actual time of the music 
        /// </summary>
        /// <param name="timeInPourcent"></param>
        public static void ChangeTime(int timeInPourcent)
        {
            if (Context.ActualContext.Track == null) return;
            var newPosition = (long) (1.0 * Context.ActualContext.Track.Duration * (timeInPourcent * 1.0 / 100));
            Bass.ChannelSetPosition(_playerStream, newPosition);
        }


    }
}
