using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Microsoft.UI.Composition.Toolkit;

namespace MusicFlow.Model
{
    public class BlurControl : Control
    {
        Visual _visualroot;
        Compositor _compositor;
        SpriteVisual _blurredVisual;

        public BlurControl()
        {
            _visualroot = ElementCompositionPreview.GetElementVisual(this as UIElement);
            _compositor = _visualroot.Compositor;
            var blurEffect = new GaussianBlurEffect()
            {
                Name = "Blur",
                BlurAmount = 10,
                Optimization = EffectOptimization.Speed,
                BorderMode = EffectBorderMode.Hard,
                Source = new CompositionEffectSourceParameter("destinationSource")
            };

            var imageFactory = CompositionImageFactory.CreateCompositionImageFactory(_compositor);
            var image = imageFactory.CreateImageFromUri(new Uri("ms-appx:///Assets/bg.jpg"));

            var effectFactory = _compositor.CreateEffectFactory(blurEffect, new[] { "Blur.BlurAmount" });
            var effectbrush = effectFactory.CreateBrush();
            effectbrush.SetSourceParameter("destinationSource",_compositor.CreateSurfaceBrush(image.Surface));
            _blurredVisual = _compositor.CreateSpriteVisual();
            _blurredVisual.Brush = effectbrush;
            ElementCompositionPreview.SetElementChildVisual(this as UIElement, _blurredVisual);
            
            this.Loading += OnLoading;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoading(FrameworkElement sender, object args)
        {
            this.SizeChanged += OnSizeChanged;
            OnSizeChanged(this, null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= OnSizeChanged;
        }


        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (_blurredVisual != null)
            {
                _blurredVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            }
        }
    }
}
