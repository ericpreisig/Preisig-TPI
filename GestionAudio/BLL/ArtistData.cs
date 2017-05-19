using DAL.Database;
using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public static class ArtistData
    {
        #region Public Methods

        public static void AddOrUpdateArtist()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if a artist exist by it's name
        /// </summary>
        public static bool CheckIfArtistExist(string name) => new Repository<Artist>().GetList().Any(a => a.Name.ToLower() == name.ToLower());

        /// <summary>
        /// Get all artists from the db
        /// </summary>
        /// <returns></returns>
        public static List<Artist> GetArtists() => new Repository<Artist>().GetList();

        public static void RemoveArtist()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}