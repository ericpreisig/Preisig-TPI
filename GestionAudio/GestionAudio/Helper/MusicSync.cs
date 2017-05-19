using BLL;
using DTO.Entity;
using NAudio.Wave;
using Presentation.View;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace Presentation.Helper
{
    public static class MusicSync
    {
        #region Private Fields

        private static readonly string[] AllowedFormat = { ".wma", ".mp3", ".wav" };
        private static List<string> _analysedFolder= new List<string>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Check if the format is on the format allowed list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckFileFormat(string path) => AllowedFormat.Any(a => a == Path.GetExtension(path));

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
                    if (MainWindowViewModel.Main.ActualView is MusicView)
                        MainWindowViewModel.Main.ActualView.DataContext = Activator.CreateInstance(MainWindowViewModel.Main.ActualView.DataContext.GetType());
                });
            });
#else
                var wrong = await MainWindowViewModel.MetroWindow.ShowMessageAsync("Erreur", "Nous n'avons détecter aucune chanson. Voulez-vous procéder à une syncronisation de votre bibliothèque ?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                {
                    DefaultButtonFocus = MessageDialogResult.Affirmative,
                    NegativeButtonText = "Fermer"
                });
            if (wrong == MessageDialogResult.Negative) return;
            new IncludeFolder { Path=Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}.AddIncludeFolder();   
            SyncAllFolders();
#endif
        }

        /// <summary>
        /// Sync all folder from the database folder include
        /// </summary>
        public static async void SyncAllFolders()
        {
            var syncMessage = await MainWindowViewModel.MetroWindow.ShowProgressAsync("Syncronisation", "Syncronisation en cours, veuillez patienter");
            await Task.Run(() =>
            {
                    
               _analysedFolder = new List<string>();
                foreach (var folders in GeneralData.GetIncludedFolder())
                {
                    SyncFolder(folders.Path);
                }
                syncMessage.CloseAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindowViewModel.Main.AnalyseStatus = "";
                    if (MainWindowViewModel.Main.ActualView is MusicView)
                        MainWindowViewModel.Main.ActualView.DataContext = Activator.CreateInstance(MainWindowViewModel.Main.ActualView.DataContext.GetType());
                });
            });

        }

        /// <summary>
        /// Loop recursively through a folder to add or update new tracks
        /// </summary>
        /// <param name="path"></param>
        public static void SyncFolder(string path)
        {

            //if already analysed folder, pass through 
            if (_analysedFolder.All(a => a != path))
            {
                _analysedFolder.Add(path);

                //Remove file if not exist anymore
                foreach (var track in TrackData.GetTracks())
                {
                    if(!File.Exists(track.Path))
                        track.RemoveTrack();
                }


                //it terate through all file with an authaurized format
                foreach (var filePath in Directory.EnumerateFiles(path).Where(x => AllowedFormat.Any(a => x.EndsWith(a))))
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

        /// <summary>
        /// Transform a file to a track
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Track TransformToTrack(string path)
        {
            WaveStream fileValues;
            try
            {
                fileValues = new AudioFileReader(path);
            }
            catch (Exception e)
            {
                fileValues = new MediaFoundationReader(path);
            }

            var fileInfo = TagLib.File.Create(path);
            var artistName = fileInfo.Tag.FirstComposer ?? "Inconnu";
            var artist = ArtistData.CheckIfArtistExist(artistName)
                ? ArtistData.GetArtists().FirstOrDefault(a => a.Name.ToLower() == artistName.ToLower())
                : new Artist { Name = artistName };

            var albumName = fileInfo.Tag.Album ?? "Inconnu";
            DateTime? dateCreation = null;

            if (fileInfo.Tag.Year != 0) dateCreation = new DateTime((int)fileInfo.Tag.Year, 1, 1);

            var album = AlbumData.CheckIfAlbumExist(albumName, artistName)
                ? AlbumData.GetAlbums().FirstOrDefault(a => a.Name.ToLower() == albumName.ToLower())
                : new Album
                {
                    Artist = artist,
                    PictureLink = LookForImage(path),
                    DateCreation = dateCreation,
                    Name = albumName,
                };

            var genreName = fileInfo.Tag.FirstGenre ?? "Inconnu";

            Genre genre = GeneralData.CheckIfGenreExist(genreName)
                ? GeneralData.GetGenres().FirstOrDefault(a => a.Name.ToLower() == genreName.ToLower())
                : new Genre { Name = genreName };

            var track = new Track
            {
                Duration = (int)fileValues.TotalTime.TotalMilliseconds,
                Album = album,
                Name = fileInfo.Tag.Title ?? Path.GetFileNameWithoutExtension(path),
                Genre = genre,
                Path = path
            };
            return track;
        }

        #endregion Public Methods

        #region Private Methods

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

        #endregion Private Methods
    }
}