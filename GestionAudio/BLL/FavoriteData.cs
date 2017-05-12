using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using DTO.Entity;

namespace BLL
{
    public static class FavoriteData
    {
       
        /// <summary>
        /// Add an audio to favorite
        /// </summary>
        /// <param name="element"></param>
        public static void AddFavorite(Audio element)
        {
            element.IsFavorite = true;
            element.AddOrUpdateAudio();
        }

        /// <summary>
        /// remove an audio to favorite
        /// </summary>
        /// <param name="element"></param>
        public static void RemoveFavorite(Audio element)
        {
            element.IsFavorite = false;
            element.AddOrUpdateAudio();
        }
    }
}
