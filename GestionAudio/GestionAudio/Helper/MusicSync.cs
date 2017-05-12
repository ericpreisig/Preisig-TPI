using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using BLL;
using DTO.Entity;
using MahApps.Metro.Controls.Dialogs;
using NAudio.Wave;
using Presentation.ViewModel;

namespace Presentation.Helper
{
    public static class MusicSync
    {
        static readonly string[] AllowedFormat =  {".wma",".mp3",".wav"};

        public static void SyncDrives()
        {
            foreach (var drives in DriveInfo.GetDrives())
            {
                SyncFolder(drives.Name);
            }
        }

        /// <summary>
        /// Loop recursively through a folder to add or update new tracks
        /// </summary>
        /// <param name="path"></param>
        public static void SyncFolder(string path)
        {


            foreach (var filePath in Directory.GetFiles(path))
            {
                Application.Current.Dispatcher.Invoke(() => MainWindowViewModel.Main.AnalyseStatus = filePath);

                if (!CheckFileFormat(filePath)) continue;

                // already in database
                if (TrackData.CheckIfAlreadyInDatabase(filePath))
                {
                    TrackData.GetTrackByPath(filePath).UpdateTrackInfo(TransformToTrack(filePath)).AddOrUpdateAudio();
                }
                else
                {
                    //Ignore mp3 that have a smaple rate change                  
                    try
                    {
                        TransformToTrack(filePath).AddOrUpdateAudio();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            foreach (var folderPath in Directory.GetDirectories(path))
            {
                try
                {
                    SyncFolder(folderPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //Unotorized folder
                }
            }
            

            //Update list after sync
           
        }


        public static bool CheckFileFormat(string path) => AllowedFormat.Any(a => a == Path.GetExtension(path));


        /// <summary>
        /// Transform a file to a track
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Track TransformToTrack(string path)
        {
            var fileValues = new AudioFileReader(path);

            var fileInfo = TagLib.File.Create(path);
            var artistName = fileInfo.Tag.FirstPerformer ?? "Inconnu";
            var artist = ArtistData.CheckIfArtistExist(artistName)
                ? ArtistData.GetArtists().FirstOrDefault(a => a.Name.ToLower() == artistName.ToLower())
                : new Artist {Name = artistName };

            var albumName = fileInfo.Tag.Album ?? "Inconnu";
            DateTime? dateCreation = null;

            if (fileInfo.Tag.Year != 0) dateCreation = new DateTime((int) fileInfo.Tag.Year,1,1);

            var album = AlbumData.CheckIfAlbumExist(albumName)
                ? AlbumData.GetAlbums().FirstOrDefault(a => a.Name.ToLower() == albumName.ToLower())
                : new Album
                {
                    Artist = artist,
                    PictureLink = LookForImage(path),
                    DateCreation = dateCreation,
                    Name = albumName,
                };


            Genre genre = null;
            if (fileInfo.Tag.FirstGenre!=null)
            {
                genre = GeneralData.CheckIfGenreExist(fileInfo.Tag.FirstGenre)
                    ? GeneralData.GetGenres().FirstOrDefault(a => a.Name.ToLower() == fileInfo.Tag.FirstGenre.ToLower())
                    : new Genre { Name = fileInfo.Tag.FirstGenre };
            }

            var track= new Track
            {
                Duration = (int)fileValues.TotalTime.TotalMilliseconds,
                Album=album,
                Name = Path.GetFileNameWithoutExtension(path),
                Genre = genre,
                Path = path
            };
            return track;

        }

        /// <summary>
        /// Look for an image in the folder of the track
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string LookForImage(string path)
        {
            var pathToCheck = Directory.GetParent(path).FullName;
            foreach (var imagePath in Directory.GetFiles(pathToCheck))
            {
                try
                {
                    //throw if not an image
                    Image.FromFile(imagePath);
                    return imagePath;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return null;
        }

        /// <summary>
        /// If the local library is empty
        /// </summary>
        public static async void NoMusic()
        {

#if DEBUG
            await Task.Run(() =>
            {
                SyncFolder(@"C:\WORKSPACE\TPI\GestionAudio\DocumentTest");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindowViewModel.Main.AnalyseStatus = "";
                    MainWindowViewModel.Main.ActualView.DataContext = Activator.CreateInstance(MainWindowViewModel.Main.ActualView.DataContext.GetType());
                });
            });
#else
                var wrong = await MainWindowViewModel.MetroWindow.ShowMessageAsync("Erreur", "Nous n'avons détecter aucune chanson. Voulez-vous procéder à une syncronisation ?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                {
                    DefaultButtonFocus = MessageDialogResult.Affirmative,
                    NegativeButtonText = "Fermer"
                });
                if (wrong != MessageDialogResult.Negative)
                {
                      await Task.Run(() =>
                     {
                         SyncFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                         Application.Current.Dispatcher.Invoke(() =>
                         {
                             MainWindowViewModel.Main.AnalyseStatus = "";
                             MainWindowViewModel.Main.ActualView.DataContext =  Activator.CreateInstance(MainWindowViewModel.Main.ActualView.DataContext.GetType());
                         });
                     });
                }
#endif

        }

    }
}
