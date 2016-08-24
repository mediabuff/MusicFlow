using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace MusicFlow.Controls
{
    public class BackDrop : Control
    {
        Compositor compositor;
        Visual visual;
        SpriteVisual blurredVisual;
        CompositionEffectFactory effectFactory;

        public BackDrop()
        {
            visual = ElementCompositionPreview.GetElementVisual(this);
            compositor = visual.Compositor;
            blurredVisual = compositor.CreateSpriteVisual();

            var graphicsEffect = new BlendEffect
            {
                Mode = BlendEffectMode.HardLight,
                Background = new ColorSourceEffect()
                {
                    Name = "Tint",
                    Color = Color.FromArgb(175, 0, 0, 0),
                },

                Foreground = new GaussianBlurEffect()
                {
                    Name = "Blur",
                    Source = new CompositionEffectSourceParameter("source"),
                    BlurAmount = 10f,
                    Optimization = EffectOptimization.Balanced,
                    BorderMode = EffectBorderMode.Hard,
                }
            };           

            effectFactory = compositor.CreateEffectFactory(graphicsEffect);
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("source", compositor.CreateBackdropBrush());

            blurredVisual.Brush = effectBrush;
            ElementCompositionPreview.SetElementChildVisual(this, blurredVisual);

            this.SizeChanged += BackDrop_SizeChanged;
        }

        private void BackDrop_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                blurredVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            }
            catch { }
        }
    }
}
