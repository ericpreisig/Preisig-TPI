using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DTO.Entity;

namespace DAL.API
{
    public static class Shoutcast
    {
        private const string DevId = "l2VAYjJAcjDXcbai";

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
        private static string ApiStringPrepare(string name)=>"http://api.shoutcast.com/legacy/" + name + "?k=" + DevId;

        public static List<Radio> GetRadioByKeyWord(string name, string genre)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the top 500 radios
        /// </summary>
        /// <returns></returns>
        public static string GetTop500Radios()=> ApiExecute(ApiStringPrepare("Top500"));          

        /// <summary>
        /// Download the radio file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DownloadFile(string id)=>ApiExecute("http://yp.shoutcast.com/sbin/tunein-station.m3u?id=" + id);
    }
}
