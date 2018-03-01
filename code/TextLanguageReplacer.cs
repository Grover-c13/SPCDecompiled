using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLanguageReplacer : MonoBehaviour
{
	public TextLanguageReplacer()
	{
	}

	public void UpdateString()
	{
		string text = TranslationReader.Get(this.keyName, this.index);
		while (text.Contains("\\n"))
		{
			text = text.Replace("\\n", Environment.NewLine);
		}
		if (base.GetComponent<TextMeshProUGUI>() != null)
		{
			base.GetComponent<TextMeshProUGUI>().text = text;
		}
		else
		{
			base.GetComponent<Text>().text = text;
		}
	}

	private void Awake()
	{
		this.UpdateString();
	}

	private MenuMusicManager mng;

	[Multiline]
	public string englishVersion;

	public string keyName;

	public int index;
}
