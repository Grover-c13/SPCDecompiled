using System;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin
{
	public class Login : RemoteAdminBehaviour
	{
		public void ClearWhite()
		{
			this.passwordField.GetComponent<RawImage>().color = Color.white;
		}

		public void Password()
		{
			PasswordHolder.password = this.passwordField.text;
			this.passwordField.GetComponent<RawImage>().color = Color.white;
			this.passwordField.interactable = false;
			PlayerManager.localPlayer.GetComponent<QueryProcessor>().CallCmdCheckPassword(this.passwordField.text);
		}

		public override void Reply(string[] reply)
		{
			if (reply[0].Contains("_PasswordCheck_"))
			{
				if (reply[0].Contains("True"))
				{
					this.loggedIn = true;
					this.obj_log.SetActive(false);
					this.obj_panel.SetActive(true);
				}
				else
				{
					this.passwordField.GetComponent<RawImage>().color = Color.red;
					this.passwordField.interactable = true;
				}
			}
		}

		public InputField passwordField;

		public GameObject obj_panel;

		public GameObject obj_log;

		public bool loggedIn;
	}
}
