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

    /// <summary>
    /// Class used for the sync, it contains the allowed formats
    /// </summary>
    public static class MusicSync
    {
        #region Private Fields

        private static readonly string[] AllowedFormat = { ".wma", ".mp3", ".wav" };
        private static List<string> _analysedFolder = new List<string>();

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
                LaunchSync(@"C:\WORKSPACE\TPI\GestionAudio\DocumentTest");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindowViewModel.Main.AnalyseStatus = "";
                    if (MainWindowViewModel.Main.ActualView is MusicView)
                        MainWindowViewModel.Main.ActualView.DataContext = Activator.CreateInstance(MainWindowViewModel.Main.ActualView.DataContext.GetType());
                });
            });
#else
                var wrong = await MainWindowViewModel.MetroWindow.ShowMessageAsync("Erreur", "Nous n'avons détecter aucune chanson. Voulez-vous procéder à une Synchronisation de votre bibliothèque ?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
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
            var syncMessage = await MainWindowViewModel.MetroWindow.ShowProgressAsync("Synchronisation", "Synchronisation en cours, veuillez patienter");
            await Task.Run(() =>
            {
                    
               _analysedFolder = new List<string>();

                foreach (var folders in GeneralData.GetIncludedFolder())
                {
                    LaunchSync(folders.Path);
                }
                syncMessage.CloseAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainWindowViewModel.Main.AnalyseStatus = "";
                    if (MainWindowViewModel.Main.ActualView is MusicView)
                        MainWindowViewModel.Main.ActualView.DataContext = Activator.CreateInstance(MainWindowViewModel.Main.ActualView.DataContext.GetType());
                    SearchViewModel.EmptyBuffer();
                });
            });

        }

        /// <summary>
        /// Remove all trash files
        /// </summary>
        private static void CleanDatabase()
        {      
            //Remove file if not exist anymore
            foreach (var track in TrackData.GetTracks())
            {
                if (!File.Exists(track.Path))
                    track.RemoveTrack();
            }

            var folderToAnalyse =GeneralData.GetIncludedFolder();


            //Remove all music that are not in path anymore
            foreach (var track in TrackData.GetTracks())
            {
                if (!folderToAnalyse.Any(a=>Path.GetFullPath(track.Path).StartsWith(Path.GetFullPath(a.Path))))
                    track.RemoveTrack();
            }
        }

        /// <summary>
        /// Loop recursively through a folder to add or update new tracks
        /// </summary>
        /// <param name="path"></param>
        private static void LaunchSync(string path)
        {
            CleanDatabase();
            SyncFolder(path);
        }


        /// <summary>
        /// Loop recursively through a folder to add or update new tracks
        /// </summary>
        /// <param name="path"></param>
        private static void SyncFolder(string path)
        {

            //if already analysed folder, pass through 
            if (_analysedFolder.All(a => a != path))
            {
                _analysedFolder.Add(path);

              
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
                        //Ignore mp3 that have a sample rate change
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
                    //Unotorized folder
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Transform a file to a track
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Track TransformToTrack(string path)
        {
            WaveStream fileValues;

            //if it can't open with the base library Naudio, use media player dll
            try
            {
                fileValues = new AudioFileReader(path);
            }
            catch (Exception e)
            {
                fileValues = new MediaFoundationReader(path);
            }

            var fileInfo = TagLib.File.Create(path);

            //set artist
            var artistName = fileInfo.Tag.FirstAlbumArtist ?? fileInfo.Tag.FirstComposer ?? fileInfo.Tag.FirstPerformer ?? "Inconnu";
            var artist = ArtistData.CheckIfArtistExist(artistName)
                ? ArtistData.GetArtists().FirstOrDefault(a => a.Name.ToLower() == artistName.ToLower())
                : new Artist { Name = artistName };

            //set album
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
                    Name = albumName
                };

            //set genre
            List<Genre> genres = new List<Genre>();
            List<string> genreTag= new List<string>(fileInfo.Tag.Genres);
            if(genreTag.Count==0)
                genreTag.Add("Inconnu");
             
            foreach (var tagGenre in genreTag)
            {
                genres.Add(GeneralData.CheckIfGenreExist(tagGenre)
                    ? GeneralData.GetGenres().FirstOrDefault(a => a.Name.ToLower() == tagGenre.ToLower())
                    : new Genre { Name = tagGenre });
            }

            //create track
            var track = new Track
            {
                Duration = (int)fileValues.TotalTime.TotalMilliseconds,
                Album = album,
                Name = fileInfo.Tag.Title ?? Path.GetFileNameWithoutExtension(path),
                Genres = genres,
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