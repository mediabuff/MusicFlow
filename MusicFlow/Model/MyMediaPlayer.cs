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
            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    if (sender.CurrentItem != null)
            //    {
            //        var cover = sender.CurrentItem.Source.CustomProperties["CoverImagePath"] as string;
            //        var title = sender.CurrentItem.Source.CustomProperties["Title"] as string;
            //        MainPage mp = (Window.Current.Content as Frame).Content as MainPage;
            //        mp.animateBackGround(cover);
            //        mp.animateMeidaTransportControl(title,cover);
            //        //mp.UpdateNowPlayingListViewSelectedIndex();
            //    }
                
            //});
        }
    }
}
