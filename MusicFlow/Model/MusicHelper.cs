using MusicFlow.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MusicFlow.Model
{
    public class MusicHelper
    {
        static MediaPlayer Player
        {
            get { return MyMediaPlayer.Instance.Player; }
        }
        static MediaPlaybackList NowPlayingList
        {
            get { return Player.Source as MediaPlaybackList; }
        }

        public static async Task<MediaPlaybackItem> ToMediaItem(MusicItem m)
        {
            var file = await StorageFile.GetFileFromPathAsync(m.FilePath);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            var source = MediaSource.CreateFromStream(stream, file.ContentType);
            source.CustomProperties["FilePath"] = m.FilePath;
            source.CustomProperties["CoverImagePath"] = m.CoveImagePath;
            source.CustomProperties["Title"] = m.Title;
            source.CustomProperties["Artist"] = m.Artist;
            source.CustomProperties["Album"] = m.Album;
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

            //Update mtc playlist
            MainPage mp = (Window.Current.Content as Frame).Content as MainPage;
            mp.UpdateNowplayingListViewSource();
        }

        public async static void AddToNowPlaying(MusicItem m)
        {
            var media = await ToMediaItem(m);
            NowPlayingList.Items.Add(media);
            Player.Play();
            Player.SystemMediaTransportControls.IsNextEnabled = true;

            //Update mtc playlist
            MainPage mp = (Window.Current.Content as Frame).Content as MainPage;
            mp.UpdateNowplayingListViewSource();
        }

        public static void PlayFromNowplaying(MediaPlaybackItem e)
        {
            var index = (uint) NowPlayingList.Items.IndexOf(e);
            NowPlayingList.MoveTo(index);
            Player.Play();

            //Update mtc playlist
            //MainPage mp = (Window.Current.Content as Frame).Content as MainPage;
            //mp.UpdateNowplayingListViewSource();
        }
    }
}
