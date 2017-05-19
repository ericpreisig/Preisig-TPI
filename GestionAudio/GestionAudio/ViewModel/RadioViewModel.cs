using BLL;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;
using Presentation.Helper;

namespace Presentation.ViewModel
{
    public class RadioViewModel : MainViewModel
    {
        #region Private Fields

        private bool _isRightClick;
        private Radio _rightClickedItem;
        private Radio _selectedItem;

        #endregion Private Fields

        #region Public Constructors

        public RadioViewModel()
        {
            OnClickElement = new RelayCommand(ClickRadio);
            OnRightClickRadio = new RelayCommand(RightClickRadio);
            Task.Run(() =>
            {
                //Get top 500 radios
                var radios = RadioData.GetRadioTop500Radios();
                Application.Current.Dispatcher.Invoke(() => Radios.AddRang(radios));
            });
            SetFavorite();
            SetLastRadios();
        }



        public RadioViewModel(List<Radio> radios)
        {
            OnClickElement = new RelayCommand(ClickRadio);
            OnRightClickRadio = new RelayCommand(RightClickRadio);
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() => Radios.AddRang(radios));
            });
        }

        #endregion Public Constructors

            #region Public Properties

        public ObservableCollection<Radio> Favorites { get; set; } = new ObservableCollection<Radio>();
        public ObservableCollection<Radio> LastRadios { get; set; } = new ObservableCollection<Radio>();
        public RelayCommand OnClickElement { get; set; }
        public RelayCommand OnRightClickRadio { get; set; }
        public ObservableCollection<Radio> Radios { get; set; } = new ObservableCollection<Radio>();

        public Radio SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_isRightClick)
                    RightClick(_selectedItem);

                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// When the user click on a radio
        /// </summary>
        public void ClickRadio()
        {
            SelectedItem.File = null;
            var radioWithPath = RadioData.SetRadioPath(SelectedItem);
            Helper.Context.PlayNewRadio(radioWithPath);
            MainWindowViewModel.Main.ReadingList.Clear();
            MainWindowViewModel.Main.IsFlyoutRunningOpen = true;

            //if the radio worked
            if (MusicPlayer.Player.PlaybackState == PlaybackState.Playing)
            {
                radioWithPath.AddRadioToRecent();
                SetLastRadios();
            }
        }

        public void DblClickRadio()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the list of all favorites radios
        /// </summary>
        public void SetFavorite()
        {
            Favorites.Clear();
            Favorites.AddRang(RadioData.GetFavouriteRadios());
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// When the user right click, set the rightclicked object and remove the selection
        /// </summary>
        /// <param name="radio"></param>
        private void RightClick(Radio radio)
        {
            _rightClickedItem = radio;

            _isRightClick = false;

            //Remove the selection
            Task.Run(() => { Thread.Sleep(10); SelectedItem = null; });
        }

        /// <summary>
        /// When the user right click, set the rightclick variable on
        /// </summary>
        private void RightClickRadio()
        {
            _isRightClick = true;
        }

        /// <summary>
        /// Set all the 10 recent radio
        /// </summary>
        private void SetLastRadios()
        {
            LastRadios.Clear();
            LastRadios.AddRang(RadioData.Get10LastRadios());        
        }
        #endregion Private Methods
    }
}