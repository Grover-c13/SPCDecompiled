using System;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
	public class Chooser : MonoBehaviour
	{
		private void Start()
		{
			foreach (Chooser.Option option in this.options)
			{
				option.button.interactable = true;
			}
			this.SelectMenu(this.options[0].button);
		}

		public void SelectMenu(Button b)
		{
			foreach (Chooser.Option option in this.options)
			{
				bool flag = b == option.button;
				option.content.SetActive(flag);
				option.button.GetComponent<Text>().color = ((!flag) ? Color.white : this.selectedColor);
			}
			base.GetComponent<Ban>().RefreshGUI();
		}

		public Chooser.Option[] options;

		public Color selectedColor;

		[Serializable]
		public class Option
		{
			public string name;

			public Button button;

			public GameObject content;
		}
	}
}
