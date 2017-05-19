using DAL.Database;
using DTO.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BLL
{
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
        public static bool CheckIfDatabaseEmpty() => new Repository<Track>().GetList().Count == 0;

        public static Track CreateTrackWithInfo(Track track)
        {
            throw new NotImplementedException();
        }

        public static List<Track> Get10LastTracks()
        {
            throw new NotImplementedException();
        }

        public static List<Track> GetFavouriteTracks()
        {
            throw new NotImplementedException();
        }

        public static List<Track> GetSuggeredTracks(string genre)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a track by his path
        /// </summary>
        /// <returns></returns>
        public static Track GetTrackByPath(string path) => new Repository<Track>().GetList().FirstOrDefault(a => Path.GetFullPath(a.Path.ToLower()) == Path.GetFullPath(path.ToLower()));

        /// <summary>
        /// Get all tracks from thge db
        /// </summary>
        /// <returns></returns>
        public static List<Track> GetTracks() => new Repository<Track>().GetList();

        public static List<Track> GetTracksByGenre(string genre)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove track from database
        /// </summary>
        public static void RemoveTrack(this Track track)
        {
            new Repository<Track>().Delete(track);
        }

        /// <summary>
        /// Update a track with new track infos
        /// </summary>
        public static Track UpdateTrackInfo(this Track oldTrack, Track newTrack)
        {
            oldTrack.Duration = newTrack.Duration;
            oldTrack.Genre = newTrack.Genre;
            return oldTrack;
        }

        #endregion Public Methods
    }
}