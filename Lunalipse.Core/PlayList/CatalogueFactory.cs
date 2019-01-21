using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IPlayList;

namespace Lunalipse.Core.PlayList
{
    public class CatalogueFactory
    {
        public static ICatalogue Create(string Name)
        {
            return new Catalogue(Name);
        }
        public static ICatalogue CreateAlbum(string Name)
        {
            return new Catalogue(Name)
            {
                isAlbumClassified = true
            };
        }
        public static ICatalogue CreateArtist(string Name)
        {
            return new Catalogue(Name)
            {
                isArtistClassified = true
            };
        }
        public static ICatalogue CreateUser(string Name)
        {
            return new Catalogue(Name)
            {
                isUserDefined = true
            };
        }
    }
}
