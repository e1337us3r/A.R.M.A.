using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GalleryButtonScript : ControlButton {


    public override void DoStuff()
    {
        SceneManager.LoadScene("GalleryScene");
    }


}
