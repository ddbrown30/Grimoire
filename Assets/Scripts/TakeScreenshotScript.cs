using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TakeScreenshotScript : MonoBehaviour
{
    public void TakeScreenshot()
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
        path += "\\Grimoire";
        Directory.CreateDirectory(path);
        ScreenCapture.CaptureScreenshot(path + "\\SpyGrimoire.png");
        Application.OpenURL("file://" + path);
    }
}
