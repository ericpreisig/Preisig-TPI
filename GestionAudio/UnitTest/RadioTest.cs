/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Test about radios
*********************************************************************************/

using BLL;
using DTO.Entity;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest
{
    /// <summary>
    /// Contain all test to make the radios work
    /// </summary>
    [TestFixture]
    public class RadioTest
    {
        #region Private Fields

        private List<Radio> _radios;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Try to initialize an API connection
        /// </summary>
        [Test]
        public void CheckApiConnection()
        {
            //_api= new Shoutcast();
        }

        /// <summary>
        /// Make a search in the api
        /// </summary>
        [Test]
        public void CheckApiData()
        {
            _radios = RadioData.GetRadioByKeyWord("metal");
        }

        /// <summary>
        /// Check that the data I got from the api is the one I was looking for
        /// </summary>
        [Test]
        public void CheckRadioInfo()
        {
            Assert.IsTrue(_radios[0].Genres.FirstOrDefault().Name.ToLower() == "metal");
        }

        #endregion Public Methods
    }
}