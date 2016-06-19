using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Data;

namespace MusicFlow.Model
{
    class MusicItemDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var duration = (TimeSpan)value;
            string newDuration = duration.Minutes.ToString("00") + ":" + duration.Seconds.ToString("00");
            return newDuration;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class NowplayingTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var source = value as MediaSource;
            var title = source.CustomProperties["Title"] as string;
            return title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class NowplayingArtistConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var source = value as MediaSource;
            var title = source.CustomProperties["Artist"] as string;
            return title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
