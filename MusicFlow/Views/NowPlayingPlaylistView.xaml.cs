using MusicFlow.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MusicFlow.Views
{
    public sealed partial class NowPlayingPlaylistView : UserControl
    {
        MediaPlayer Player => MyMediaPlayer.Instance.Player; 

        public NowPlayingPlaylistView()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void NowPlayingListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var x = e.ClickedItem as MediaPlaybackItem;
            MusicHelper.PlayFromNowplaying(x);
        }

        public void UpdateSource(List<MediaPlaybackItem> list)
        {
            try
            {
                var i = NowPlayingListView.SelectedIndex;
                NowPlayingListView.ItemsSource = list;
                NowPlayingListView.SelectedIndex = i;
            }
            catch { }
        }
       
        public void UpdateSelectedIndex()
        {
            try
            {
                NowPlayingListView.SelectedIndex = (int)(Player.Source as MediaPlaybackList).CurrentItemIndex;
            }
            catch { }
        }
    }
}
