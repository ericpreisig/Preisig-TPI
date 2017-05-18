using NAudio.Wave;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Threading;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls.Dialogs;

namespace Shared
{
    public static class MusicFile
    {
        #region Public Methods

        public static string Beetween(this string source, string start, string end)
        {
            var indexOf1 = source.IndexOf(start);
            if (indexOf1 == -1) return "";
            source = source.Substring(indexOf1+ start.Length);
            var indexOf2 = source.IndexOf(end);
            if (indexOf2 == -1) return "";
            var sub = source.Substring(0, indexOf2);
            return sub;
        }

        public static TimeSpan GetDuration(WaveOut file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an image from the file or from the setted up path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pictureLink"></param>
        /// <returns></returns>
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

        public static AudioFileReader LoadMusic(string path)
        {
            return new AudioFileReader(path);
        }

        /// <summary>
        /// Stop the buffering thread of the radio
        /// </summary>
        public static void StopRadio()
        {
            if(_radioThread!=null && _radioThread.IsAlive)
                _radioThread.Abort();
        }

        private static Thread _radioThread;

        /// <summary>
        /// Code adapted from http://stackoverflow.com/questions/184683/play-audio-from-a-stream-using-c-sharp:
        /// Read a streaming music
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static WaveStream LoadRadio(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                GeneralHelper.ShowMessage("Erreur", "Pas de chemin disponnible dans le fichier de stream",
                    MessageDialogStyle.Affirmative);
                return null;
            }
            StopRadio();

            Stream ms = new MemoryStream();
            var cancelStream = false;
            _radioThread = new Thread(delegate(object o)
                {

                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    SettingsSection section = (SettingsSection) config.GetSection("system.net/settings");
                    section.HttpWebRequest.UseUnsafeHeaderParsing = true;
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.MaxServicePointIdleTime = 2000;
                    config.Save();
                    try
                    {
                        var response = WebRequest.Create(path).GetResponse();
                        using (var stream = response.GetResponseStream())
                        {
                            byte[] buffer = new byte[65536]; // 64KB chunks
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {                              
                                var pos = ms.Position;
                                ms.Position = ms.Length;
                                ms.Write(buffer, 0, read);
                                ms.Position = pos;
                                if (ms.Length == 0)
                                {
                                    cancelStream = true;
                                    GeneralHelper.ShowMessage("Erreur", "impossible de lire le stream de données",
                                        MessageDialogStyle.Affirmative);
                                    break;
                                }
                            }
                        }
                    }
                    catch (ThreadAbortException e)
                    {
                        cancelStream = true;                      
                    }
                    catch (Exception e)
                    {
                        cancelStream = true;
                        GeneralHelper.ShowMessage("Erreur", "Le serveur de stream a mal répondu",
                            MessageDialogStyle.Affirmative);
                    }

                }
            );
            _radioThread.IsBackground = true;
            _radioThread.Start();

            try
            {
                // Pre-buffering some data to allow NAudio to start playing
                while (ms.Length < 1000)
                {
                    if (cancelStream)
                        return null;

                    Thread.Sleep(100);
                }
               

                ms.Position = 0;
                return new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new AudioFileReader(ms)));
            }
            catch (Exception e)
            {
                GeneralHelper.ShowMessage("Erreur", "Impossible de convertire le stream en mp3",
                    MessageDialogStyle.Affirmative);
                return null;
            }

        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get an image from a music metadata
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get an image from a uri
        /// </summary>
        /// <param name="pictureLink"></param>
        /// <returns></returns>
        private static BitmapImage GetBitmapFromLink(string pictureLink) => new BitmapImage(new Uri(pictureLink));

        #endregion Private Methods
    }
}