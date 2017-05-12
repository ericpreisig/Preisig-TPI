using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Database;
using DTO.Entity;

namespace BLL
{
    public static class ArtistData
    {

        /// <summary>
        /// Get all artists from the db
        /// </summary>
        /// <returns></returns>
        public static List<Artist> GetArtists() => new Repository<Artist>().GetList();

        public static void AddOrUpdateArtist()
        {
            throw new NotImplementedException();
        }

        public static void RemoveArtist()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if a artist exist by it's name
        /// </summary>
        public static bool CheckIfArtistExist(string name) => new Repository<Artist>().GetList().Any(a => a.Name.ToLower() == name.ToLower());
    }
}
