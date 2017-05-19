using DAL.Database;
using DTO;
using DTO.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public static class GeneralData
    {
        #region Public Methods
     
        /// <summary>
        /// Add or update an audio element
        /// </summary>
        /// <param name="audio"></param>
        public static void AddOrUpdateAudio(this Audio audio)
        {
            if (audio is Track)
                new Repository<Track>().AddOrUpdate((Track)audio);
            else if (audio is Radio)
                new Repository<Radio>().AddOrUpdate((Radio)audio);
        }

        /// <summary>
        /// Check if a genre exist by it's name
        /// </summary>
        public static bool CheckIfGenreExist(string name) => new Repository<Genre>().GetList().Any(a => a.Name.ToLower() == name.ToLower());

        /// <summary>
        /// Get or create a application context
        /// if context does exist, else, create it
        /// </summary>
        /// <returns></returns>
        public static Context GetContext()=> new Repository<Context>().GetList().Any() ? new Repository<Context>().GetList().FirstOrDefault() : new Context();
        

        /// <summary>
        /// Get all genre from thge db
        /// </summary>
        /// <returns></returns>
        public static List<Genre> GetGenres() => new Repository<Genre>().GetList();

        /// <summary>
        /// Get all folder to sync
        /// </summary>
        public static List<IncludeFolder> GetIncludedFolder()=>new Repository<IncludeFolder>().GetList();

        /// <summary>
        /// remove a folder from the sync list
        /// </summary>
        public static void RemoveIncludeFolder(this IncludeFolder folder)
        {
            new Repository<IncludeFolder>().Delete(folder);
        } 


        /// <summary>
        /// add a folder from the sync list
        /// </summary>
        public static void AddIncludeFolder(this IncludeFolder folder)
        {
            if(new Repository<IncludeFolder>().GetList().All(a => a.Path != folder.Path))
                new Repository<IncludeFolder>().AddOrUpdate(folder);
        } 

        /// <summary>
        /// Save the context
        /// </summary>
        /// <param name="context"></param>
        public static void SetContext(this Context context)=>new Repository<Context>().AddOrUpdate(context);

        #endregion Public Methods
    }
}