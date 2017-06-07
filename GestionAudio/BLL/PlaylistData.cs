/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Handle action with playlists
*********************************************************************************/

using DAL.Database;
using DTO.Entity;
using System.Collections.Generic;

namespace BLL
{
    /// <summary>
    /// Contain all action usable by a playlist
    /// </summary>
    public static class PlaylistData
    {
        #region Public Methods

        /// <summary>
        /// add or update a playlist
        /// </summary>
        /// <param name="playlist"></param>
        public static void AddOrUpdatePlayist(this Playlist playlist) => new Repository<Playlist>().AddOrUpdate(playlist);

        /// <summary>
        /// Create a playlist
        /// </summary>
        /// <param name="playlist"></param>
        public static void CreatePlaylist(Playlist playlist) => new Repository<Playlist>().AddOrUpdate(playlist);

        /// <summary>
        /// Get all playlists
        /// </summary>
        /// <returns></returns>
        public static List<Playlist> GetPlaylists() => new Repository<Playlist>().GetList();

        /// <summary>
        /// Remove a playlist
        /// </summary>
        /// <param name="playlist"></param>
        public static void RemovePlaylist(Playlist playlist) => new Repository<Playlist>().Delete(playlist);

        #endregion Public Methods
    }
}