using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DTO.Entity;

namespace Presentation.Converter
{
    public class IsPlayingImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Track) || Helper.Context.ActualContext.Track != (Track) value) return null;
             
            var file = Helper.Context.ActualContext.IsMusicPlaying ? @"Resources/Images/play.png": @"Resources/Images/pause.png";
            FileStream fileStream =new FileStream(file, FileMode.Open, FileAccess.Read);
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
    }
}
