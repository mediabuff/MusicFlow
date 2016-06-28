using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MusicFlow.Model
{
    public class MusicFlowMediaTransportControls : MediaTransportControls
    {
        public MusicFlowMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(MusicFlowMediaTransportControls);
        }

        protected override void OnApplyTemplate()
        {
            var nowplayingButton = GetPlayListButton();
            nowplayingButton.Click += NowplayingButton_Click;
            base.OnApplyTemplate();
        }

        private void NowplayingButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
           
        }

        public TextBlock GetAlbumTitleTextbox()
        {
            var SongTitleTextBlock = GetTemplateChild("TitleTextBlock") as TextBlock;
            return SongTitleTextBlock;
        }
        public Image GetAlbumCoverImage()
        {
            var image = GetTemplateChild("CoverImage") as Image;
            return image;
        }
        public AppBarButton GetPlayListButton()
        {
            var button = GetTemplateChild("NowPlayingListButton") as AppBarButton;
            return button;
        }
    }
}
