using DAL.Database;
using NUnit.Framework;

namespace UnitTest
{
    /// <summary>
    /// Check a connection with the database
    /// </summary>
    [TestFixture]
    public class DatabaseTest
    {
        #region Public Methods

        /// <summary>
        /// Try to contect  on the database
        /// </summary>
        [Test]
        public void CheckConnection()
        {
            var database = new DbApplicationContext();
            database.SaveChanges();
        }

        #endregion Public Methods
    }
}