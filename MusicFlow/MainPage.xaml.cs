using MusicFlow.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MusicFlow
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Song> MusicList;

        public MainPage()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }
        

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            setupTitleBar();
            await GetMusic();
            MainFrame.Navigate(typeof(Views.AlbumView), MusicList);
            var mymp = new MyMediaPlayer();
        }        

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
        private async Task<ObservableCollection<Song>> createSongList(ObservableCollection<StorageFile> songfilelist)
        {
            StorageFolder localFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("CoverArt", CreationCollisionOption.OpenIfExists);

            var songList = new ObservableCollection<Song>();           
            
            foreach (var p in songfilelist)
            {
                var prop = await p.Properties.GetMusicPropertiesAsync();
                StorageItemThumbnail currentThumb = await p.GetThumbnailAsync(ThumbnailMode.MusicView,200,ThumbnailOptions.UseCurrentScale);
                var song = new Song();
                song.Title = prop.Title;
                song.Artist = prop.Artist;
                song.Album = prop.Album;
                song.AlbumCover = string.Format("ms-appdata:///local/CoverArt/Cover_{0}_{1}.jpg", song.Album, song.Artist);
                song.SongFile = p.Path;
                song.AlbumArtist = prop.AlbumArtist;
                try { song.Genre = prop.Genre[0].ToString(); }
                catch { }
                song.TrackNo = prop.TrackNumber.ToString();
                song.Year = prop.Year.ToString();
                song.Length = prop.Duration.Minutes.ToString() + ":" + prop.Duration.Seconds.ToString();
                songList.Add(song);
                var fname = string.Format("Cover_{0}_{1}.jpg", song.Album, song.Artist);
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
                ProgressTextBlock.Text = "Loading songs " + songList.Count() + " out of " + songfilelist.Count();
            }
            ProgressTextBlock.Visibility = Visibility.Collapsed;            

            return songList;
        }

        //Serialize metadata for next app startup
        private async Task serialize(ObservableCollection<Song> mymusic)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Song>));
            var file1 = await ApplicationData.Current.LocalFolder.CreateFileAsync("MusicData.dat", CreationCollisionOption.ReplaceExisting);
            Stream ms = await file1.OpenStreamForWriteAsync();
            serializer.WriteObject(ms, mymusic);
            ms.Dispose();
        }

        //Deserialize at app startup
        private async Task<ObservableCollection<Song>> deserialize()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Song>));
            var f1 = await ApplicationData.Current.LocalFolder.CreateFileAsync("MusicData.dat", CreationCollisionOption.OpenIfExists);           
            Stream ms = await f1.OpenStreamForReadAsync();
            var songlist = (ObservableCollection<Song>)serializer.ReadObject(ms);           
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

        //Create Media Transport Controls
        public void setupMediaPlayer(string c,string t,string a)
        {
            npCoverImage.Source = new BitmapImage(new Uri(c));
            npTitleTextBlock.Text = t;
            npAlbumTextBlock.Text = a;
        }

        //Back requested
        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                e.Handled = true;
                MainFrame.GoBack();
            }
        }

        //Applyy Titlebar colors
        private void setupTitleBar()
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

        private void BackDrop_Loaded(object sender, RoutedEventArgs e)
        {
            animateBackGround();
        }

        public void animateBackGround()
        {
            Storyboard1.Begin();
            nowPlayingImageAnimation.Begin();
            nowPlayingTextAnimation.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
        }

       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.playNextSong();
        }

        public void disableNextButton()
        {
            NextButton.IsEnabled = false;
        }

        public void enableNextButton()
        {
            NextButton.IsEnabled = true;
        }

        public void updateNPList()
        {
            PlayListView.ItemsSource = null;
            PlayListView.ItemsSource = MyMediaPlayer.nowPlayingList;            
        }

        private void PlayListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ci = e.ClickedItem as Song;
            MyMediaPlayer.currentSong = ci;
            MyMediaPlayer.playSong(ci);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.playPreviousSong();
        }
    }
}
