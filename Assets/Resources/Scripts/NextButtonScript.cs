using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButtonScript : ControlButton {

    public override void DoStuff()
    {
        Next();
    }

    public void Next () {
        GameObject.Find("ScreenShotViewer").GetComponent<ScreenshotPreview>().NextPicture();

    }
	
}
