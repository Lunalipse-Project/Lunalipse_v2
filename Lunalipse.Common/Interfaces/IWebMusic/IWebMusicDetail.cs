﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IWebMusic
{
    public interface IWebMusicDetail
    {
        string getID();
        string getMusicName();
        string getAlbumName();
        string getAlbumPicture();
        string getArtistName();
        double getTotalSeconds();
    }
}
