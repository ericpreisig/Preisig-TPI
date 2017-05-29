﻿using DAL.API;
using DAL.Database;
using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using static System.String;

namespace BLL
{
    public static class RadioData
    {
        #region Public Methods

        /// <summary>
        /// Get last ten listened radios
        /// </summary>
        /// <returns></returns>
        public static List<Radio> Get10LastRadios()=> new Repository<Radio>().GetList().OrderByDescending(a => a.LastListen).Take(10).ToList();      

        /// <summary>
        /// Add a radio to recent radios, and remove when they is more than 10 radios 
        /// </summary>
        /// <returns></returns>
        public static Radio AddRadioToRecent(this Radio radio)
        {
            var repo= new Repository<Radio>();
            var newRadio = repo.GetList().FirstOrDefault(a => a.Path == radio.Path) ?? radio;
            newRadio.LastListen= DateTime.Now;
            repo.AddOrUpdate(newRadio);
            DeleteOldRadio();
            return newRadio;
        }

        /// <summary>
        ///Delete radio which is not in favorite, nor in last 10 listen
        /// </summary>
        /// <returns></returns>
        private static void DeleteOldRadio()
        {
            var repo = new Repository<Radio>();
            var count=0;
            foreach (var radio in repo.GetList().OrderByDescending(a=>a.LastListen))
            {
                count++;
                if(count>10 && !radio.IsFavorite)
                    repo.Delete(radio);                    
            }

        }

        /// <summary>
        /// Get all radio the user put on favorite
        /// </summary>
        /// <returns></returns>
        public static List<Radio> GetFavouriteRadios()=> new Repository<Radio>().GetList().Where(radio => radio.IsFavorite).ToList();

        /// <summary>
        /// Get all radio that match the keyword
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static List<Radio> GetRadioByKeyWord(string keyWord) => GetRadiosFromXml(Shoutcast.GetRadioByKeyWord(keyWord)).Where(a=>a.Name.ToLower().Contains(keyWord) || a.Genres.Any(b=>b.Name.ToLower().Contains(keyWord))).ToList();

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
            if (!IsNullOrWhiteSpace(radio.Path)) return radio;

            var counterTry = 0;
            while (counterTry<3 && IsNullOrWhiteSpace(radio.Path))
            {
                Thread.Sleep(200*counterTry);
                var downloadedFile = Shoutcast.DownloadFile(radio.ShoutCastId);
                var doc = XDocument.Parse(downloadedFile).Root;
                var path = "";
                if (doc != null)
                {
                    path = (from d in doc.Descendants()
                        where d.Name.LocalName == "location"
                        select d.Value).FirstOrDefault();
                }

                radio.Path = path;
                counterTry++;
            }          
            return radio;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// parse the xml and get radio element out if it
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static List<Radio> GetRadiosFromXml(string xml)
        {
            if(IsNullOrWhiteSpace(xml)) return new List<Radio>();

            //parse the xml
            var items = from i in XDocument.Parse(xml).Descendants("station")
                        select new Radio
                        {
                            ShoutCastId = (string)i.Attribute("id"),
                            Name = (string)i.Attribute("name"),
                            LogoUrl = (string)i.Attribute("logo"),
                            Format = (string)i.Attribute("mt"),
                            Desrciption = (string)i.Attribute("ct"),
                            Genres = new List<Genre> {  GeneralData.CheckIfGenreExist((string)i.Attribute("genre")) ? GeneralData.GetGenres().FirstOrDefault(a => a.Name.ToLower() == ((string)i.Attribute("genre")).ToLower())
                                : new Genre { Name = (string)i.Attribute("genre") }}                          
                        };

            return items.ToList();
        }

        #endregion Private Methods
    }
}