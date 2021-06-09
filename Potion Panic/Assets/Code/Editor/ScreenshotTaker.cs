using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScreenshotTaker
{
	[MenuItem("Screen/Capture")]
	public static void CaptureScreenshot()
	{
		string date = System.DateTime.Now.ToString();
		date = date.Replace("/","-");
		date = date.Replace(" ","_");
		date = date.Replace(":","-");
		ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshots/Potion_Panic_"+date+".png");
	}
}
