using System;
using TMPro;
using UnityEngine;

namespace RemoteAdmin
{
	public class PlayerInspector : RemoteAdminBehaviour
	{
		private void Start()
		{
			this.monitor = base.GetComponent<PlayersMonitor>();
		}

		private void Update()
		{
			if (this.rootObject.activeSelf)
			{
				this.text.text = string.Empty;
				foreach (PlayersMonitor.Player player in this.monitor.players)
				{
					string text = "[unconnected]";
					Class @class = null;
					if (player.playerInfo.instance != null)
					{
						try
						{
							text = player.playerInfo.instance.GetComponent<NicknameSync>().myNick;
							@class = PlayerManager.localPlayer.GetComponent<CharacterClassManager>().klasy[player.playerInfo.instance.GetComponent<CharacterClassManager>().curClass];
						}
						catch
						{
						}
					}
					TextMeshProUGUI textMeshProUGUI = this.text;
					string text2 = textMeshProUGUI.text;
					textMeshProUGUI.text = string.Concat(new string[]
					{
						text2,
						"\n<b>",
						text,
						"</b> | IP: <b>",
						player.playerInfo.address,
						"</b> | Connection time: <b>",
						player.playerInfo.time,
						"</b> | Class: <b>",
						(@class != null) ? (PlayerInspector.ColorToHex(@class.classColor) + @class.fullName + "</color></b>") : "none</b>"
					});
				}
			}
		}

		public static string ColorToHex(Color c)
		{
			Color32 color = c;
			string str = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			return "<color=#" + str + ">";
		}

		public TextMeshProUGUI text;

		public GameObject rootObject;

		private PlayersMonitor monitor;
	}
}
