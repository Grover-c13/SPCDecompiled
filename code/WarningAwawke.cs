using System;
using UnityEngine;
using UnityEngine.UI;

public class WarningAwawke : MonoBehaviour
{
	private void Awake()
	{
		if (PlayerPrefs.GetString("warningToggle", "false") != "false")
		{
			base.gameObject.SetActive(false);
		}
	}

	public void Close()
	{
		if (this.toggle.isOn)
		{
			PlayerPrefs.SetString("warningToggle", "true");
		}
		base.gameObject.SetActive(false);
	}

	public Toggle toggle;
}
