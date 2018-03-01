using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RemoteAdmin
{
	public class PlayersMonitor : RemoteAdminBehaviour
	{
		public PlayersMonitor()
		{
		}

		public void PlayerList_Clear(int removeAt = -1)
		{
			if (removeAt == -1)
			{
				foreach (PlayersMonitor.Player player in this.players)
				{
					player.Destroy();
				}
				this.players.Clear();
			}
			else
			{
				this.players[removeAt].Destroy();
				this.players.RemoveAt(removeAt);
			}
		}

		public void SelectPlayer(int id)
		{
			bool selected = !this.players[id].GetSelected();
			if (!Input.GetKey(KeyCode.LeftControl))
			{
				foreach (PlayersMonitor.Player player in this.players)
				{
					player.SetSelected(false);
				}
			}
			this.players[id].SetSelected(selected);
			this.component_ban.RefreshGUI();
		}

		public override void Reply(QueryProcessor.PlayerInfo[] reply)
		{
			List<string> list = new List<string>();
			foreach (PlayersMonitor.Player player in this.players)
			{
				if (player.GetSelected())
				{
					list.Add(player.playerInfo.address);
				}
			}
			this.PlayerList_Clear(-1);
			for (int i = 0; i < reply.Length; i++)
			{
				this.players.Add(new PlayersMonitor.Player(reply[i], this.template, this.parent, i));
			}
			foreach (string b in list)
			{
				for (int j = 0; j < this.players.Count; j++)
				{
					if (this.players[j].playerInfo.address == b)
					{
						this.players[j].SetSelected(true);
					}
				}
			}
			this.component_ban.RefreshGUI();
		}

		private IEnumerator Start()
		{
			this.component_ban = base.GetComponent<Ban>();
			for (;;)
			{
				while (base.GetComponent<Login>().loggedIn)
				{
					this.Request();
					yield return new WaitForSeconds(4f);
				}
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		public void Request()
		{
			PlayerManager.localPlayer.GetComponent<QueryProcessor>().CallCmdSendQuery("GetPlayers", PasswordHolder.password);
		}

		private Ban component_ban;

		public GameObject parent;

		public GameObject template;

		public List<PlayersMonitor.Player> players = new List<PlayersMonitor.Player>();

		[Serializable]
		public class Player
		{
			public Player(QueryProcessor.PlayerInfo info, GameObject template, GameObject parent, int id)
			{
				this.playerInfo = info;
				this.record = UnityEngine.Object.Instantiate<GameObject>(template, parent.transform).GetComponent<PlayerRecord>();
				this.record.gameObject.transform.localScale = Vector3.one;
				this.record.Setup(id, (!(this.playerInfo.instance == null)) ? this.playerInfo.instance.GetComponent<NicknameSync>().myNick : "(unconnected)");
			}

			public void SetSelected(bool b)
			{
				if (b != this.selected)
				{
					this.selected = b;
					this.record.Select(b);
				}
			}

			public bool GetSelected()
			{
				return this.selected;
			}

			public void Destroy()
			{
				UnityEngine.Object.DestroyImmediate(this.record.gameObject);
			}

			public PlayerRecord record;

			public QueryProcessor.PlayerInfo playerInfo;

			private bool selected;
		}

		[CompilerGenerated]
		private sealed class <Start>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <Start>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.$this.component_ban = this.$this.GetComponent<Ban>();
					break;
				case 1u:
					IL_6F:
					if (!this.$this.GetComponent<Login>().loggedIn)
					{
						this.$current = new WaitForEndOfFrame();
						if (!this.$disposing)
						{
							this.$PC = 2;
						}
					}
					else
					{
						this.$this.Request();
						this.$current = new WaitForSeconds(4f);
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
					}
					return true;
				case 2u:
					break;
				default:
					return false;
				}
				goto IL_6F;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$disposing = true;
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			internal PlayersMonitor $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
