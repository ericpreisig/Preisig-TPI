using DTO.Entity;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Presentation.Converter
{

    /// <summary>
    /// Converter used for the readingListView
    /// </summary>
    public class IsPlayingImage : IValueConverter
    {
        #region Public Methods

        /// <summary>
        /// Display a picture of play if the music is acually playing
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Track) || Helper.Context.ActualContext.Track != (Track)value) return null;

            var file = Helper.Context.ActualContext.IsMusicPlaying ? @"Resources/Images/play.png" : @"Resources/Images/pause.png";
            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = fileStream;
            img.EndInit();
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }

        #endregion Public Methods
    }
}