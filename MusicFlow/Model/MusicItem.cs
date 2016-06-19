using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace MusicFlow.Model
{    
    public class MusicItem 
    {        
        public string FilePath;       
        public string CoveImagePath;
        public string Album;
        public string AlbumArtist;
        public string Artist;
        public TimeSpan Duration;
        public string Genre;
        public string Title;
        public string TrackNumber;
        public string Year;
    }
}
