using MahApps.Metro.Controls.Dialogs;
using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Shared
{
    public static class MusicFile
    {
        #region Public Methods

        private static Thread _radioThread;

        private static Stream Mp3ms = new MemoryStream();

        private static Stream ms = new MemoryStream();

        /// <summary>
        /// Get a string beetween
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string Between(this string source, string start, string end)
        {
            var indexOf1 = source.IndexOf(start);
            if (indexOf1 == -1) return "";
            source = source.Substring(indexOf1 + start.Length);
            var indexOf2 = source.IndexOf(end);
            if (indexOf2 == -1) return "";
            var sub = source.Substring(0, indexOf2);
            return sub;
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

        /// <summary>
        /// Load a music, if it fails, try to load it with the windows api
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static WaveStream LoadMusic(string path)
        {
            try
            {
                return new AudioFileReader(path);
            }
            catch (Exception e)
            {
                return new MediaFoundationReader(path);
            }
        }

        /// <summary>
        /// Code adapted from http://stackoverflow.com/questions/184683/play-audio-from-a-stream-using-c-sharp:
        /// Read a streaming music
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static WaveStream LoadRadio(string path, string mt)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                GeneralHelper.ShowMessage("Erreur", "Pas de chemin disponnible dans le fichier de stream",
                    MessageDialogStyle.Affirmative);
                return null;
            }

            ms = new MemoryStream();
            Mp3ms = new MemoryStream();
            var cancelStream = false;

            //make sure that radios are stopped
            _radioThread = new Thread(delegate (object o)
                {
                    try
                    {
                        var response = WebRequest.Create(path).GetResponse();
                        var errorResponse = response as HttpWebResponse;

                        //Check that is not a 404 error
                        if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            cancelStream = true;
                            GeneralHelper.ShowMessage("Erreur", "La radio n'est pas disponnible",
                                MessageDialogStyle.Affirmative);
                            return;
                        }

                        //ask the radio for stream
                        using (var stream = response.GetResponseStream())
                        {
                            byte[] buffer = new byte[65536]; // 64KB chunks
                            int read;

                            //Read all bytes from the reponse the radio gave
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                if (cancelStream) break;
                                var pos = ms.Position;
                                ms.Position = ms.Length;
                                ms.Write(buffer, 0, read);
                                ms.Position = pos;

                                //if the stream is always 0 after the first iteration, abort the radio
                                if (ms.Length == 0)
                                {
                                    cancelStream = true;
                                    GeneralHelper.ShowMessage("Erreur", "impossible de lire le stream de données",
                                        MessageDialogStyle.Affirmative);
                                    break;
                                }
                                try
                                {
                                    if (mt == "audio/mpeg") continue;

                                    //Convert the stream to an mpeg format
                                    var mp3Stream = StreamToMp3(ms);
                                    mp3Stream.Position = Mp3ms.Length;

                                    //Read the stream and put every thing back in a stream
                                    while ((read = mp3Stream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        var pos2 = Mp3ms.Position;
                                        Mp3ms.Position = Mp3ms.Length;
                                        Mp3ms.Write(buffer, 0, read);
                                        Mp3ms.Position = pos2;
                                    }
                                }
                                catch (ThreadAbortException e)
                                {
                                    cancelStream = true;
                                }
                                catch (Exception e)
                                {
                                    cancelStream = true;
                                    GeneralHelper.ShowMessage("Erreur", "Une erreur inattendue c'est produite",
                                        MessageDialogStyle.Affirmative);
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
            )
            { IsBackground = true };
            _radioThread.Start();
            var numberTry = 0;
            while (numberTry<3)
            {
                try
                {
                    // Pre-buffering some data to allow NAudio to start playing
                    while (ms.Length < 16000)
                    {
                        if (cancelStream)
                            return null;
                        Thread.Sleep(200);
                    }

                    //If the steram is an mpeg, take directly the stream
                    if (mt == "audio/mpeg")
                    {
                        Mp3ms.Position = 0;
                        return new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms)));
                    }

                    //else, take the converted stream
                    ms.Position = 0;
                    return new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(Mp3ms)));
                }
                catch (Exception e)
                {
                    numberTry++;
                    Thread.Sleep(100 * numberTry);
                    if (numberTry<3) continue;
                    GeneralHelper.ShowMessage("Erreur", "Impossible de convertire le stream en mp3",
                        MessageDialogStyle.Affirmative);
                    throw new ArgumentException("Impossible de convertire le stream en mp3");
                }
            }
            return null;


        }

        /// <summary>
        /// Stop the buffering thread of the radio
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public static void StopRadio()
        {
            if (_radioThread == null || !_radioThread.IsAlive) return;
           
            //Wait for the true stop
            while (_radioThread!= null && _radioThread.IsAlive)
            {
               _radioThread.Abort();
               _radioThread=null;
               
                Thread.Sleep(100);
            }
            _radioThread = null;
        }

        /// <summary>
        /// Change stream to an mp3 stream
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Stream StreamToMp3(this Stream source)
        {
            var retMs = new MemoryStream();
            var rdr = new StreamMediaFoundationReader(source);
            var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, LAMEPreset.VBR_90);
            rdr.CopyTo(wtr);
            return retMs;
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