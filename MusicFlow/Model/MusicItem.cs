﻿using System;
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

    public class MusicHelper
    {
        static MediaPlayer Player
        {
            get{ return MyMediaPlayer.Instance.Player;}
        }
        static MediaPlaybackList NowPlayingList
        {
            get { return Player.Source as MediaPlaybackList; }
        }

        public static async Task<MediaPlaybackItem> ToMediaItem(MusicItem m)
        {
            var file = await StorageFile.GetFileFromPathAsync(m.FilePath);
            var stream =await file.OpenAsync(FileAccessMode.Read);
            var source = MediaSource.CreateFromStream(stream,file.ContentType);
            source.CustomProperties["FilePath"] = m.FilePath;
            source.CustomProperties["CoverImagePath"] = m.CoveImagePath;
            var item = new MediaPlaybackItem(source);

            var displayproperties = item.GetDisplayProperties();
            displayproperties.Type = MediaPlaybackType.Music;
            displayproperties.MusicProperties.Title = m.Title;
            displayproperties.MusicProperties.Artist = m.Artist;
            displayproperties.MusicProperties.AlbumTitle = m.Album;
            if (m.CoveImagePath != null)
                displayproperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(m.CoveImagePath));
            item.ApplyDisplayProperties(displayproperties);

            return item;
        }

        public async static void Play(MusicItem m)
        {
            var media = await ToMediaItem(m);
            NowPlayingList.Items.Clear();
            NowPlayingList.Items.Add(media);
            Player.Play();
            
        }

        public async static void AddToNowPlaying(MusicItem m)
        {
            var media = await ToMediaItem(m);
            NowPlayingList.Items.Add(media);
            Player.Pause();
            Player.Play();
            
        }
    }
}
