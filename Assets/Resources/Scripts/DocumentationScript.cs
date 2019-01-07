using UnityEngine;
using UnityEngine.UI;

public class DocumentationScript : MonoBehaviour
{

    //Text field to put additional info about screenshot

    //TF: Text Field
    public Text timestampTF;
    public Text railTF;
    public Text trainTypeTF;
    public Text turnRadiusTF;
    public Text turnTiltTF;
    public Text signHeightTF;


    private void fillInfoField(string timestamp)
    {
        MainLogic mainLogic = MainLogic.GetInst();

        timestampTF.text = timestamp;
		railTF.text = (mainLogic.mainrail) ? "Main" : "Side";
        switch (mainLogic.elecspeedhigh)
        {
            case 0:
                trainTypeTF.text = "Non Electric";
                break;
            case 1:
                trainTypeTF.text = "Electric, speed < 160 Km/h";
                break;
            case 2:
                trainTypeTF.text = "Electric, speed > 160 Km/h";
                break;
            default:
                break;
        }

        turnRadiusTF.text = mainLogic.turnrad.text;
        turnTiltTF.text = mainLogic.turntilt.text;
        signHeightTF.text = mainLogic.objectheight.text;

    }

    public Texture2D GetDocumentationPic(int captureWidth, int captureHeight, string timestamp)
    {
        Rect rect;
        RenderTexture renderTexture;
        Texture2D screenShot;
        // fill info field
        fillInfoField(timestamp);

        // creates off-screen render texture that can rendered into
        rect = new Rect(0, 0, captureWidth, captureHeight);
        renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        screenShot = new Texture2D(captureWidth * 2, captureHeight, TextureFormat.RGB24, false);


        // get main camera and manually render scene into rt
        Camera camera = this.GetComponent<Camera>(); // NOTE: added because there was no reference to camera in original script; must add this script to Camera
        camera.targetTexture = renderTexture;
        camera.Render();

        // read pixels will read from the currently active render texture so make our offscreen 
        // render texture active and then read the pixels
        RenderTexture.active = renderTexture;

        //Start writing of the pixels from half way of the screen for the desired documentation effect
        screenShot.ReadPixels(rect, captureWidth, 0);

        // reset active camera texture and render texture
        camera.targetTexture = null;
        RenderTexture.active = null;

        // cleanup if needed            
        Destroy(renderTexture);
        renderTexture = null;

        return screenShot;

    }


}
