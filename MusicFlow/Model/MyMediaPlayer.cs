using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicFlow.Model
{
    class MyMediaPlayer
    {
        public static MediaPlayer mp;
        public static LinkedList<Song> nowPlayingList;
        public static Song currentSong;

        public MyMediaPlayer()
        {
            mp = new MediaPlayer();
            nowPlayingList = new LinkedList<Song>();
            currentSong = new Song();
            mp.AutoPlay = true;
            mp.MediaEnded += MediaEnded;
            mp.SystemMediaTransportControls.ButtonPressed += smtcButtonPressed;
            mp.AudioCategory = MediaPlayerAudioCategory.Media;
        }

        private void MediaEnded(MediaPlayer sender, object args)
        {
            if (currentSong != nowPlayingList.Last.Value)
            {
                playNextSong();
            }

        }

        private void smtcButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            
        }

        
        //Play Song
        public static async void playSong(Song ci)
        {
            mp.SetFileSource(await StorageFile.GetFileFromPathAsync(ci.SongFile));
            mp.Play();            
            currentSong = ci;
            updateSMTC(ci);
            MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
            mainpage.PlayButton.Content = "";
            mainpage.setupMediaPlayer(ci.AlbumCover, ci.Title, ci.Album);
            mainpage.bgImage.Source = new BitmapImage(new Uri(ci.AlbumCover));
            mainpage.animateBackGround();
        }

        public static async void updateSMTC(Song s1)
        {
            var smtc = mp.SystemMediaTransportControls;
            var f = await StorageFile.GetFileFromPathAsync(s1.SongFile);
            await smtc.DisplayUpdater.CopyFromFileAsync(Windows.Media.MediaPlaybackType.Music, f);
            smtc.IsNextEnabled = true;
            smtc.IsFastForwardEnabled = true;
            smtc.IsStopEnabled = true;
            smtc.DisplayUpdater.Update();
        }


        //Button Events
        public static void PlayPause()
        {
            MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
            if (mp.CurrentState == MediaPlayerState.Playing)
            {
                mp.Pause();
                mainpage.PlayButton.Content = "";
            }
            else if (mp.CurrentState == MediaPlayerState.Paused)
            {
                mp.Play();
                mainpage.PlayButton.Content = "";
            }
        }

        public static void playNextSong()
        {
            if (currentSong.Length != null)
            {
                if (nowPlayingList.Find(currentSong).Value != nowPlayingList.Last.Value)
                {
                    currentSong = nowPlayingList.Find(currentSong).Next.Value;
                    playSong(currentSong);
                }
                else if (nowPlayingList.Count() == 1 || nowPlayingList.Find(currentSong).Value == nowPlayingList.Last.Previous.Value)
                {
                    MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
                    mainpage.disableNextButton();
                }
            }
        }

        public static void playPreviousSong()
        {
            if (currentSong.Length != null)
            {
                if (nowPlayingList.Find(currentSong).Value != nowPlayingList.First.Value)
                {
                    currentSong = nowPlayingList.Find(currentSong).Previous.Value;
                    playSong(currentSong);
                }
                else if (nowPlayingList.Count() == 1 || nowPlayingList.Find(currentSong).Value == nowPlayingList.Last.Previous.Value)
                {
                    MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
                    mainpage.disableNextButton();
                }
            }            
        }

        public static void addToNowPlaying(Song s)
        {
            MainPage mainpage = (Window.Current.Content as Frame).Content as MainPage;
            nowPlayingList.AddLast(s);
            if(nowPlayingList.Count() == 1)
            {
                playSong(s);
                mainpage.disableNextButton();
            }
            else
            {
                mainpage.enableNextButton();
            }
        }         
    }
}
