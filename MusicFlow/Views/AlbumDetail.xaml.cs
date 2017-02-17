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
using Windows.Media.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicFlow.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumDetail : Page
    {

        
        IEnumerable<MusicItem> songs;
        MusicItem song1;
        MainPage mp = (Window.Current.Content as Frame).Content as MainPage;
        MediaPlayer Player => MyMediaPlayer.Instance.Player;
        MediaPlaybackList NowPlayingList => MyMediaPlayer.Instance.Player.Source as MediaPlaybackList;

        public AlbumDetail()
        {
            this.InitializeComponent();
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var album = e.Parameter as string;
            //await Task.Run(() => {
                songs = mp.MusicList.Where(i => i.Album == album).Select(i => i);
                song1 = songs.FirstOrDefault();
            //});            
            
            //setupVisibility();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {           
            var list = songs.ToList();
            var index = (uint)list.IndexOf(e.ClickedItem as MusicItem);
            MusicHelper.PlayAlbum(songs,index);

            //OLD
            //var ci = (MusicItem)e.ClickedItem;
            //MusicHelper.Play(ci);
        }
        
        private void B1_Click(object sender, RoutedEventArgs e)
        {
            var media = (sender as Button).DataContext as MusicItem;
            MusicHelper.AddToNowPlaying(media);
           
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


        //Animations

        private Compositor _compositor;

        private void gridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int index = args.ItemIndex;
            var root = args.ItemContainer.ContentTemplateRoot as UserControl;
            var item = args.Item as MusicItem;

            if (!args.InRecycleQueue)
            {

                args.ItemContainer.Loaded += ItemConainer_Loaded;

                //Resize animation
                var visual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
                var comp = visual.Compositor;

                var offsetAnimation = comp.CreateVector3KeyFrameAnimation();
                offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);
                offsetAnimation.Target = "Offset";
                var aniGroup = comp.CreateAnimationGroup();
                aniGroup.Add(offsetAnimation);

                var implicitAnimationMap = comp.CreateImplicitAnimationCollection();
                implicitAnimationMap.Add("Offset", aniGroup);

                visual.ImplicitAnimations = implicitAnimationMap;
            }
            args.Handled = true;
        }

        private void ItemConainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemPanel = gridView.ItemsPanelRoot as ItemsWrapGrid;
            var itemContainer = sender as GridViewItem;
            var itemIndex = gridView.IndexFromContainer(itemContainer);


            if (itemIndex >= itemPanel.FirstVisibleIndex && itemIndex <= itemPanel.LastVisibleIndex)
            {
                //Loading animation
                var itemVisual = ElementCompositionPreview.GetElementVisual(itemContainer);

                float width = 200;
                float height = 200;
                itemVisual.Size = new Vector2(width, height);
                itemVisual.CenterPoint = new Vector3(width / 2, height / 2, 0f);
                itemVisual.Opacity = 0.0f;
                //itemVisual.Offset = new Vector3(0, -400, 0);

                Vector3KeyFrameAnimation scalAnimation = _compositor.CreateVector3KeyFrameAnimation();
                scalAnimation.InsertKeyFrame(0f, new Vector3(0.9f, 0.9f, .9f));
                scalAnimation.InsertKeyFrame(1f, new Vector3(1f, 1f, 0f));
                scalAnimation.Duration = TimeSpan.FromMilliseconds(600);                
                scalAnimation.DelayTime = TimeSpan.FromMilliseconds((itemIndex - itemPanel.FirstVisibleIndex) * 20);

                //var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
                //offsetAnimation.InsertExpressionKeyFrame(1f, "0");
                //offsetAnimation.Duration = TimeSpan.FromMilliseconds(1250);
                //offsetAnimation.DelayTime = TimeSpan.FromMilliseconds((itemIndex) * 20);

                KeyFrameAnimation fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertExpressionKeyFrame(1f, "1");
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(600);
                fadeAnimation.DelayTime = TimeSpan.FromMilliseconds((itemIndex - itemPanel.FirstVisibleIndex) * 20);

                itemVisual.StartAnimation("Scale", scalAnimation);
                //itemVisual.StartAnimation("Offset.Y", offsetAnimation);
                itemVisual.StartAnimation("Opacity", fadeAnimation);
            }
            itemContainer.Loaded -= ItemConainer_Loaded;            
        }

        private void AlbumInfo_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var visual = ElementCompositionPreview.GetElementVisual(grid);
            var compositor = visual.Compositor;

            visual.Size = new Vector2((float)grid.ActualWidth, (float)grid.ActualHeight);
            visual.CenterPoint = new Vector3((float)grid.ActualWidth / 2,(float)grid.ActualHeight/2,0f);

            var animation = compositor.CreateVector3KeyFrameAnimation();
            animation.InsertKeyFrame(0f, new Vector3(.9f));
            animation.InsertKeyFrame(1f, new Vector3(1f));
            animation.Duration = TimeSpan.FromMilliseconds(300);
            visual.StartAnimation("Scale", animation);
        }

        private void setupVisibility()
        {
            if (song1.Genre == "")
            {
                dot.Visibility = Visibility.Collapsed;
            }
        }        
    }
}
