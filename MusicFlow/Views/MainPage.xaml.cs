using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI.Composition.Toolkit;
using MusicFlow.Model;
using MusicFlow.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicFlow.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<MusicItem> MusicList;
        public static MediaPlayerElement mpElement;

        NowPlayingPlaylistView MainPageNowPlayingListView = new NowPlayingPlaylistView();
        MediaPlayer Player => MyMediaPlayer.Instance.Player;
        MediaPlaybackList NowPlayingList
        {
            get { return Player.Source as MediaPlaybackList; }
            set { Player.Source = value; }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SetTitleBar();
            SetBackNavigation();
            SetMediaTransportControls();
            await GetMusic();
            MainFrame.Navigate(typeof(Views.AlbumView), MusicList);
        }


        #region Music scanning algorithm

        public async Task GetMusic()
        {
            try
            {
                MusicList = await deserialize();
            }
            catch { }
            if (MusicList == null)
            {
                Progressbar.ShowPaused = false;
                var folder = KnownFolders.MusicLibrary;
                var songfilelist = new ObservableCollection<StorageFile>();
                await initCollection(folder, songfilelist);
                MusicList = await createSongList(songfilelist);
                await serialize(MusicList);
                Progressbar.ShowPaused = true;
            }
        }

        //Get list of StorageFile inside Music Library
        private async Task initCollection(StorageFolder folder, ObservableCollection<StorageFile> songfilelist)
        {
            foreach (var item in await folder.GetFilesAsync())
            {
                if (item.FileType == ".mp3" || item.FileType == ".m4a")
                    songfilelist.Add(item);
            }
            foreach (var item in await folder.GetFoldersAsync())
            {
                await initCollection(item, songfilelist);
            }
        }

        //Scan metadata from music files
        private async Task<ObservableCollection<MusicItem>> createSongList(ObservableCollection<StorageFile> songfilelist)
        {
            StorageFolder localFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("CoverArt", CreationCollisionOption.OpenIfExists);

            var SongList = new ObservableCollection<MusicItem>();

            foreach (var p in songfilelist)
            {
                var mItem = new MusicItem();
                var mp = await p.Properties.GetMusicPropertiesAsync();
                mItem.Artist = mp.Artist;
                mItem.Album = mp.Album;
                mItem.AlbumArtist = mp.AlbumArtist;
                mItem.Duration = mp.Duration;
                mItem.Genre = mp.Genre != null ? mp.Genre[0].ToString() : "Unknown";
                mItem.Title = mp.Title;
                mItem.TrackNumber = mp.TrackNumber.ToString();
                mItem.Year = mp.Year.ToString();
                mItem.FilePath = p.Path;
                mItem.CoveImagePath = string.Format("ms-appdata:///local/CoverArt/Cover_{0}_{1}.jpg", mItem.Album, mItem.Artist);
                SongList.Add(mItem);

                StorageItemThumbnail currentThumb = await p.GetThumbnailAsync(ThumbnailMode.MusicView, 200, ThumbnailOptions.UseCurrentScale);
                var fname = string.Format("Cover_{0}_{1}.jpg", mItem.Album, mItem.Artist);
                if (!File.Exists(fname))
                {
                    try
                    {
                        var file = await localFolder.CreateFileAsync(fname);
                        Windows.Storage.Streams.Buffer buff = new Windows.Storage.Streams.Buffer(Convert.ToUInt32(currentThumb.Size));
                        IBuffer iBuff = await currentThumb.ReadAsync(buff, buff.Capacity, InputStreamOptions.None);
                        using (var strm = await file.OpenAsync(FileAccessMode.ReadWrite))
                            await strm.WriteAsync(iBuff);
                    }
                    catch { }
                }
                ProgressTextBlock.Text = "Loading songs " + SongList.Count() + " out of " + songfilelist.Count();
            }
            ProgressTextBlock.Visibility = Visibility.Collapsed;
            return SongList;
        }

        //Serialize metadata for next app startup
        private async Task serialize(ObservableCollection<MusicItem> mymusic)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<MusicItem>));
            var file1 = await ApplicationData.Current.LocalFolder.CreateFileAsync("MusicData.dat", CreationCollisionOption.ReplaceExisting);
            Stream ms = await file1.OpenStreamForWriteAsync();
            serializer.WriteObject(ms, mymusic);
            ms.Dispose();
        }

        //Deserialize at app startup
        private async Task<ObservableCollection<MusicItem>> deserialize()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<MusicItem>));
            var f1 = await ApplicationData.Current.LocalFolder.CreateFileAsync("MusicData.dat", CreationCollisionOption.OpenIfExists);
            Stream ms = await f1.OpenStreamForReadAsync();
            var songlist = (ObservableCollection<MusicItem>)serializer.ReadObject(ms);
            ms.Dispose();
            return songlist;
        }

        private void Deletecache()
        {
            //var f1 = await ApplicationData.Current.LocalFolder.GetFileAsync("AlbumData.dat");
            //var f2 = await ApplicationData.Current.LocalFolder.GetFileAsync("MusicData.dat");
            //await f1.DeleteAsync();
            //await f2.DeleteAsync();              
        }
        #endregion

        #region Navigation + Titlebar colors

        //Back requested
        void SetBackNavigation()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                e.Handled = true;
                MainFrame.GoBack();
            }
        }

        //Applyy Titlebar colors
        private void SetTitleBar()
        {
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            Color black = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
            Color white = new Color() { A = 255, R = 255, G = 255, B = 255 };
            Color grey = new Color() { A = 255, R = 100, G = 100, B = 100 };
            titlebar.BackgroundColor = black;
            titlebar.InactiveBackgroundColor = black;
            titlebar.ButtonBackgroundColor = black;
            titlebar.ButtonInactiveBackgroundColor = black;
            titlebar.ForegroundColor = white;
            titlebar.InactiveForegroundColor = white;
            titlebar.ButtonForegroundColor = white;
            titlebar.ButtonInactiveForegroundColor = white;
            titlebar.ButtonHoverForegroundColor = grey;
            titlebar.ButtonHoverBackgroundColor = black;
        }

        #endregion

        #region Media Transport Controls

        void SetMediaTransportControls()
        {
            NowPlayingList = Player.Source as MediaPlaybackList;
            mpElement = new MediaPlayerElement();
            mpElement.SetMediaPlayer(Player);
            mpElement.AreTransportControlsEnabled = true;
            mpElement.TransportControls = MainPageTransportControls;

        }

        //Attach flyout to playlist button
        private void MainPageTransportControls_Loaded(object sender, RoutedEventArgs e)
        {
            var button = MainPageTransportControls.GetPlayListButton();
            var f = new Flyout();
            f.Content = MainPageNowPlayingListView;
            f.FlyoutPresenterStyle = new Style(typeof(FlyoutPresenter));
            f.FlyoutPresenterStyle.Setters.Add(new Setter(FlyoutPresenter.PaddingProperty, "0"));
            f.FlyoutPresenterStyle.Setters.Add(new Setter(FlyoutPresenter.BackgroundProperty, "Transparent"));
            f.FlyoutPresenterStyle.Setters.Add(new Setter(FlyoutPresenter.BorderThicknessProperty, "0"));
            button.Flyout = f;
            button.Click += Button_Click;
        }



        //Update listview source
        public void UpdateNowplayingListViewSource()
        {
            MainPageNowPlayingListView.UpdateSource(NowPlayingList.Items.ToList());
        }

        //Update Selected Index
        public void UpdateNowPlayingListViewSelectedIndex()
        {
            MainPageNowPlayingListView.UpdateSelectedIndex();
        }

        //Show flyout on buttonclick
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Flyout.ShowAt(button);
        }

        #endregion

        #region Drag and drop

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {

            //if (e.DataView.Contains(StandardDataFormats.Text))
            //{
            //    // We need to take a Deferral as we won't be able to confirm the end
            //    // of the operation synchronously
            //    var def = e.GetDeferral();
            //    var str = await e.DataView.GetTextAsync();
            //    var type = str.Substring(str.Length - 4);
            //    var song = Song.fromString(str.Substring(0,str.Length-4));
            //    if (type == "albm")
            //    {
            //        e.DragUIOverride.Caption = "Play "+song.Album;
            //        //MyMediaPlayer.playAlbum(song.Album);
            //    }
            //    else if (type == "song")
            //    {
            //        e.DragUIOverride.Caption = "Play "+ song.Title;
            //        //MyMediaPlayer.playSong(song);
            //    }                
            //    e.AcceptedOperation = DataPackageOperation.Copy;
            //    def.Complete();
            //}
        }

        private void StackPanel_DragOver(object sender, DragEventArgs e)
        {
            //    e.AcceptedOperation = (e.DataView.Contains(StandardDataFormats.Text)) ? DataPackageOperation.Copy : DataPackageOperation.None;
            //    var def = e.GetDeferral();
            //    var str = await e.DataView.GetTextAsync();
            //    var type = str.Substring(str.Length - 4);
            //    var song = Song.fromString(str.Substring(0, str.Length - 4));
            //    if (type == "albm")
            //    {
            //        e.DragUIOverride.Caption = "Play \"" + song.Album+"\"";                
            //    }
            //    else if (type == "song")
            //    {
            //        e.DragUIOverride.Caption = "Play \"" + song.Title+"\"";                
            //    }
            //    e.DragUIOverride.IsGlyphVisible = false;
            //    e.AcceptedOperation = DataPackageOperation.Copy;
            //    def.Complete();
        }

        #endregion

        #region Animations

        Compositor compositor;
        Visual visual;
        SpriteVisual blurredVisual;
        CompositionImageFactory imageFactory;
        CompositionEffectFactory effectFactory;
        ScalarKeyFrameAnimation fadeOutAnimation;
        ScalarKeyFrameAnimation fadeInAnimation;
        ScalarKeyFrameAnimation flipAnimation;

        void setupBackgroundAnimations()
        {
            visual = ElementCompositionPreview.GetElementVisual(this);
            compositor = visual.Compositor;
            blurredVisual = compositor.CreateSpriteVisual();
            imageFactory = CompositionImageFactory.CreateCompositionImageFactory(compositor);

            var graphicsEffect = new BlendEffect
            {
                Mode = BlendEffectMode.HardLight,
                Background = new ColorSourceEffect()
                {
                    Name = "Tint",
                    Color = Color.FromArgb(150, 0, 0, 0),
                },

                Foreground = new GaussianBlurEffect()
                {
                    Name = "Blur",
                    Source = new CompositionEffectSourceParameter("source"),
                    BlurAmount = 15.0f,
                    Optimization = EffectOptimization.Balanced,
                    BorderMode = EffectBorderMode.Hard,
                }
            };

            effectFactory = compositor.CreateEffectFactory(graphicsEffect, new[] { "Blur.BlurAmount", "Tint.Color" });

            fadeOutAnimation = compositor.CreateScalarKeyFrameAnimation();
            fadeOutAnimation.InsertExpressionKeyFrame(0.0f, "1");
            fadeOutAnimation.InsertExpressionKeyFrame(1f, "0");
            fadeOutAnimation.Duration = TimeSpan.FromMilliseconds(500);

            fadeInAnimation = compositor.CreateScalarKeyFrameAnimation();
            fadeInAnimation.InsertExpressionKeyFrame(0.0f, "0");
            fadeInAnimation.InsertExpressionKeyFrame(1f, "1");
            fadeInAnimation.Duration = TimeSpan.FromMilliseconds(500);

            flipAnimation = compositor.CreateScalarKeyFrameAnimation();
            flipAnimation.InsertExpressionKeyFrame(0.0f, "");
        }

        public async void animateBackGround(string img)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {

                blurredVisual.StartAnimation(nameof(Opacity), fadeOutAnimation);
                var image = imageFactory.CreateImageFromUri(new Uri(img));
                var surfaceBrush = compositor.CreateSurfaceBrush(image.Surface);

                var effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("source", surfaceBrush);

                blurredVisual.Brush = effectBrush;
                blurredVisual.Size = getBackgroundSize();
                blurredVisual.CenterPoint = new Vector3(getBackgroundSize().X / 2, getBackgroundSize().Y / 2, 0);
                ElementCompositionPreview.SetElementChildVisual(bgImage, blurredVisual);
                blurredVisual.StartAnimation(nameof(Opacity), fadeInAnimation);

            });

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (blurredVisual != null)
            {
                blurredVisual.Size = getBackgroundSize();
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            setupBackgroundAnimations();
        }

        public void animateMeidaTransportControl(string title, string cover)
        {
            MainPageTransportControls.GetAlbumTitleTextbox().Text = title;
            MainPageTransportControls.GetAlbumCoverImage().Source = new BitmapImage(new Uri(cover));
        }

        #endregion

        #region Helper methods

        Vector2 getBackgroundSize()
        {
            var x = RootGrid.ActualHeight > RootGrid.ActualWidth ? RootGrid.ActualHeight : RootGrid.ActualWidth;
            var v2 = new Vector2((float)x, (float)x);
            return v2;
        }

        #endregion
    }
}
