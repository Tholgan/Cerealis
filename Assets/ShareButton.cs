using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShareButton : MonoBehaviour
{

	public Button shareButton;
	private bool isFocus = false;
	private bool isProcessing = false;

	void Start()
	{
		shareButton.onClick.AddListener(takeScreenShotAndShare);
	}

    void takeScreenShotAndShare()
    {
        StartCoroutine(takeScreenshotAndSave());
    }

    private IEnumerator takeScreenshotAndSave()
    {
        string path = "";
        yield return new WaitForEndOfFrame();

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);

        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        //Convert to png
        byte[] imageBytes = screenImage.EncodeToPNG();


        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/GameOverScreenShot");
        path = Application.persistentDataPath + "/GameOverScreenShot" + "/DiedScreenShot.png";
        System.IO.File.WriteAllBytes(path, imageBytes);

        StartCoroutine(shareScreenshot(path));
    }

    private IEnumerator shareScreenshot(string destination)
    {
        string ShareSubject = "Partager votre dessin";
        string textToShare = "Regarde comme il est beau mon dessin";

        Debug.Log(destination);


        if (!Application.isEditor)
        {

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);

            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), textToShare);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), ShareSubject);
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);
        }
        yield return null;
    }

    //	void OnApplicationFocus(bool focus)
    //	{
    //		isFocus = focus;
    //	}

    //	private void ShareText()
    //	{
    //#if UNITY_ANDROID
    //		if (!isProcessing)
    //		{
    //			StartCoroutine(ShareTextInAnroid());
    //		}
    //#else
    //		Debug.Log("No sharing set up for this platform.");
    //#endif
    //	}

    //#if UNITY_ANDROID
    //	public IEnumerator ShareTextInAnroid()
    //	{
    //		var shareSubject = "Dessin Cerealis";
    //		var shareMessage = "Regarde comme il est beau mon dessin";
    //		isProcessing = true;
    //		if (!Application.isEditor)
    //		{
    //			//Create intent for action send
    //			AndroidJavaClass intentClass =
    //				new AndroidJavaClass("android.content.Intent");
    //			AndroidJavaObject intentObject =
    //				new AndroidJavaObject("android.content.Intent");
    //			intentObject.Call<AndroidJavaObject>
    //				("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

    //			//put text and subject extra
    //			intentObject.Call<AndroidJavaObject>("setType", "text/plain");
    //			intentObject.Call<AndroidJavaObject>
    //				("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
    //			intentObject.Call<AndroidJavaObject>
    //				("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

    //			//call createChooser method of activity class
    //			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //			AndroidJavaObject currentActivity =
    //				unity.GetStatic<AndroidJavaObject>("currentActivity");
    //			AndroidJavaObject chooser =
    //				intentClass.CallStatic<AndroidJavaObject>
    //				("createChooser", intentObject, "Share your high score");
    //			currentActivity.Call("startActivity", chooser);
    //		}

    //		yield return new WaitUntil(() => isFocus);
    //		isProcessing = false;
    //	}
    //#endif
}
