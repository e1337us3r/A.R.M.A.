using System.Collections;
using UnityEngine;

public class CameraButton : ControlButton
{
    public GameObject screenshotCamera;


    override public void DoStuff()
    {
        //TakeAShot();
        screenshotCamera.SetActive(true);
        screenshotCamera.GetComponent<CustomScreenshotScript>().CaptureScreenshot();
    }


    
}
