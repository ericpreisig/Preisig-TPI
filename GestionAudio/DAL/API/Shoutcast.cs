using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DAL.API
{
    /// <summary>
    /// Class to make the link between the software and the shoutcast API, it contains all the call used by this app
    /// </summary>
    public static class Shoutcast
    {
        #region Private Fields

        private const string DevId = "l2VAYjJAcjDXcbai";

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Download the radio file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DownloadFile(string id) => ApiExecute("http://yp.shoutcast.com/sbin/tunein-station.xspf?id=" + id).Result;

        /// <summary>
        /// Return all radio that match the result in xml
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static string GetRadioByKeyWord(string keyWord)
        {
            return ApiExecute(ApiStringPrepare("stationsearch") + "&search=" + keyWord.Replace(" ", "+")+ "&limit=100").Result;
        }

        /// <summary>
        /// Get the top 500 radios
        /// </summary>
        /// <returns></returns>
        public static string GetTop500Radios() => ApiExecute(ApiStringPrepare("Top500")).Result;

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Execute a request and wait for the anser
        /// </summary>
        /// <param name="executionString"></param>
        /// <returns></returns>
        private static async Task<string> ApiExecute(string executionString)
        {
            return await Task.Run(() =>
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(executionString);
                }
            });
         
        }

        /// <summary>
        /// Prepare a string with the api key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string ApiStringPrepare(string name) => "http://api.shoutcast.com/legacy/" + name + "?k=" + DevId;

        #endregion Private Methods
    }
}