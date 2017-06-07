/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Take every action from the small and big player
*********************************************************************************/

using BLL;
using DTO;
using DTO.Entity;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NAudio.Wave;
using Presentation.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    /// <summary>
    /// This class contain the logic for the small and big player
    /// </summary>
    public class PlayerViewModel : ViewModelBase
    {
        #region Private Fields

        private TimeSpan _actualTime;
        private Audio _audio;
        private int _isLooping;
        private bool _isMouseHolded;
        private double _oldValue;
        private bool _playlerStatus;
        private double _sliderValue;

        #endregion Private Fields

        #region Public Constructors

        public PlayerViewModel()
        {
            OnClickPlay = new RelayCommand(ClickPlay);
            OnMousedownRewind = new RelayCommand(MousedownRewind);
            OnMousedownForward = new RelayCommand(MousedownForward);
            OnClickPicture = new RelayCommand(ClickPicture);
            OnClickLoopReadingList = new RelayCommand(ClickLoopReadingList);
            OnClickFavorite = new RelayCommand(ClickFavorite);
            OnClickRandom = new RelayCommand(ClickRandom);
            OnClickReadingList = new RelayCommand(ClickReadingList);
        }

        #endregion Public Constructors

        #region Public Properties

        private Visibility _isRadio = Visibility.Visible;
        private bool _isRandom;

        private float _volumeValue;

        public TimeSpan ActualTime
        {
            get { return _actualTime; }
            set
            {
                _actualTime = value;
                RaisePropertyChanged();
            }
        }

        public Audio Audio
        {
            get { return _audio; }
            set
            {
                _audio = value;
                RaisePropertyChanged();
            }
        }

        public int IsLooping
        {
            get { return _isLooping; }
            set
            {
                _isLooping = value;
                RaisePropertyChanged();
            }
        }

        public Visibility IsRadio
        {
            get { return _isRadio; }
            set
            {
                _isRadio = value;
                RaisePropertyChanged();
            }
        }

        public bool IsRandom
        {
            get { return _isRandom; }
            set
            {
                _isRandom = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OnClickFavorite { get; set; }
        public RelayCommand OnClickLoopReadingList { get; set; }
        public RelayCommand OnClickPicture { get; set; }
        public RelayCommand OnClickPlay { get; set; }
        public RelayCommand OnClickRandom { get; set; }
        public RelayCommand OnClickReadingList { get; set; }
        public RelayCommand OnMousedownForward { get; set; }
        public RelayCommand OnMousedownRewind { get; set; }

        public bool PlaylerStatus
        {
            get { return _playlerStatus; }
            set
            {
                _playlerStatus = value;
                RaisePropertyChanged();
            }
        }

        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                //it means the user change the time
                if (Math.Abs(_sliderValue - _oldValue) > 1)
                {
                    MusicPlayer.ChangeTime((int)Math.Round(_sliderValue));
                }

                RaisePropertyChanged();
            }
        }

        public float VolumeValue
        {
            get { return _volumeValue; }
            set
            {
                _volumeValue = value;
                MusicPlayer.Player.Volume = _volumeValue;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Change the time of the slider
        /// </summary>
        /// <param name="milisecond"></param>
        /// <param name="total"></param>
        public void ChangeTimeToSlider(int milisecond, int total)
        {
            ActualTime = TimeSpan.FromMilliseconds(milisecond);
            SliderValue = _oldValue = 1.0 * milisecond / total * 100;
        }

        /// <summary>
        /// When the user click on the favorite/unfavorite logo
        /// </summary>
        public void ClickFavorite()
        {
            if (Audio.IsFavorite)
                FavoriteData.RemoveFavorite(Audio);
            else
                FavoriteData.AddFavorite(Audio);
            RaisePropertyChanged("Audio");

            //Uppdate Favorite list
            if (MainWindowViewModel.Main.ActualView.DataContext is MusicViewModel)
                ((MusicViewModel)MainWindowViewModel.Main.ActualView.DataContext).SetFavorite();

            if (MainWindowViewModel.Main.ActualView.DataContext is RadioViewModel)
                ((RadioViewModel)MainWindowViewModel.Main.ActualView.DataContext).SetFavorite();
        }

        public void ClickForward()
        {
            MusicPlayer.Next();
        }

        /// <summary>
        /// change the lecture type (straight -> continous)
        /// </summary>
        public void ClickLoopReadingList()
        {
            Helper.Context.ActualContext.IsLooping = IsLooping = GetNextLoopinhAction();
        }

        /// <summary>
        /// When the user click on the picture, open the running flyout
        /// </summary>
        public void ClickPicture()
        {
            MainWindowViewModel.Main.IsFlyoutRunningOpen = true;
        }

        /// <summary>
        /// Play the music
        /// </summary>
        public void ClickPlay()
        {
            if (MusicPlayer.Player.PlaybackState == PlaybackState.Playing)
            {
                MusicPlayer.Pause();
            }
            else
            {
                MusicPlayer.Play();
            }
        }

        /// <summary>
        /// When the user click on the random button
        /// </summary>
        public void ClickRandom()
        {
            Helper.Context.ActualContext.IsRandom = IsRandom = !IsRandom;
        }

        /// <summary>
        /// Show the reading list
        /// </summary>
        public void ClickReadingList()
        {
            MainWindowViewModel.Main.IsFlyoutReadingListOpen = true;
        }

        /// <summary>
        /// Go to previous music
        /// </summary>
        public void ClickRewind()
        {
            MusicPlayer.Previous();
        }

        /// <summary>
        /// Set up a track
        /// </summary>
        /// <param name="audio"></param>
        public void InitPlayer(Audio audio)
        {
            IsRadio = audio is Track ? Visibility.Visible : Visibility.Collapsed;
            Audio = audio;
        }

        /// <summary>
        /// When the user hold down the forward button
        /// </summary>
        public void MousedownForward()
        {
            Task.Run(() => MovePosition(3, ClickForward));
        }

        /// <summary>
        /// When the user hold down the rewind button
        /// </summary>
        public void MousedownRewind()
        {
            Task.Run(() => MovePosition(-3, ClickRewind));
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get the next icon to show (continous, loop, or only one song)
        /// </summary>
        /// <returns></returns>
        private int GetNextLoopinhAction()
        {
            var newLoopingAction = IsLooping + 1;
            if (newLoopingAction >= 3)
                newLoopingAction = 0;
            return newLoopingAction;
        }

        /// <summary>
        /// Move the position of the cursor on the slide bar
        /// </summary>
        /// <param name="changeValue"></param>
        /// <param name="actionOnClick"></param>
        private void MovePosition(int changeValue, Action actionOnClick)
        {
            _isMouseHolded = true;

            //wait that it's actually a true holding
            Thread.Sleep(150);

            //if it was a click, stop it there
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Mouse.LeftButton == MouseButtonState.Released)
                    actionOnClick();
            });

            //Check if still holding
            while (_isMouseHolded)
            {
                Thread.Sleep(100);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.SetCursor(Cursors.Hand);
                    if (Mouse.LeftButton != MouseButtonState.Released)
                    {
                        SliderValue += changeValue;
                        return;
                    }

                    Mouse.SetCursor(Cursors.AppStarting);
                    _isMouseHolded = false;
                });
            }
        }

        #endregion Private Methods
    }
}