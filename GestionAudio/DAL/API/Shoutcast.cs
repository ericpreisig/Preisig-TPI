﻿using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Net;

namespace DAL.API
{
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
        public static string DownloadFile(string id) => ApiExecute("http://yp.shoutcast.com/sbin/tunein-station.m3u?id=" + id);

        /// <summary>
        /// Return all radio that match the result in xml
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static string GetRadioByKeyWord(string keyWord)
        {
            return ApiExecute(ApiStringPrepare("stationsearch") + "&search=" + keyWord.Replace(" ", "+")+ "&limit=100");
        }

        /// <summary>
        /// Get the top 500 radios
        /// </summary>
        /// <returns></returns>
        public static string GetTop500Radios() => ApiExecute(ApiStringPrepare("Top500"));

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Execute a request and wait for the anser
        /// </summary>
        /// <param name="executionString"></param>
        /// <returns></returns>
        private static string ApiExecute(string executionString)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(executionString);
            }
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