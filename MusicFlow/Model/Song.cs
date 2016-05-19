using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
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

        public override string ToString()
        {
            string x = "";
            x = Title + "\n" + Artist + "\n" + Album + "\n" + AlbumArtist + "\n" + Year + "\n" + TrackNo + "\n" + Genre + "\n" + Length + "\n" + SongFile + "\n" + AlbumCover;
            return x;
        }

        public static Song fromString(string x)
        {
            var s = new Song();
            var y = x.Split('\n');
            s.Title = y[0];
            s.Artist = y[1];
            s.Album = y[2];
            s.AlbumArtist = y[3];
            s.Year = y[4];
            s.TrackNo = y[5];
            s.Genre = y[6];
            s.Length = y[7];
            s.SongFile = y[8];
            s.AlbumCover = y[9];
            return s;
        }
    }  
    
}
