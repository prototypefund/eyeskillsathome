using System;

public interface IScreenOrientationManager
{
    void CallbackWhenInLandscapeLeft(Action callback);
    void CallbackWhenInPortrait(Action callback);
}