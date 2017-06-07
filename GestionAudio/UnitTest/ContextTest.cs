﻿/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Test about context
*********************************************************************************/

using BLL;
using DTO.Entity;
using NUnit.Framework;
using System.Collections.Generic;
using Context = Presentation.Helper.Context;

namespace UnitTest
{
    /// <summary>
    /// try to changes the context
    /// </summary>
    [TestFixture]
    public class ContextTest
    {
        #region Public Methods

        /// <summary>
        /// Check that the software can generate the file with a path
        /// </summary>
        [Test]
        public void CheckThatFileExist()
        {
            var track = new Track
            {
                Album = new Album
                {
                    Name = "Black",
                    Artist = new Artist
                    {
                        Name = "Metallica"
                    }
                },
                Genres = new List<Genre> { new Genre { Name = "metal" } },
                Name = "Nothing else matter",
                Path = @"C:\WORKSPACE\TPI\GestionAudio\DocumentTest\Mozart\track3.mp3",
                Duration = 333
            };
            Assert.IsTrue(track.File != null);
        }

        /// <summary>
        /// Create a simple context that should play lthe actual music
        /// </summary>
        [Test]
        public void CreateAndSetContext()
        {
            Context.ActualContext.IsMusicPlaying = true;
            Context.ActualContext.SaveContext();
        }

        #endregion Public Methods
    }
}