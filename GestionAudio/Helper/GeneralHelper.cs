/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : General class that can be used by every one, 
*  it can show messages and has an implementation of add range 
*  for observablecollections
*********************************************************************************/

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Shared
{
    /// <summary>
    /// general helper that can be used by any project
    /// </summary>
    public static class GeneralHelper
    {
        #region Public Methods

        /// <summary>
        /// Allow to add rang in a observablecollection #not thread safe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="addedCollection"></param>
        public static void AddRang<T>(this ObservableCollection<T> collection, List<T> addedCollection)
        {
            foreach (var item in addedCollection)
                collection.Add(item);
        }

        /// <summary>
        /// Show a message
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="dialogStyle"></param>
        /// <returns></returns>
        public static Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle dialogStyle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var metroWindow = (Application.Current.MainWindow as MetroWindow);
                metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;
                return metroWindow.ShowMessageAsync(title, message, dialogStyle, metroWindow.MetroDialogOptions);
            });
            return null;
        }

        /// <summary>
        /// Show a prompt
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="dialogStyle"></param>
        /// <returns></returns>
        public static async Task<Tuple<string, bool>> ShowPrompt(string title, string message, MessageDialogStyle dialogStyle)
        {
            return await Application.Current.Dispatcher.Invoke(async () =>
             {
                 while (true)
                 {
                     var metroWindow = (Application.Current.MainWindow as MetroWindow);
                     metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;
                     metroWindow.MetroDialogOptions.AffirmativeButtonText = "OK";
                     metroWindow.MetroDialogOptions.NegativeButtonText = "Annuler";
                     var text = await metroWindow.ShowInputAsync(title, message, metroWindow.MetroDialogOptions);
                     return string.IsNullOrWhiteSpace(text) ? new Tuple<string, bool>(text, false) : new Tuple<string, bool>(text, true);
                 }
             });
        }

        #endregion Public Methods
    }
}