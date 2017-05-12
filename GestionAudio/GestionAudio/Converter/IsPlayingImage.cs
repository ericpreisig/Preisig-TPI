using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DTO.Entity;

namespace Presentation.Converter
{
    public class IsPlayingImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Track)
            {
                
                if(Helper.Context.ActualContext.IsMusicPlaying && Helper.Context.ActualContext.Track==(Track)value)
                    return "../../Ressources/Images/play.png";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
