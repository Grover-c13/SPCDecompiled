using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TranslationBrowserButton : MonoBehaviour
{
	public TranslationBrowserButton()
	{
	}

	public void OnClick()
	{
		PlayerPrefs.SetString("translation_path", base.GetComponent<Text>().text);
		SceneManager.LoadScene(0);
	}
}
