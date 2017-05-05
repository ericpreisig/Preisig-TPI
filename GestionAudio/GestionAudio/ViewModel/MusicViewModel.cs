using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ViewModel
{
    public class MusicViewModel : MainViewModel
    {
        private List<object> _elementMusicFlyout;
        public List<object> ElementMusicFlyout
        {
            get { return _elementMusicFlyout; }
            set
            {
                _elementMusicFlyout = value;
                RaisePropertyChanged();
            }
        }

        public MusicViewModel()
        {
            //set favortie and recent
        }

        public void DblClickTrack()
        {
            throw new NotImplementedException();
        }

        public void ClickElement()
        {
            throw new NotImplementedException();
        }

        private void FlyoutMusic()
        {
            throw new NotImplementedException();
        }
    }
}
