using UnityEngine;

public class CustomScreenshotScript : MonoBehaviour
{
    // 4k = 3840 x 2160   default:1080p = 1920 x 1080, 
    int captureWidth=1080;
    int captureHeight=1920;

    public GameObject cameraGameObject;
    public GameObject uiGameObject; 

    // private vars for screenshot
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;


    private void Start()
    {
        documentationScript = documentationCamera.GetComponent<DocumentationScript>();
        captureWidth = Screen.currentResolution.width;
        captureHeight = Screen.currentResolution.height;
    }

    

    private string timestamp="";

    public GameObject documentationCamera;
    private DocumentationScript documentationScript;

    // create a unique filename using a one-up variable
    private string uniqueFilename()
    {
        timestamp = System.DateTime.Now.ToString("dd-MM-yyy-HH-mm-ss");
        
        return "Screenshot_" + timestamp + ".png"; ;
    }

    public void CaptureScreenshot()
    {

        Debug.Log("Starting screenshot sequence...");

        uiGameObject.SetActive(false);

        // get our unique filename
        string filename = uniqueFilename();


        // create screenshot objects if needed
        if (renderTexture == null)
        {
            // creates off-screen render texture that can rendered into
            rect = new Rect(0, 0, captureWidth, captureHeight);
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
            documentationCamera.SetActive(true);
            screenShot = documentationScript.GetDocumentationPic(captureWidth, captureHeight, timestamp);
            documentationCamera.SetActive(false);
        }

        // get main camera and manually render scene into rt
        Camera camera = this.GetComponent<Camera>(); // NOTE: added because there was no reference to camera in original script; must add this script to Camera
        camera.targetTexture = renderTexture;
        camera.Render();

        // read pixels will read from the currently active render texture so make our offscreen 
        // render texture active and then read the pixels
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);

        // reset active camera texture and render texture
        camera.targetTexture = null;
        RenderTexture.active = null;

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(screenShot, "VRnocollision", filename,(error)=>
        {
            if (error != null) Debug.Log("An error occured while saving the image: " + error);

            Destroy(renderTexture);
            renderTexture = null;
            screenShot = null;
            uiGameObject.SetActive(true);

        });

        Debug.Log("permission status: " + permission);
        
        Debug.Log("Ending screenshot function...");
    }
    
}
