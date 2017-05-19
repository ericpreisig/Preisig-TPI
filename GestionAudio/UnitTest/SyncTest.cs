using NUnit.Framework;
using Presentation.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTest
{
    /// <summary>
    /// Contain all test to make the sync
    /// </summary>
    [TestFixture]
    public class SyncTest
    {
        #region Private Fields

        private readonly List<string> _directories = new List<string>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// check that folder are the folders I was looking for
        /// </summary>
        [Test]
        public void AnalyseFolderTree()
        {
            RecursiveFolderSearch("../../DocumentTest");
            Assert.IsTrue(_directories.Any(a => a == "Chopin"));
            Assert.IsTrue(_directories.Any(a => a == "Mozart"));
        }

        /// <summary>
        /// Try to sync a folder with the database
        /// </summary>
        [Test]
        public void SyncFolder()
        {
            MusicSync.SyncFolder("../../DocumentTest");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Recusivly search child folder in a folder
        /// </summary>
        /// <param name="folder"></param>
        private void RecursiveFolderSearch(string folder)
        {
            foreach (var folderName in Directory.GetDirectories(folder))
            {
                _directories.Add(Path.GetDirectoryName(folderName));
                RecursiveFolderSearch(folder);
            }
        }

        #endregion Private Methods
    }
}