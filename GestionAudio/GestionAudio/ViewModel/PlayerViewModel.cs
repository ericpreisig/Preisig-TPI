using BLL;
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
using Presentation.View;

namespace Presentation.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        #region Private Fields

        private TimeSpan _actualTime;
        private int _isLooping;
        private bool _isMouseHolded = false;
        private double _oldValue;
        private bool _playlerStatus;
        private double _sliderValuerack;
        private Track _track;

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

        public TimeSpan ActualTime
        {
            get { return _actualTime; }
            set
            {
                _actualTime = value;
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

        public RelayCommand OnClickFavorite { get; set; }
        public RelayCommand OnClickLoopReadingList { get; set; }
        public RelayCommand OnClickPicture { get; set; }
        public RelayCommand OnClickPlay { get; set; }
        public RelayCommand OnMousedownForward { get; set; }
        public RelayCommand OnMousedownRewind { get; set; }
        public RelayCommand OnClickRandom { get; set; }
        public RelayCommand OnClickReadingList { get; set; }

        public bool PlaylerStatus
        {
            get { return _playlerStatus; }
            set
            {
                _playlerStatus = value;
                RaisePropertyChanged();
            }
        }

        private bool _isRandom;
        public bool IsRandom
        {
            get { return _isRandom; }
            set
            {
                _isRandom = value;
                RaisePropertyChanged();
            }
        }

        public double SliderValue
        {
            get { return _sliderValuerack; }
            set
            {
                _sliderValuerack = value;
                //it means the user chnage the time
                if (Math.Abs(_sliderValuerack - _oldValue) > 1)
                {
                    MusicPlayer.ChangeTime((int)Math.Round(_sliderValuerack));
                }

                RaisePropertyChanged();
            }
        }

        public Track Track
        {
            get { return _track; }
            set
            {
                _track = value;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

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
            if (Track.IsFavorite)
                FavoriteData.RemoveFavorite(Track);
            else
                FavoriteData.AddFavorite(Track);
            RaisePropertyChanged("Track");

            //Uppdate FavoritList
            if (MainWindowViewModel.Main.ActualView.DataContext is MusicViewModel)
                ((MusicViewModel) MainWindowViewModel.Main.ActualView.DataContext).SetFavorite();

            //Track = TrackData.GetTrackByPath(Track.Path);
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

        public void ClickPicture()
        {
            MainWindowViewModel.Main.IsFlyoutRunningOpen = true;
        }

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
           Helper.Context.ActualContext.IsRandom= IsRandom = !IsRandom;
        }

        public void ClickReadingList()
        {
            MainWindowViewModel.Main.IsFlyoutReadingListOpen = true;
        }

        public void ClickRewind()
        {
            MusicPlayer.Previous();
        }

        public void InitTrack(Track track)
        {
            Track = track;
        }

        public void MousedownForward()
        {
            Task.Run(() => MovePosition(3, ClickForward));
        }

        public void MousedownRewind()
        {
            Task.Run(() => MovePosition(-3, ClickRewind));
        }

        #endregion Public Methods

        #region Private Methods

        private int GetNextLoopinhAction()
        {
            var newLoopingAction = IsLooping + 1;
            if (newLoopingAction >= 3)
                newLoopingAction = 0;
            return newLoopingAction;
        }

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