using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DTO.Entity;
using NUnit.Framework;
using Presentation.Helper;

namespace UnitTest
{
    /// <summary>
    /// Contain all test to make the sync
    /// </summary>
    [TestFixture]
    public class SyncTest
    {
        readonly List<string> _directories= new List<string>();

        /// <summary>
        /// check that folder are the folders I was looking for
        /// </summary>
        [Test]
        public void AnalyseFolderTree()
        {
            RecursiveFolderSearch("../../DocumentTest");
            Assert.IsTrue(_directories.Any(a=>a=="Chopin"));
            Assert.IsTrue(_directories.Any(a=>a=="Mozart"));
        }

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

        /// <summary>
        /// Try to sync a folder with the database
        /// </summary>
        [Test]
        public void SyncFolder()
        {
            MusicSync.SyncFolder("../../DocumentTest");
        }

    }
}
