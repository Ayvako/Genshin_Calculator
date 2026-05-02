using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Genshin_Calculator.Presentation.Features.Splash;

public partial class SplashWindow : Window
{
    public SplashWindow()
    {
        this.InitializeComponent();
    }

    public void SetStatus(string message, double progress)
    {
        this.Dispatcher.Invoke(() =>
        {
            this.StatusText.Text = message;
            DoubleAnimation animation = new()
            {
                To = progress,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
            };

            this.ProgressBar.BeginAnimation(System.Windows.Controls.Primitives.RangeBase.ValueProperty, animation);
        });
    }
}