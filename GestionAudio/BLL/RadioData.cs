/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Handle action with radios
*********************************************************************************/

using DAL.API;
using DAL.Database;
using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using static System.String;

namespace BLL
{
    /// <summary>
    /// Contain all action usable by a radio
    /// </summary>
    public static class RadioData
    {
        #region Public Methods

        /// <summary>
        /// Add a radio to recent radios, and remove when they is more than 10 radios
        /// </summary>
        /// <returns></returns>
        public static Radio AddRadioToRecent(this Radio radio)
        {
            var repo = new Repository<Radio>();
            var newRadio = repo.GetList().FirstOrDefault(a => a.Path == radio.Path) ?? radio;
            newRadio.LastListen = DateTime.Now;
            repo.AddOrUpdate(newRadio);
            DeleteOldRadio();
            return newRadio;
        }

        /// <summary>
        /// Get last ten listened radios
        /// </summary>
        /// <returns></returns>
        public static List<Radio> Get10LastRadios() => new Repository<Radio>().GetList().OrderByDescending(a => a.LastListen).Take(10).ToList();

        /// <summary>
        /// Get all radio the user put on favorite
        /// </summary>
        /// <returns></returns>
        public static List<Radio> GetFavouriteRadios() => new Repository<Radio>().GetList().Where(radio => radio.IsFavorite).ToList();

        /// <summary>
        /// Get all radio that match the keyword
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static List<Radio> GetRadioByKeyWord(string keyWord) => GetRadiosFromXml(Shoutcast.GetRadioByKeyWord(keyWord)).Where(a => a.Name.ToLower().Contains(keyWord) || a.Genres.Any(b => b.Name.ToLower().Contains(keyWord))).ToList();

        /// <summary>
        /// Get top 500 radios
        /// </summary>
        /// <returns></returns>
        public static List<Radio> GetRadioTop500Radios() => GetRadiosFromXml(Shoutcast.GetTop500Radios());

        /// <summary>
        /// Let 3 tryies to get the strea link form a radio
        /// </summary>
        /// <param name="radio"></param>
        /// <returns></returns>
        public static Radio SetRadioPath(Radio radio)
        {
            if (!IsNullOrWhiteSpace(radio.Path)) return radio;

            var counterTry = 0;
            while (counterTry < 3 && IsNullOrWhiteSpace(radio.Path))
            {
                Thread.Sleep(200 * counterTry);
                var downloadedFile = Shoutcast.DownloadFile(radio.ShoutCastId);

                //Parse the document as an XML
                var doc = XDocument.Parse(downloadedFile).Root;
                var path = "";
                if (doc != null)
                {
                    //Get the element location which is the link
                    path = (from d in doc.Descendants()
                            where d.Name.LocalName == "location"
                            select d.Value).FirstOrDefault();
                }

                radio.Path = path;
                counterTry++;
            }
            return radio;
        }

        /// <summary>
        ///Delete radio which is not in favorite, nor in last 10 listen
        /// </summary>
        /// <returns></returns>
        private static void DeleteOldRadio()
        {
            var repo = new Repository<Radio>();
            var count = 0;
            foreach (var radio in repo.GetList().OrderByDescending(a => a.LastListen))
            {
                count++;
                if (count > 10 && !radio.IsFavorite)
                    repo.Delete(radio);
            }
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
            if (IsNullOrWhiteSpace(xml)) return new List<Radio>();

            //parse the xml
            var items = from i in XDocument.Parse(xml).Descendants("station")
                        select new Radio
                        {
                            ShoutCastId = (string)i.Attribute("id"),
                            Name = (string)i.Attribute("name"),
                            LogoUrl = (string)i.Attribute("logo"),
                            Format = (string)i.Attribute("mt"),
                            Desrciption = (string)i.Attribute("ct"),
                            Genres = new List<Genre> { new Genre { Name = (string)i.Attribute("genre") } }
                        };

            return items.ToList();
        }

        #endregion Private Methods
    }
}