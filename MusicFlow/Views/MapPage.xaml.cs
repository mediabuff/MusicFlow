using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicFlow.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
       

        public MapPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                var geoLocator = new Geolocator();
                var pos = await geoLocator.GetGeopositionAsync();
                geoLocator.PositionChanged += positionCHanged;
                MyMap.Center = pos.Coordinate.Point;
                MyMap.ZoomLevel = 15;

                // map icon
                var currentLocationIcon = new MapIcon();
                currentLocationIcon.Location = pos.Coordinate.Point;
                MyMap.MapElements.Add(currentLocationIcon);
            }

           
        }

        private async void positionCHanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var y = (MyMap.MapElements.FirstOrDefault() as MapIcon);
                y.Location = args.Position.Coordinate.Point;
                
            });
           
        }
    }
}
