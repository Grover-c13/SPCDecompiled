using System;
using UnityEngine;

namespace RemoteAdmin
{
	public class PasswordHolder : MonoBehaviour
	{
		private void OnLevelWasLoaded(int level)
		{
			PasswordHolder.password = string.Empty;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.M) && !base.GetComponent<Login>().passwordField.isFocused)
			{
				this.ChangeVisibility();
			}
		}

		public void ChangeVisibility()
		{
			if (!this.panel.activeSelf && (CursorManager.consoleOpen || CursorManager.pauseOpen))
			{
				return;
			}
			this.panel.SetActive(!this.panel.activeSelf);
			CursorManager.raOp = this.panel.activeSelf;
			PlayerManager.localPlayer.GetComponent<FirstPersonController>().usingRemoteAdmin = this.panel.activeSelf;
		}

		public static string password;

		public GameObject panel;
	}
}
