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
        public NowPlayingPlaylistView()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {           
            this.Height = 500;
            this.Width = 400;
        }

        public void UpdateSource(List<MediaPlaybackItem> list)
        {
            NowPlayingListView.ItemsSource = list;
        }
    }
}
