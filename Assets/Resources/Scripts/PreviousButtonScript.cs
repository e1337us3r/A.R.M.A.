using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousButtonScript : ControlButton {

    public override void DoStuff()
    {
        Previous();
    }

    public void Previous()
    {
        GameObject.Find("ScreenShotViewer").GetComponent<ScreenshotPreview>().PreviousPicture();
    }
}
