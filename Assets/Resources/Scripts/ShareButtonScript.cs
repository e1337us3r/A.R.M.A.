using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareButtonScript : ControlButton {

    public override void DoStuff()
    {
        Share();
    }

    public void Share()
    {
        GameObject.Find("ScreenShotViewer").GetComponent<ScreenshotPreview>().ShareIt();
    }
}
