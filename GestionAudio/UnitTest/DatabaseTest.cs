using System;
using DAL.Database;
using DTO.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace UnitTest
{
    /// <summary>
    /// Check a connection with the database
    /// </summary>
    [TestFixture]
    public class DatabaseTest
    {
        /// <summary>
        /// Try to contect  on the database 
        /// </summary>
        [Test]
        public void CheckConnection()
        {
            var database= new DbApplicationContext();
            database.SaveChanges();
        }

    }
}
