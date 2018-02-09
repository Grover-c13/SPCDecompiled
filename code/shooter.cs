using System;
using UnityEngine;

public class shooter : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			ScreenCapture.CaptureScreenshot("Taken" + UnityEngine.Random.Range(0, 1000) + ".png", this.mtpl);
		}
	}

	public int mtpl = 5;
}
