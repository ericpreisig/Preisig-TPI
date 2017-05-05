using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Entity;

namespace Presentation.Helper
{
    public static class Context
    {
        public static List<Track> ReadingList= new List<Track>();

        public static void AddToReadingList(Track track)
        {
            throw new NotImplementedException();
        }

        public static void RemoveFromReadingList(Track track)
        {
            throw new NotImplementedException();
        }
    }
}
