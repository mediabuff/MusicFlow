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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicFlow.Model
{
    class MyMediaPlayer
    {
        static MyMediaPlayer instance;

        public static MyMediaPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyMediaPlayer();                    
                }
                return instance;
            }
        }

        public MediaPlayer Player { get; private set; }

      
        public MyMediaPlayer()
        {
            Player = new MediaPlayer();
            Player.AutoPlay = false;
            Player.Pause();
            Player.Source = new MediaPlaybackList();
            (Player.Source as MediaPlaybackList).CurrentItemChanged += MyMediaPlayer_CurrentItemChanged;
            Player.SystemMediaTransportControls.IsPauseEnabled = true;
        }

        private async void MyMediaPlayer_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender.CurrentItem != null)
                {
                    var cover = sender.CurrentItem.Source.CustomProperties["CoverImagePath"] as string;
                    var title = sender.CurrentItem.Source.CustomProperties["Title"] as string;
                    MainPage mp = (Window.Current.Content as Frame).Content as MainPage;
                    mp.animateBackGround(cover);
                    mp.animateMeidaTransportControl(title,cover);
                }
                
            });
        }








        //public MyMediaPlayer()
        //{
        //    //MediaPlayer = new MediaPlayer();
        //    //nowPlayingList = new LinkedList<Song>();
        //    //currentSong = new Song();
        //    //MediaPlayer.AutoPlay = true;
        //    //MediaPlayer.MediaEnded += MediaEnded;
        //    //mediaplayer.SystemMediaTransportControls.ButtonPressed += smtcButtonPressed;
        //    //mediaplayer.AudioCategory = MediaPlayerAudioCategory.Media;            
        //}       

        //private void MediaEnded(MediaPlayer sender, object args)
        //{
        //    if (currentSong != NowPlayingList.Last.Value)
        //    {
        //        playNextSong();
        //    }

        //}

        //private void smtcButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        //{
        //}


        //Play Song
        //public void playSong(MediaPlaybackItem ci)
        //{            
        //    MediaPlayer.Source = ci;
        //    MediaPlayer.Play();                 
        //    //updateSMTC(ci);
        //    //MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
        //    //mainpage.PlayButton.Content = "";
        //    //.setupMediaPlayer(ci.AlbumCover, ci.Title, ci.Album);           
        //}

        //public async void updateSMTC(MusicItem s1)
        //{
        //    var smtc = MediaPlayer.SystemMediaTransportControls;
        //    var f = await StorageFile.GetFileFromPathAsync(s1.FilePath);
        //    await smtc.DisplayUpdater.CopyFromFileAsync(Windows.Media.MediaPlaybackType.Music, f);
        //    smtc.IsNextEnabled = true;
        //    smtc.IsFastForwardEnabled = true;
        //    smtc.IsStopEnabled = true;
        //    smtc.DisplayUpdater.Update();
        //}

        //public void playAlbum(string s1)
        //{
        //    var mp = (Window.Current.Content as Frame).Content as MainPage;
        //    var x = mp.MusicList.Where(i => i.Album == s1).ToList();
        //    NowPlayingList.Clear();
        //    foreach (var ss in x)
        //        NowPlayingList.AddLast(ss);
        //    playSong(x[0]);
        //    mp.updateNPList();
        //}

        //Button Events
        //public  void PlayPause()
        //{
        //    if (mediaPlayer.CurrentState == MediaPlayerState.Playing)
        //    {
        //        mediaPlayer.Pause();
        //    }
        //    else if (mediaPlayer.CurrentState == MediaPlayerState.Paused)
        //    {
        //        mediaPlayer.Play();
        //    }
        //}

        //public void playNextSong()
        //{
        //    if (currentSong.Length != null)
        //    {
        //        if (nowPlayingList.Find(currentSong).Value != nowPlayingList.Last.Value)
        //        {
        //            currentSong = nowPlayingList.Find(currentSong).Next.Value;
        //            playSong(currentSong);
        //        }
        //        else if (nowPlayingList.Count() == 1 || nowPlayingList.Find(currentSong).Value == nowPlayingList.Last.Previous.Value)
        //        {
        //            MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
        //            mainpage.disableNextButton();
        //        }
        //    }
        //}

        //public void playPreviousSong()
        //{
        //    if (currentSong.Length != null)
        //    {
        //        if (nowPlayingList.Find(currentSong).Value != nowPlayingList.First.Value)
        //        {
        //            currentSong = nowPlayingList.Find(currentSong).Previous.Value;
        //            playSong(currentSong);
        //        }
        //        else if (nowPlayingList.Count() == 1 || nowPlayingList.Find(currentSong).Value == nowPlayingList.Last.Previous.Value)
        //        {
        //            MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
        //            mainpage.disableNextButton();
        //        }
        //    }            
        //}

        //public void addToNowPlaying(Song s)
        //{
        //    MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
        //    nowPlayingList.AddLast(s);
        //    if(nowPlayingList.Count() == 1)
        //    {
        //        playSong(s);
        //        mainpage.disableNextButton();
        //    }
        //    else
        //    {
        //        mainpage.enableNextButton();
        //    }
        //}         
    }
}
