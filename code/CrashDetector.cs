using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrashDetector : MonoBehaviour
{
	public CrashDetector()
	{
	}

	private void Awake()
	{
		CrashDetector.singleton = this;
	}

	public static bool Show()
	{
		if (SystemInfo.graphicsDeviceName.ToUpper().Contains("INTEL") && PlayerPrefs.GetInt("intel_warning") != 1)
		{
			PlayerPrefs.SetInt("intel_warning", 1);
			CrashDetector.singleton.StartCoroutine(CrashDetector.singleton.IShow());
			return true;
		}
		return false;
	}

	public IEnumerator IShow()
	{
		this.root.SetActive(true);
		Button button = this.root.GetComponentInChildren<Button>();
		Text text = button.GetComponent<Text>();
		button.interactable = false;
		for (int i = 10; i >= 1; i--)
		{
			text.text = "OKAY (" + i + ")";
			yield return new WaitForSeconds(1f);
		}
		text.text = "OKAY";
		button.interactable = true;
		yield break;
	}

	public GameObject root;

	public static CrashDetector singleton;
}
