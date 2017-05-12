using System;
using DTO.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Presentation.Helper;

namespace UnitTest
{
    /// <summary>
    /// try to changes the context
    /// </summary>
    [TestFixture]
    public class ContextTest
    {
        /// <summary>
        /// Create a simple context that should play lthe actual music
        /// </summary>
        [Test]
        public void CreateAndSetContext()
        {
            Presentation.Helper.Context.ActualContext.IsMusicPlaying = true;
            Presentation.Helper.Context.SaveConext();
            Presentation.Helper.Context.SetConext();
        }

        /// <summary>
        /// Try to change the actual music to the next then the previous one
        /// </summary>
        [Test]
        public void ReadingListOperation()
        {
            MusicPlayer.Next();
            MusicPlayer.Previous();
        }
    }
}
