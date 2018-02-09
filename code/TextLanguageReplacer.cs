using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLanguageReplacer : MonoBehaviour
{
	private void Awake()
	{
		if (base.GetComponent<TextMeshProUGUI>() != null)
		{
			base.GetComponent<TextMeshProUGUI>().text = ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? this.englishVersion : this.polishVersion);
		}
		else
		{
			base.GetComponent<Text>().text = ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? this.englishVersion : this.polishVersion);
		}
	}

	private MenuMusicManager mng;

	[SerializeField]
	[Multiline]
	public string polishVersion;

	[SerializeField]
	[Multiline]
	public string englishVersion;
}
