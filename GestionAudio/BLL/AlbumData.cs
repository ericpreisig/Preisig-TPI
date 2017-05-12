using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DAL.Database;
using DTO.Entity;

namespace BLL
{
    public static class AlbumData
    {
        /// <summary>
        /// Get all albums from thge db
        /// </summary>
        /// <returns></returns>
        public static List<Album> GetAlbums() => new Repository<Album>().GetList();

        public static void AddOrUpdateAlbum()
        {
            throw new NotImplementedException();
        }

        public static void RemoveAlbum()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if a album exist by it's name
        /// </summary>
        public static bool CheckIfAlbumExist(string name) => new Repository<Album>().GetList().Any(a => a.Name.ToLower() == name.ToLower());


    }
}
