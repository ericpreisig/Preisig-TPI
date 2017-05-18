using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BLL;
using DTO.Entity;
using GalaSoft.MvvmLight.Command;
using Shared;

namespace Presentation.ViewModel
{
    public class RadioViewModel : MainViewModel
    {
        public ObservableCollection<Radio> Radios { get; set; }= new ObservableCollection<Radio>();
        public ObservableCollection<Radio> LastRadios { get; set; }= new ObservableCollection<Radio>();
        public RelayCommand OnRightClickRadio { get; set; }
        public RelayCommand OnClickElement { get; set; }


        public RadioViewModel()
        {
            OnClickElement = new RelayCommand(ClickRadio);
            OnRightClickRadio = new RelayCommand(RightClickRadio);
            Task.Run(() =>
            {
                //Get top 500 radios
                var radios = RadioData.GetRadioTop500Radios();
                Application.Current.Dispatcher.Invoke(() =>Radios.AddRang(radios));
            });

            //set favortie and recent

        }


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
        private bool _isRightClick;
        private Radio _rightClickedItem;

        public void ClickRadio()
        {
            var radioWithPath = RadioData.SetRadioPath(SelectedItem);
            Helper.Context.PlayNewRadio(radioWithPath);
            MainWindowViewModel.Main.ReadingList.Clear();
            MainWindowViewModel.Main.IsFlyoutRunningOpen = true;
        }

        public void DblClickRadio()
        {
            throw new NotImplementedException();
        }

        private Radio _selectedItem;
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
    }
}
