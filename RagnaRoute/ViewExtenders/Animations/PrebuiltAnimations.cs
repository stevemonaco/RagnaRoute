using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;
using System;

namespace RagnaRoute.Animations;
public static class PrebuiltAnimations
{
    public static Animation PopupFade = new Animation()
    {
        Duration = TimeSpan.FromMilliseconds(1200),
        FillMode = FillMode.Forward,
        Easing = new CubicEaseOut(),
        IterationCount = new IterationCount(1),
        Children =
        {
            new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, 1d) },
                KeyTime = TimeSpan.FromMilliseconds(0)
            },
            new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, 1d) },
                KeyTime = TimeSpan.FromMilliseconds(800)
            },
            new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, 0d) },
                KeyTime = TimeSpan.FromMilliseconds(1200)
            }
        }
    };
}
