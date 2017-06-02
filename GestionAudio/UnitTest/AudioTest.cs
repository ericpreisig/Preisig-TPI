/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary :Test about audio
*********************************************************************************/

using DTO.Entity;
using NAudio.Wave;
using NUnit.Framework;
using Presentation.Helper;
using System.Collections.Generic;
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

        private Album _album;
        private Artist _artist;
        private Playlist _playlist;
        private readonly List<Track> _tracks = new List<Track>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Check that the track info I save is the same from the mp3
        /// </summary>
        [Test]
        public void CheckTrackInfo()
        {
            var infoTrack = MusicSync.TransformToTrack(_tracks[0].Path);
            var trueValues = new Mp3FileReader(_tracks[0].Path);

            Assert.IsTrue(infoTrack.Duration == trueValues.TotalTime.TotalMilliseconds);
            Assert.IsTrue(infoTrack.Name == Path.GetFileNameWithoutExtension(_tracks[0].Path));
        }

        /// <summary>
        /// Create and add tracks to a playlist
        /// </summary>
        [Test]
        public void CreatePlaylist()
        {
            _playlist = new Playlist { Name = "test" };
            _playlist.Tracks.Add(_tracks[0]);
            _playlist.Tracks.Add(_tracks[1]);
            _playlist.Tracks.Add(_tracks[2]);
        }

        /// <summary>
        /// Create basic music datas
        /// </summary>
        [Test, Order(1)]
        public void LoadData()
        {
            _artist = new Artist
            {
                Name = "Metallica"
            };

            _album = new Album
            {
                Name = "Black",
                Artist = _artist
            };

            _tracks.Add(new Track
            {
                Album = _album,
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "Nothing else matter",
                Path = @"C:\WORKSPACE\TPI\GestionAudio\DocumentTest\Mozart\track3.mp3",
                Duration = 333
            });

            _tracks.Add(new Track
            {
                Album = _album,
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "Back in Black",
                Path = "../../Chopin/trackWma.wma",
                Duration = 123
            });

            _tracks.Add(new Track
            {
                Album = _album,
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "Back in Black",
                Path = "../../Bach/trackWav.wav",
                Duration = 111
            });

            var radio=  new Radio
            {
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "radio",
                Path = "http://54.202.122.200:8000",
            };
        }

        #endregion Public Methods
    }
}