using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUnlocker : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < Mathf.Clamp(PlayerPrefs.GetInt("TutorialProgress", 1), 1, this.buttons.Length); i++)
		{
			this.buttons[i].interactable = true;
		}
	}

	public Button[] buttons;
}
