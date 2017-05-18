using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DAL.API;
using DTO.Entity;
using Shared;

namespace BLL
{
    public static class RadioData
    {
        /// <summary>
        /// parse the xml and get radio element out if it 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static List<Radio> GetRadiosFromXml(string xml)
        {
            //parse the xml
            var items = from i in XDocument.Parse(xml).Descendants("station")
                select new Radio
                {
                    ShoutCastId = (string)i.Attribute("id"),
                    Name = (string)i.Attribute("name"),
                    Desrciption = (string)i.Attribute("ct"),
                    Genre = GeneralData.CheckIfGenreExist((string)i.Attribute("genre")) ? GeneralData.GetGenres().FirstOrDefault(a => a.Name.ToLower() == ((string)i.Attribute("genre")).ToLower())
                        : new Genre { Name = (string)i.Attribute("genre") }
                };

           
            return items.ToList();
        }



        /// <summary>
        /// Get top 500 radios
        /// </summary>
        /// <returns></returns>
        public static List<Radio> GetRadioTop500Radios()
        {
            return GetRadiosFromXml(Shoutcast.GetTop500Radios());
        }
       
        public static Radio SetRadioPath(Radio radio)
        {
            if (!string.IsNullOrWhiteSpace(radio.Path)) return radio;

            var downloadedFile = Shoutcast.DownloadFile(radio.ShoutCastId);
            var path = downloadedFile.Replace("\r", "").Split('\n');
            radio.Path = path.Length>=2 ? path[2] ?? "" : "";
            return radio;
        }

        public static List<Radio> GetRadioByKeyWord()
        {
            return Shoutcast.GetRadioByKeyWord("","");
        }

        public static List<Track> GetFavouriteRadios()
        {
            throw new NotImplementedException();
        }

        public static List<Track> Get10LastRadios()
        {
            throw new NotImplementedException();
        }
    }
}
