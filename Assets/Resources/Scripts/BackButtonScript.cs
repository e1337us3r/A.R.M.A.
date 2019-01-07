using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonScript : ControlButton {

    public override void DoStuff()
    {
        BackToMainScene();
    }

    public void BackToMainScene()
    {
        SceneManager.LoadScene("VR-ATU-MainScene");
    }
}
