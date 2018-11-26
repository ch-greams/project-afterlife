using System;
using Sirenix.OdinInspector;
using UnityEngine;


public class CameraUtils : MonoBehaviour
{
    [BoxGroup("Screenshot")]
    [FolderPath(AbsolutePath = true)]
    public string folder = "";
    [BoxGroup("Screenshot")]
    public string fileName = "";
    [BoxGroup("Screenshot")]
    public int superSize = 4;


    [BoxGroup("Screenshot")]
    [Button(ButtonSizes.Medium)]
    public void TakeScreenshot()
    {
        string fullpath = string.Format("{0}/{1}_{2:yyyy-MM-dd_hh-mm-ss}.png", this.folder, this.fileName, DateTime.Now);

        Debug.Log(string.Format("Saving screenshot to {0}", fullpath));

        ScreenCapture.CaptureScreenshot(fullpath, this.superSize);
    }
}
