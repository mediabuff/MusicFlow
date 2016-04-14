using MusicFlow.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicFlow.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumDetail : Page
    {

        
        IEnumerable<Song> songs;
        Song song1;
        MainPage mp = (Window.Current.Content as Frame).Content as MainPage;  

        public AlbumDetail()
        {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            songs = e.Parameter as IEnumerable<Song>;
            song1 =  songs.FirstOrDefault();            
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ci = (Song)e.ClickedItem;
            MyMediaPlayer.nowPlayingList.Clear();
            MyMediaPlayer.nowPlayingList.AddFirst(ci);
            MyMediaPlayer.playSong(ci);
            mp.updateNPList();
        }

      

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.addToNowPlaying(((sender as Button).DataContext) as Song);
            mp.updateNPList();
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var x = ((sender as Grid).Children[1] as RelativePanel).Children[2] as Button;
            x.Visibility = Visibility.Visible;
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var x = ((sender as Grid).Children[1] as RelativePanel).Children[2] as Button;
            x.Visibility = Visibility.Collapsed;
        }
    }
}
