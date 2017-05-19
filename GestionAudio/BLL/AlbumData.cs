using DAL.Database;
using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public static class AlbumData
    {
        #region Public Methods

        public static void AddOrUpdateAlbum()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if a album exist by it's name
        /// </summary>
        public static bool CheckIfAlbumExist(string name, string artistName) => new Repository<Album>().GetList().Any(a => a.Name.ToLower() == name.ToLower() && a.Artist.Name == artistName);

        /// <summary>
        /// Get all albums from thge db
        /// </summary>
        /// <returns></returns>
        public static List<Album> GetAlbums() => new Repository<Album>().GetList();

        public static void RemoveAlbum()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}