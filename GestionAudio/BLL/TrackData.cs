/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Handle action with tracks
*********************************************************************************/

using DAL.Database;
using DTO.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BLL
{
    /// <summary>
    /// Contain all action usabl e by tracks
    /// </summary>
    public static class TrackData
    {
        #region Public Methods

        /// <summary>
        /// Return true  if a track is already in the database
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckIfAlreadyInDatabase(string path) => GetTrackByPath(path) != null;

        /// <summary>
        /// Check if there is no track a the database
        /// </summary>
        /// <returns></returns>
        public static bool CheckIfDatabaseEmpty() => !new Repository<Track>().Table.Any();

        /// <summary>
        /// Get a track by his path
        /// </summary>
        /// <returns></returns>
        public static Track GetTrackByPath(string path) => new Repository<Track>().GetList().FirstOrDefault(a => Path.GetFullPath(a.Path.ToLower()) == Path.GetFullPath(path.ToLower()));

        /// <summary>
        /// Get all tracks from the db
        /// </summary>
        /// <returns></returns>
        public static List<Track> GetTracks() => new Repository<Track>().GetList();

        /// <summary>
        /// Remove track from database, delete album and artist if empty
        /// </summary>
        public static void RemoveTrack(this Track track)
        {
            var albumId = track.Album.ID;
            var artistId = track.Album.Artist.ID;
            new Repository<Track>().Delete(track);
            var albumRepo = new Repository<Album>();
            if (albumRepo.GetById(albumId).Tracks.Count == 0) albumRepo.Delete(albumRepo.GetById(albumId));
            var artistRepo = new Repository<Artist>();
            if (artistRepo.GetById(artistId).Albums.Count == 0) artistRepo.Delete(artistRepo.GetById(artistId));
        }

        /// <summary>
        /// Update a track with new track infos
        /// </summary>
        public static Track UpdateTrackInfo(this Track oldTrack, Track newTrack)
        {
            oldTrack.Album = newTrack.Album;
            oldTrack.Album.Artist = newTrack.Album.Artist;
            oldTrack.Genres = newTrack.Genres;
            oldTrack.Name = newTrack.Name;
            oldTrack.Duration = newTrack.Duration;
            return oldTrack;
        }

        #endregion Public Methods
    }
}