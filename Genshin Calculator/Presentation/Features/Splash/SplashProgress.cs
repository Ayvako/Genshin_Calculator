using System;

namespace Genshin_Calculator.Presentation.Features.Splash;

public class SplashProgress : IProgress<(string Message, double Percent)>
{
    private readonly SplashWindow splash;

    public SplashProgress(SplashWindow splash) => this.splash = splash;

    public void Report((string Message, double Percent) value)
    {
        this.splash.SetStatus(value.Message, value.Percent);
    }
}