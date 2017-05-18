using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using BLL;
using DTO.Entity;
using NAudio.Wave;
using NUnit.Framework;
using Presentation.Helper;
using Presentation.ViewModel;

namespace UnitTest
{
    /// <summary>
    /// Contain all test to make a simple try work
    /// </summary>
    [TestFixture]
    public class AudioTest
    {
        private List<Track> tracks= new List<Track>();
        private Artist artist;
        private Album album;
        private Radio radio;
        private Playlist playlist;

        /// <summary>
        /// Create basic music datas
        /// </summary>
        [Test]
        public void LoadData()
        {
            artist = new Artist
            {
                Name = "Metallica"
            };

            album = new Album
            {
                Name = "Black",
                Artist = artist          
            };

            tracks.Add(new Track
            {
                Album = album,
                Genre = new Genre { Name = "metal"},
                Name = "Nothing else matter",
                Path = "../../Mozart/track1.mp3",
                Duration = 333                             
            });

            tracks.Add(new Track
            {
                Album = album,
                Genre = new Genre { Name = "metal" },
                Name = "Back in Black",
                Path = "../../Chopin/trackWma.wma",
                Duration = 123
            });


            tracks.Add(new Track
            {
                Album = album,
                Genre = new Genre { Name = "metal" },
                Name = "Back in Black",
                Path = "../../Bach/trackWav.wav",
                Duration = 111
            });

            radio=new Radio
            {
                Genre = new Genre { Name = "metal" },
                Name = "radio",
                Path = "http://54.202.122.200:8000",
            };

            MainWindowViewModel.Main.ReadingList = new ObservableCollection<Track>(tracks);
        }

        /// <summary>
        /// Test musics format (wma, wmv, mp3)
        /// </summary>
        [Test]
        public void MusicTestFormats()
        {
            Presentation.Helper.Context.PlayNewSong(tracks[0]);
            MusicPlayer.Play();
            Presentation.Helper.Context.PlayNewSong(tracks[1]);
            MusicPlayer.Play();
            Presentation.Helper.Context.PlayNewSong(tracks[2]);
            MusicPlayer.Play();
        }

        /// <summary>
        /// Test to Pause, Rewind and go forward in the actual music
        /// </summary>
        [Test]
        public void MusicOperationTrack()
        {
            MusicPlayer.Pause();
         
        }


        /// <summary>
        /// Test to Pause, Rewind and go forward in the actual music
        /// </summary>
        [Test]
        public void MusicOperationRadio()
        {
            //Presentation.Helper.Context.PlayNewSong(radio);
            MusicPlayer.Pause();
         
        }

        /// <summary>
        /// Check that the track info I save is the same from the mp3
        /// </summary>
        [Test]
        public void CheckTrackInfo()
        {
            var infoTrack = MusicSync.TransformToTrack(tracks[0].Path);
            var trueValues = new Mp3FileReader(tracks[0].Path);
            
            Assert.IsTrue(infoTrack.Duration== trueValues.TotalTime.TotalMilliseconds);
            Assert.IsTrue(infoTrack.Name== Path.GetFileName(tracks[0].Path));
        }

        /// <summary>
        /// Try to add then try to remove a song from favorites
        /// </summary>
        [Test]
        public void FavoriteAddRemoveTrackAndRadio()
        {
            FavoriteData.AddFavorite(tracks[0]);
            FavoriteData.RemoveFavorite(tracks[0]);
        }

        /// <summary>
        /// Create and add tracks to a playlist
        /// </summary>
        [Test]
        public void CreatePlaylist()
        {
            playlist = new Playlist {Name = "test"};
            playlist.Tracks.Add(tracks[0]);
            playlist.Tracks.Add(tracks[1]);
            playlist.Tracks.Add(tracks[3]);
        }

        /// <summary>
        /// Try to chnage to the next, then previous file in playlist
        /// </summary>
        [Test]
        public void CheckPlaylist()
        {
            MusicPlayer.Next();
            MusicPlayer.Previous();
        }
    }
}
