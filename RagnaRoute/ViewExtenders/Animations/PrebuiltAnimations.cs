using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using System;

namespace RagnaRoute.Animations;
public static class PrebuiltAnimations
{
    public static Animation PopupFade = new Animation()
    {
        Duration = TimeSpan.FromMilliseconds(300),
        FillMode = FillMode.Forward,
        Easing = new CubicEaseOut(),
        IterationCount = new IterationCount(1),
        Delay = TimeSpan.FromMilliseconds(800),
        Children =
        {
            new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, 1d) },
                Cue = new(0)
            },
            new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, 0d) },
                Cue = new(1)
            }
        }
    };

    public static Animation CopyIconColorShift = new Animation()
    {
        Duration = TimeSpan.FromMilliseconds(500),
        FillMode = FillMode.Backward,
        Easing = new ExponentialEaseOut(),
        IterationCount = new IterationCount(2),
        PlaybackDirection = PlaybackDirection.Alternate,
        Children =
        {
            new KeyFrame
            {
                Setters = { new Setter(FAIconElement.ForegroundProperty, new ImmutableSolidColorBrush(Color.Parse("#FFFFFF"))) },
                Cue = new(0)
            },
            new KeyFrame
            {
                Setters = { new Setter(FAIconElement.ForegroundProperty, new ImmutableSolidColorBrush(Color.Parse("#238636"))) },
                Cue = new(1)
            }
        }
    };
}
