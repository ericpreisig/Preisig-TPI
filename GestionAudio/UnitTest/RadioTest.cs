using System;
using System.Collections.Generic;
using DAL.API;
using DTO.Entity;
using NUnit.Framework;

namespace UnitTest
{
    /// <summary>
    /// Contain all test to make the radios work
    /// </summary>
    [TestFixture]
    public class RadioTest
    {
        private List<Radio> _radios;
        private Shoutcast _api;

        /// <summary>
        /// Try to initialize an API connection
        /// </summary>
        [Test]
        public void CheckApiConnection()
        {
            _api= new Shoutcast();
        }

        /// <summary>
        /// Make a search in the api
        /// </summary>
        [Test]
        public void CheckApiData()
        {
            _radios = _api.GetRadio("acdc", "metal");
        }

        /// <summary>
        /// Check that the data I got from the api is the one I was looking for
        /// </summary>
        [Test]
        public void CheckRadioInfo()
        {
            Assert.IsTrue(_radios[0].Genre.Name.ToLower() == "metal");
        }

    }
}
