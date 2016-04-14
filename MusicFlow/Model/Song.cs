using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicFlow.Model
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string AlbumArtist { get; set; }
        public string Year { get; set; }
        public string TrackNo { get; set; }
        public string Genre { get; set; }
        public string Length { get; set; }
        public string SongFile { get; set; }
        public string AlbumCover;

    }  
    
}
