using BLL;
using DTO.Entity;
using NAudio.Wave;
using NUnit.Framework;
using Presentation.Helper;
using Presentation.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace UnitTest
{
    /// <summary>
    /// Contain all test to make a simple try work
    /// </summary>
    [TestFixture]
    public class AudioTest
    {
        #region Private Fields

        private Album album;
        private Artist artist;
        private Playlist playlist;
        private Radio radio;
        private List<Track> tracks = new List<Track>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Create basic music datas
        /// </summary>
        [Test, Order(1)]
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
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "Nothing else matter",
                Path = @"C:\WORKSPACE\TPI\GestionAudio\DocumentTest\Mozart\track3.mp3",
                Duration = 333
            });

            tracks.Add(new Track
            {
                Album = album,
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "Back in Black",
                Path = "../../Chopin/trackWma.wma",
                Duration = 123
            });

            tracks.Add(new Track
            {
                Album = album,
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "Back in Black",
                Path = "../../Bach/trackWav.wav",
                Duration = 111
            });

            radio = new Radio
            {
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "radio",
                Path = "http://54.202.122.200:8000",
            };
        }

        /// <summary>
        /// Check that the track info I save is the same from the mp3
        /// </summary>
        [Test]
        public void CheckTrackInfo()
        {
            var infoTrack = MusicSync.TransformToTrack(tracks[0].Path);
            var trueValues = new Mp3FileReader(tracks[0].Path);

            Assert.IsTrue(infoTrack.Duration == trueValues.TotalTime.TotalMilliseconds);
            Assert.IsTrue(infoTrack.Name == Path.GetFileNameWithoutExtension(tracks[0].Path));
        }

        /// <summary>
        /// Create and add tracks to a playlist
        /// </summary>
        [Test]
        public void CreatePlaylist()
        {
            playlist = new Playlist { Name = "test" };
            playlist.Tracks.Add(tracks[0]);
            playlist.Tracks.Add(tracks[1]);
            playlist.Tracks.Add(tracks[2]);
        }

        
        #endregion Public Methods
    }
}