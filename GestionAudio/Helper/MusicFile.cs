using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using NAudio.Wave;

namespace Shared
{
    public static class MusicFile
    {
        public static AudioFileReader LoadMusic(string path)
        {
            return new AudioFileReader(path);
        }

        public static AudioFileReader LoadRadio(string path)
        {
            throw new NotImplementedException();
        }

        public static TimeSpan GetDuration(WaveOut file)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage GetBitmapFromFile(string path)
        {
            TagLib.File fileInfo = TagLib.File.Create(path);
            MemoryStream ms = new MemoryStream(fileInfo.Tag.Pictures[0].Data.Data);
            ms.Seek(0, SeekOrigin.Begin);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            return bitmap;
        }

        private static BitmapImage GetBitmapFromLink(string pictureLink) => new BitmapImage(new Uri(pictureLink));

        public static BitmapImage GetImage(string path, string pictureLink)
        {
            if (string.IsNullOrWhiteSpace(pictureLink))
                return GetBitmapFromFile(path);

            try
            {
                return GetBitmapFromLink(pictureLink);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return GetBitmapFromFile(path);
            }

        }
    }
}
