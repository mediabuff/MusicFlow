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
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicFlow.Views
{
    public sealed partial class AlbumView : Page
    {
        ObservableCollection<MusicItem> myMusic;
        IEnumerable<MusicItem> albumList;        
        MainPage mp = (Window.Current.Content as Frame).Content as MainPage;

        public AlbumView()
        {
            this.InitializeComponent();
            InitializeCompositor();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {           
            myMusic = (ObservableCollection<MusicItem>)e.Parameter;
            albumList = myMusic.GroupBy(i=>i.Album).Select(i=>i.FirstOrDefault()).OrderBy(i=>i.Album);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var SC = FindFirstChild<ScrollViewer>(albumView) as ScrollViewer;
            (App.Current as App).ScrollPosition = SC.VerticalOffset;
        }


        //Gridview

        private void albumView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (MusicItem)e.ClickedItem;
            Frame.Navigate(typeof(AlbumDetail), clickedItem.Album);
        }       
       
        private void albumView_Loaded(object sender, RoutedEventArgs e)
        {
            var SC = FindFirstChild<ScrollViewer>(albumView) as ScrollViewer;
            if ((App.Current as App).ScrollPosition != 0)
                SC.ScrollToVerticalOffset((App.Current as App).ScrollPosition);
        }        


        //Mouse hover play button
        private void albumgrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var grid = sender as Grid;
            (grid.Children[2] as Grid).Visibility = Visibility.Visible;
        }

        private void albumgrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var RelPan = sender as Grid;
            (RelPan.Children[2] as Grid).Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var song = (sender as Button).DataContext as MusicItem;
            //MyMediaPlayer.playAlbum(song.Album);
        }


        //Drag and Drop
        private void albumView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            //var SelectedSong = e.Items[0] as Song;
            //e.Data.SetText(SelectedSong.ToString()+"albm");
            //e.Data.RequestedOperation = DataPackageOperation.Copy;
        }


        //Animations
        private Compositor _compositor;

        private void InitializeCompositor()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        private void ItemConainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemPanel = albumView.ItemsPanelRoot as ItemsWrapGrid;
            var itemContainer = sender as GridViewItem;
            var itemIndex = albumView.IndexFromContainer(itemContainer);
            if (itemIndex >= itemPanel.FirstVisibleIndex && itemIndex <= itemPanel.LastVisibleIndex)
            {
                //Loading animation
                var itemVisual = ElementCompositionPreview.GetElementVisual(itemContainer);

                float width = 200;
                float height = 200;
                itemVisual.Size = new Vector2(width, height);
                itemVisual.CenterPoint = new Vector3(width / 2, height / 2, 0f);
                itemVisual.Opacity = 0.0f;
                
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

                itemVisual.StartAnimation("Scale", scalAnimation);
                itemVisual.StartAnimation("Opacity", fadeAnimation);
            }
            itemContainer.Loaded -= ItemConainer_Loaded;
        }

        private void AlbumView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int index = args.ItemIndex;
            var root = args.ItemContainer.ContentTemplateRoot as UserControl;
            

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


        //Helper methos
        DependencyObject FindFirstChild<T>(DependencyObject initial)
        {
            DependencyObject current = initial;
            if (current == null)
                return null;
            if (current.GetType() == typeof(T))
                return current;
            else
            {
                var count = VisualTreeHelper.GetChildrenCount(current);
                DependencyObject result = null;
                for (int i = 0; i < count; i++)
                {
                    result = FindFirstChild<T>(VisualTreeHelper.GetChild(current, i));
                    if (result != null)
                        break;
                }
                return result;
            }

        }
        
    }
}
