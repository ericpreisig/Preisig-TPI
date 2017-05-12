using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using NAudio.WindowsMediaFormat;

namespace Presentation.TrackInfoExtracter
{
    class WmvInfo
    {
        public Info GetInfo(string path)
        {
            var infos = new Info();
            using (var wmaStream = new WmaStream(path))
            {
                infos.Name = wmaStream["Title"];
                infos.Artist = wmaStream["Author"];
                infos.Album = wmaStream["Album"];
                infos.Genre = wmaStream["Genre"];
            }
            return infos;
        }
    }
}
