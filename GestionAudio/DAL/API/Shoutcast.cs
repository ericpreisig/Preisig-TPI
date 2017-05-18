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

        private static void ApiConnect()
        {
            
        }

        private static string ApiExecute(string executionString)
        {
            var request = (HttpWebRequest)WebRequest.Create(executionString);
            var response = (HttpWebResponse)request.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        private static string ApiStringPrepare(string name)
        {
            return "http://api.shoutcast.com/legacy/" + name + "?k=" + DevId+ "&mt=audio/aacp";
        }

        public static List<Radio> GetRadioByKeyWord(string name, string genre)
        {
            throw new NotImplementedException();
        }

        public static string GetTop500Radios()
        {
            return ApiExecute(ApiStringPrepare("Top500"));          
        }

        public static string DownloadFile(string id)
        {
            return ApiExecute("http://yp.shoutcast.com/sbin/tunein-station.m3u?id=" + id);
        }
    }
}
