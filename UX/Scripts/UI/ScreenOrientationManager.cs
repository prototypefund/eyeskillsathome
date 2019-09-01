using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationManager : MonoBehaviour, IScreenOrientationManager
{
    public bool fakeHorizontalOrientation;

    public class OrientationParts{
        public ScreenOrientation orientation;
        public Action callback;
    }
 
    IEnumerator DetectOrientationChange(OrientationParts orientationParts)
    {

        //TODO : Warn the user if they put it in ScreenOrientation.LandscapeLeft. We don't want that.
        while (!((Screen.orientation == orientationParts.orientation) || (fakeHorizontalOrientation==true)))
        {
            //Debug.Log("Screen orientation is " + Screen.orientation.ToString());
            yield return null;
        }
        Debug.Log("Calling back from screen orientation");
        orientationParts.callback();
    }

    public void CallbackWhenInLandscapeLeft(Action callback){
        OrientationParts op = new OrientationParts();
        op.callback = callback;
        op.orientation = ScreenOrientation.LandscapeLeft;
        StartCoroutine("DetectOrientationChange",op);
    }

    public void CallbackWhenInPortrait(Action callback)
    {
        OrientationParts op = new OrientationParts();
        op.callback = callback;
        op.orientation = ScreenOrientation.Portrait;
        StartCoroutine("DetectOrientationChange", op);
    }
}
