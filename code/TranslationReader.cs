using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TranslationReader : MonoBehaviour
{
	public TranslationReader()
	{
	}

	private void Awake()
	{
		TranslationReader.singleton = this;
	}

	private void Start()
	{
		this.Refresh();
	}

	private void OnLevelWasLoaded(int i)
	{
		if (i == 0)
		{
			this.Refresh();
		}
	}

	public static string Get(string n, int v)
	{
		string result;
		try
		{
			foreach (TranslationReader.TranslatedElement translatedElement in TranslationReader.singleton.elements)
			{
				if (translatedElement.fileName == n)
				{
					return translatedElement.values[v].Replace("\\n", Environment.NewLine);
				}
			}
			result = "NO_TRANSLATION";
		}
		catch
		{
			result = "TRANSLATION_ERROR";
		}
		return result;
	}

	private void Refresh()
	{
		TranslationReader.path = "Translations/" + PlayerPrefs.GetString("translation_path", "Translations/English_default");
		if (!Directory.Exists(TranslationReader.path))
		{
			TranslationReader.path = "Translations/English (default)";
		}
		TranslationReader.singleton.elements.Clear();
		string[] files = Directory.GetFiles(TranslationReader.path);
		foreach (string text in files)
		{
			string text2 = text.Replace("\\", "/");
			try
			{
				StreamReader streamReader = new StreamReader(text2);
				string text3 = streamReader.ReadToEnd();
				streamReader.Close();
				string text4 = text2.Remove(0, text2.LastIndexOf("/") + 1);
				TranslationReader.TranslatedElement item = new TranslationReader.TranslatedElement
				{
					fileName = text4.Remove(text4.IndexOf('.')),
					values = text3.Split(new string[]
					{
						Environment.NewLine
					}, StringSplitOptions.None)
				};
				TranslationReader.singleton.elements.Add(item);
			}
			catch
			{
			}
		}
		SceneManager.LoadScene("MainMenuRemastered");
	}

	public static TranslationReader singleton;

	public List<TranslationReader.TranslatedElement> elements = new List<TranslationReader.TranslatedElement>();

	public static string path;

	[Serializable]
	public class TranslatedElement
	{
		public TranslatedElement()
		{
		}

		public string fileName;

		public string[] values;
	}
}
