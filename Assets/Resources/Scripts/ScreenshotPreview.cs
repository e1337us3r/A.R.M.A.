using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class ScreenshotPreview : MonoBehaviour
{

    [SerializeField]
    GameObject panel;
    [SerializeField]
    Sprite defaultImage;
    string[] files = null;
    int whichScreenShotIsShown = 0;

    AndroidJavaObject currentActivity;
    AndroidJavaObject unityContext;
    AndroidJavaClass fileProvider;
    int FLAG_GRANT_READ_URI_PERMISSION;
    string authority;

    // Use this for initialization
    void Start()
    {
       
        panel.GetComponent<Image>().sprite = defaultImage;
        files = Directory.GetFiles(Application.persistentDataPath + "/", "*.png");

        // Images are in reverse chronogical order so we reverse it before showing them
        Array.Reverse(files);

        if (files.Length > 0)
        {
            GetPictureAndShowIt();
        }

        // Load these resources only once to improve performance
        if (Application.platform == RuntimePlatform.Android) {

            //Get Activity then Context
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            unityContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            // This flag is required for sharing things to other apps
            FLAG_GRANT_READ_URI_PERMISSION = new AndroidJavaClass("android.content.Intent").GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION");

            string packageName = unityContext.Call<string>("getPackageName");
            authority = packageName + ".fileprovider";


            fileProvider = new AndroidJavaClass("android.support.v4.content.FileProvider");
        }



    }

    void GetPictureAndShowIt()
    {
        string pathToFile = files[whichScreenShotIsShown];
        Texture2D texture = GetScreenshotImage(pathToFile);
        Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        panel.GetComponent<Image>().sprite = sp;
    }

    public static Texture2D GetScreenshotImage(string filePath)
    {
        Texture2D texture = null;
        byte[] fileBytes;
        if (File.Exists(filePath))
        {
            Debug.Log(filePath);
            fileBytes = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(fileBytes);
        }
        return texture;
    }


    // This feature is not used atm
    public void DeleteImage()
    {
        if (files.Length > 0)
        {
            string pathToFile = files[whichScreenShotIsShown];
            if (File.Exists(pathToFile))
                File.Delete(pathToFile);
            files = Directory.GetFiles(Application.persistentDataPath + "/", "*.png");
            if (files.Length > 0)
                NextPicture();
            else
                panel.GetComponent<Image>().sprite = defaultImage;
        }
    }

    // This method is android spesific atm
    public void ShareIt()
    {

        if (Application.platform == RuntimePlatform.Android)
        {
            // This method uses new standard FileProvider
            // it is required when targeting android api >23
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentObject.GetStatic<string>("ACTION_SEND"));

            AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", files[whichScreenShotIsShown]);
            AndroidJavaObject uriObject = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", unityContext, authority, fileObj);
         
            intentObject.Call<AndroidJavaObject>("putExtra", intentObject.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            intentObject.Call<AndroidJavaObject>("addFlags", FLAG_GRANT_READ_URI_PERMISSION);
            currentActivity.Call("startActivity", intentObject);
        }
    }

    public void NextPicture()
    {
        if (files.Length > 0)
        {
            whichScreenShotIsShown += 1;
            if (whichScreenShotIsShown > files.Length - 1)
                whichScreenShotIsShown = 0;
            GetPictureAndShowIt();
        }
    }

    public void PreviousPicture()
    {
        if (files.Length > 0)
        {
            whichScreenShotIsShown -= 1;
            if (whichScreenShotIsShown < 0)
                whichScreenShotIsShown = files.Length - 1;
            GetPictureAndShowIt();
        }
    }
}
