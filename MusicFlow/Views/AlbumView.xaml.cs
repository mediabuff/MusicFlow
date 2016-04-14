using MusicFlow.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Effects;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicFlow.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumView : Page
    {
        ObservableCollection<Song> myMusic;
        IEnumerable<Song> albumList;
        private Compositor _compositor;
      
        public AlbumView()
        {
            this.InitializeComponent();
            InitializeCompositor();
        }

        private void InitializeCompositor()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {           
            myMusic = (ObservableCollection<Song>)e.Parameter;
            albumList = myMusic.GroupBy(i=>i.Album).Select(i=>i.FirstOrDefault()).OrderBy(i=>i.Album);           
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {           
            var itemPanel = albumView.ItemsPanelRoot as ItemsWrapGrid;
        }

        private void albumView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (Song)e.ClickedItem;
            var al = clickedItem.Album;
            var selectedAlbum = myMusic.Where(i => i.Album == al).Select(i=>i);
            Frame.Navigate(typeof(AlbumDetail), selectedAlbum);
        }

        private void albumView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int index = args.ItemIndex;
            var root = args.ItemContainer.ContentTemplateRoot as UserControl;
            var item = args.Item as Song;

            if (!args.InRecycleQueue)
                args.ItemContainer.Loaded += ItemConainer_Loaded;
            args.Handled = true;
        }

        private void ItemConainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemPanel = albumView.ItemsPanelRoot as ItemsWrapGrid;
            var itemContainer = sender as GridViewItem;
            var itemIndex = albumView.IndexFromContainer(itemContainer);
          

            if(itemIndex>=itemPanel.FirstVisibleIndex && itemIndex <= itemPanel.LastVisibleIndex)
            {
                var itemVisual = ElementCompositionPreview.GetElementVisual(itemContainer);
                
                float width = 200;
                float height = 200;
                itemVisual.Size = new Vector2(width, height);
                itemVisual.CenterPoint = new Vector3(width/2, height / 2 ,0f);
                itemVisual.Opacity = 0.0f;
                //itemVisual.Offset = new Vector3(0, 100, 0);

                //KeyFrameAnimation offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
                //offsetAnimation.InsertExpressionKeyFrame(0f, "70");
                //offsetAnimation.InsertExpressionKeyFrame(1f, "0");
                //offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);
                //offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(itemIndex * 50);

                Vector3KeyFrameAnimation scalAnimation = _compositor.CreateVector3KeyFrameAnimation();
                scalAnimation.InsertKeyFrame(0f, new Vector3(0.9f, 0.9f, .9f));
                scalAnimation.InsertKeyFrame(1f, new Vector3(1f, 1f, 0f));
                scalAnimation.Duration = TimeSpan.FromMilliseconds(600);
                var x = itemIndex - itemPanel.FirstVisibleIndex;
                scalAnimation.DelayTime = TimeSpan.FromMilliseconds((itemIndex - itemPanel.FirstVisibleIndex) * 20);

                KeyFrameAnimation fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertExpressionKeyFrame(1f, "1");
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(600);
                fadeAnimation.DelayTime = TimeSpan.FromMilliseconds((itemIndex - itemPanel.FirstVisibleIndex) * 20);

                //itemVisual.StartAnimation("Offset.Y", offsetAnimation);
                itemVisual.StartAnimation("Scale", scalAnimation);
                itemVisual.StartAnimation("Opacity", fadeAnimation);
            }
            itemContainer.Loaded -= ItemConainer_Loaded;
        }

        private void albumView_Loaded(object sender, RoutedEventArgs e)
        {

        }

      
    }
}
