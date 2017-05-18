using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Database;
using DTO.Entity;

namespace BLL
{
    public static class PlaylistData
    {
        /// <summary>
        /// Get all playlists
        /// </summary>
        /// <returns></returns>
        public static List<Playlist> GetPlaylists()=> new Repository<Playlist>().GetList();

        /// <summary>
        /// Create a playlist
        /// </summary>
        /// <param name="playlist"></param>
        public static void CreatePlaylist(Playlist playlist)=> new Repository<Playlist>().AddOrUpdate(playlist);

        /// <summary>
        /// Remove a playlist
        /// </summary>
        /// <param name="playlist"></param>
        public static void RemovePlaylist(Playlist playlist)=>new Repository<Playlist>().Delete(playlist);

        /// <summary>
        /// add or update a playlist
        /// </summary>
        /// <param name="playlist"></param>
        public static void AddOrUpdatePlayist(this Playlist playlist) => new Repository<Playlist>().AddOrUpdate(playlist);
    }
}
