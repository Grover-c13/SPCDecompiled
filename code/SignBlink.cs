using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SignBlink : MonoBehaviour
{
	public void Play(int duration)
	{
		if (this.startText == string.Empty)
		{
			this.startText = base.GetComponent<TextMeshProUGUI>().text;
		}
		else
		{
			base.GetComponent<TextMeshProUGUI>().text = this.startText;
		}
		base.StartCoroutine(this.Blink((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? base.GetComponent<TextLanguageReplacer>().englishVersion : base.GetComponent<TextLanguageReplacer>().polishVersion, duration));
	}

	private IEnumerator Blink(string text, int iterations)
	{
		for (int i = 0; i < iterations; i++)
		{
			string s = string.Empty;
			for (int j = 0; j < text.Length; j++)
			{
				s += "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>"[UnityEngine.Random.Range(0, "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>".Length)];
				if (j != text.Length - 1 && this.verticalText)
				{
					s += "\n";
				}
			}
			base.GetComponent<TextMeshProUGUI>().text = s;
			yield return new WaitForSeconds(0.02f);
		}
		base.GetComponent<TextMeshProUGUI>().text = text;
		yield break;
	}

	public bool verticalText;

	private string startText;

	private const string alphabet = "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>";
}
