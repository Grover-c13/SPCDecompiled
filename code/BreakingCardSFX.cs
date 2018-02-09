using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakingCardSFX : MonoBehaviour
{
	private void OnEnable()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.DoAnimation());
	}

	private IEnumerator DoAnimation()
	{
		this.text = base.GetComponent<TextMeshProUGUI>();
		this.txt = base.GetComponent<Text>();
		for (;;)
		{
			foreach (string item in this.texts)
			{
				try
				{
					if (this.text != null)
					{
						this.text.text = item;
					}
					if (this.txt != null)
					{
						this.txt.text = item;
					}
				}
				catch
				{
					Debug.LogError("Iteration error in BC-SFX script");
				}
				yield return new WaitForSeconds(this.waitTime);
			}
		}
		yield break;
	}

	public string[] texts;

	public float waitTime = 1.3f;

	private TextMeshProUGUI text;

	private Text txt;
}
