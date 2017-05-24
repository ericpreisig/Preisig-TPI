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
using MahApps.Metro.Controls.Dialogs;
using NAudio.Wave;
using Presentation.Helper;
using Presentation.View;

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

        public RadioViewModel(bool instantLoadRadio=false)
        {
            OnClickElement = new RelayCommand(ClickRadio);
            OnRightClickRadio = new RelayCommand(RightClickRadio);

            //if I only want to play a radio, return
            if (instantLoadRadio) return;
            SetFavorite();
            SetLastRadios();
            LoadRadio();
        }

        /// <summary>
        /// Get and populate the top 500 radios
        /// </summary>
        private async void LoadRadio()
        {
            //Get top 500 radios         
            var radioMessage =await MainWindowViewModel.MetroWindow.ShowProgressAsync("Radio", "Accès à Shoutcast en cours... Veuillez patienter");
            await Task.Run(() =>
            {
                try
                {
                    var radios = RadioData.GetRadioTop500Radios();
                    Application.Current.Dispatcher.Invoke(() => Radios.AddRang(radios));
                }
                catch (Exception e)
                {
                    GeneralHelper.ShowMessage("Erreur", "Accès à Shoutcast impossible", MessageDialogStyle.Affirmative);
                }
                radioMessage.CloseAsync();


            });
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
        public async void ClickRadio()
        {
            if(SelectedItem==null) return;
            SelectedItem.File = null;
            var accessRadioMessage = await MainWindowViewModel.MetroWindow.ShowProgressAsync("Radio", "Accès à la radio en cours... Veuillez patienter");
            await Task.Run(() =>
            {
                try
                {
                    var radioWithPath = RadioData.SetRadioPath(SelectedItem);
                    Helper.Context.PlayNewRadio(radioWithPath.AddRadioToRecent());
                    Application.Current.Dispatcher.Invoke(() => MainWindowViewModel.Main.ReadingList.Clear());

                    MainWindowViewModel.Main.IsFlyoutRunningOpen = true;
                    Application.Current.Dispatcher.Invoke(SetLastRadios);                   
                }
                catch (Exception e)
                {
                    GeneralHelper.ShowMessage("Erreur", "Accès à Shoutcast impossible", MessageDialogStyle.Affirmative);
                }
                accessRadioMessage.CloseAsync();

            });
            await Task.Run(() => { Thread.Sleep(30); SelectedItem = null; });

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