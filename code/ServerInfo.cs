using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ServerInfo : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public ServerInfo()
	{
	}

	public static void ShowInfo(string id)
	{
		ServerInfo serverInfo = UnityEngine.Object.FindObjectOfType<ServerInfo>();
		serverInfo.StartCoroutine(serverInfo.Show(id));
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		int num = TMP_TextUtilities.FindIntersectingLink(this.text, Input.mousePosition, null);
		if (num != -1)
		{
			TMP_LinkInfo tmp_LinkInfo = this.text.textInfo.linkInfo[num];
			Application.OpenURL(tmp_LinkInfo.GetLinkID());
		}
	}

	public IEnumerator Show(string id)
	{
		this.root.SetActive(true);
		this.text.text = string.Empty;
		if (id.Contains("/"))
		{
			this.text.text = "The URL isn't directing to pastebin site. Please contact server owner.";
		}
		WWW www = new WWW("https://pastebin.com/raw/" + id);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			this.text.text = www.text;
		}
		else
		{
			this.text.text = www.error;
		}
		yield break;
	}

	public void Close()
	{
		UnityEngine.Object.FindObjectOfType<ServerInfo>().root.SetActive(false);
	}

	private Canvas canvas;

	public GameObject root;

	public TextMeshProUGUI text;

	[CompilerGenerated]
	private sealed class <Show>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Show>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.root.SetActive(true);
				this.$this.text.text = string.Empty;
				if (this.id.Contains("/"))
				{
					this.$this.text.text = "The URL isn't directing to pastebin site. Please contact server owner.";
				}
				this.<www>__1 = new WWW("https://pastebin.com/raw/" + this.id);
				this.$current = this.<www>__1;
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
				if (string.IsNullOrEmpty(this.<www>__1.error))
				{
					this.$this.text.text = this.<www>__1.text;
				}
				else
				{
					this.$this.text.text = this.<www>__1.error;
				}
				this.$PC = -1;
				break;
			}
			return false;
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

		internal string id;

		internal WWW <www>__1;

		internal ServerInfo $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
