using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzConversations.Mobile.Entities
{
    public class Album
    {
        public string AlbumTitle { get; set; }
        public string AlbumArt { get; set; }
        public string AlbumURL { get; set; }

        public Album(string album, string art, string url)
        {
            this.AlbumTitle = album;
            this.AlbumArt = art;
            this.AlbumURL = url;
        }
    }
}
