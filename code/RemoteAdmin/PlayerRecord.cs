using System;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
	public class PlayerRecord : MonoBehaviour
	{
		public void Setup(int _id, string _nick)
		{
			this.monitor = base.GetComponentInParent<PlayersMonitor>();
			this.text = base.GetComponent<Text>();
			this.text.text = _nick;
			this.id = _id;
		}

		public void Select(bool b)
		{
			this.text.color = ((!b) ? Color.white : this.selectedColor);
		}

		public void Click()
		{
			this.monitor.SelectPlayer(this.id);
		}

		private PlayersMonitor monitor;

		private int id;

		private Text text;

		public Color selectedColor;
	}
}
