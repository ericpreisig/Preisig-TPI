using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Database;
using DTO;
using DTO.Entity;

namespace BLL
{
    public static class GeneralData
    {
        public static List<Genre> GetMoreListenedGenre()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all genre from thge db
        /// </summary>
        /// <returns></returns>
        public static List<Genre> GetGenres() => new Repository<Genre>().GetList();

        /// <summary>
        /// Check if a genre exist by it's name
        /// </summary>
        public static bool CheckIfGenreExist(string name)=> new Repository<Genre>().GetList().Any(a => a.Name.ToLower() == name.ToLower());

        /// <summary>
        /// Add one time to the listen times in genre table
        /// </summary>
        public static void AddListen()
        {
            throw new NotImplementedException();
        }
        public static void AddExludeFolder()
        {
            throw new NotImplementedException();
        }

        public static void RemoveExludeFolder()
        {
            throw new NotImplementedException();
        }

        public static void IsFolderExclude()
        {
            throw new NotImplementedException();
        }

        public static void GetConfig()
        {
            throw new NotImplementedException();
        }

        public static void SetConfig()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add or update an audio element
        /// </summary>
        /// <param name="audio"></param>
        public static void AddOrUpdateAudio(this Audio audio)
        {
            if(audio is Track)
                new Repository<Track>().AddOrUpdate((Track) audio);
            else if (audio is Radio)
                new Repository<Radio>().AddOrUpdate((Radio) audio);
        }

        /// <summary>
        /// Get or create a application context
        /// </summary>
        /// <returns></returns>
        public static Context GetContext()
        {
            Context context = null;
            //if context does exist, else, create it
            context = new Repository<Context>().GetList().Any() ? new Repository<Context>().GetList().FirstOrDefault() : new Context();

            return context;
        }


        public static void SetContext()
        {
            throw new NotImplementedException();
        }
    }
}
