using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ServerListManager : MonoBehaviour
{
	public ServerListManager()
	{
	}

	private void Awake()
	{
		this.filters = base.GetComponent<ServerFilters>();
		ServerListManager.singleton = this;
	}

	public GameObject AddRecord()
	{
		RectTransform rectTransform = UnityEngine.Object.Instantiate<RectTransform>(this.element);
		rectTransform.SetParent(this.contentParent);
		rectTransform.localScale = Vector3.one;
		rectTransform.localPosition = Vector3.zero;
		this.spawns.Add(rectTransform.gameObject);
		this.contentParent.sizeDelta = Vector2.up * 150f * (float)this.spawns.Count;
		return rectTransform.gameObject;
	}

	private void OnEnable()
	{
		this.Refresh();
	}

	public void ApplyNameFilter(string nameFilter)
	{
		this.filters.nameFilter = nameFilter;
		this.Refresh();
	}

	public void Refresh()
	{
		base.StopCoroutine(this.DownloadList());
		base.StartCoroutine(this.DownloadList());
	}

	public IEnumerator DownloadList()
	{
		this.loadingText.text = TranslationReader.Get("MainMenu", 53);
		foreach (GameObject obj in this.spawns)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.spawns.Clear();
		WWW www = new WWW("https://hubertmoszka.pl/lobbylist.php");
		yield return www;
		Text text = this.loadingText;
		string text2 = TranslationReader.Get("MainMenu", 54);
		this.loadingText.text = text2;
		text.text = text2;
		if (string.IsNullOrEmpty(www.error))
		{
			if (www.text.Contains("<br>"))
			{
				string[] results = www.text.Split(new string[]
				{
					"<br>"
				}, StringSplitOptions.None);
				foreach (string server in results)
				{
					if (server.Contains(":"))
					{
						string[] info = server.Split(new char[]
						{
							':'
						});
						if (info.Length == 4)
						{
							try
							{
								if (this.Base64Decode(info[2]).Split(new string[]
								{
									":[:BREAK:]:"
								}, StringSplitOptions.None)[2] == UnityEngine.Object.FindObjectOfType<CustomNetworkManager>().versionstring)
								{
									string text3 = this.Base64Decode(info[2]).Split(new string[]
									{
										":[:BREAK:]:"
									}, StringSplitOptions.None)[0];
									if (this.filters.AllowToSpawn(text3))
									{
										this.loadingText.text = string.Empty;
										PlayButton component = this.AddRecord().GetComponent<PlayButton>();
										component.ip = this.Base64Decode(info[0]);
										component.port = this.Base64Decode(info[1]);
										component.motd.text = text3;
										component.infoType = this.Base64Decode(info[2]).Split(new string[]
										{
											":[:BREAK:]:"
										}, StringSplitOptions.None)[1];
										component.players.text = info[3];
									}
								}
							}
							catch (Exception)
							{
								throw;
							}
							if (this.loadingText.text == string.Empty)
							{
								yield return new WaitForEndOfFrame();
							}
						}
					}
				}
			}
		}
		else
		{
			this.loadingText.text = www.error;
		}
		yield break;
	}

	private string Base64Decode(string t)
	{
		byte[] bytes = Convert.FromBase64String(t);
		return Encoding.UTF8.GetString(bytes);
	}

	public RectTransform contentParent;

	public RectTransform element;

	public Text loadingText;

	public static ServerListManager singleton;

	private ServerFilters filters;

	private List<GameObject> spawns = new List<GameObject>();

	[CompilerGenerated]
	private sealed class <DownloadList>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <DownloadList>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.loadingText.text = TranslationReader.Get("MainMenu", 53);
				this.$locvar0 = this.$this.spawns.GetEnumerator();
				try
				{
					while (this.$locvar0.MoveNext())
					{
						GameObject obj = this.$locvar0.Current;
						UnityEngine.Object.Destroy(obj);
					}
				}
				finally
				{
					((IDisposable)this.$locvar0).Dispose();
				}
				this.$this.spawns.Clear();
				this.<www>__0 = new WWW("https://hubertmoszka.pl/lobbylist.php");
				this.$current = this.<www>__0;
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
			{
				Text loadingText = this.$this.loadingText;
				string text = TranslationReader.Get("MainMenu", 54);
				this.$this.loadingText.text = text;
				loadingText.text = text;
				if (!string.IsNullOrEmpty(this.<www>__0.error))
				{
					this.$this.loadingText.text = this.<www>__0.error;
					goto IL_372;
				}
				if (!this.<www>__0.text.Contains("<br>"))
				{
					goto IL_352;
				}
				this.<results>__1 = this.<www>__0.text.Split(new string[]
				{
					"<br>"
				}, StringSplitOptions.None);
				this.$locvar1 = this.<results>__1;
				this.$locvar2 = 0;
				break;
			}
			case 2u:
				IL_331:
				this.$locvar2++;
				break;
			default:
				return false;
			}
			if (this.$locvar2 < this.$locvar1.Length)
			{
				this.<server>__2 = this.$locvar1[this.$locvar2];
				if (!this.<server>__2.Contains(":"))
				{
					goto IL_331;
				}
				this.<info>__3 = this.<server>__2.Split(new char[]
				{
					':'
				});
				if (this.<info>__3.Length != 4)
				{
					goto IL_331;
				}
				try
				{
					if (this.$this.Base64Decode(this.<info>__3[2]).Split(new string[]
					{
						":[:BREAK:]:"
					}, StringSplitOptions.None)[2] == UnityEngine.Object.FindObjectOfType<CustomNetworkManager>().versionstring)
					{
						string text2 = this.$this.Base64Decode(this.<info>__3[2]).Split(new string[]
						{
							":[:BREAK:]:"
						}, StringSplitOptions.None)[0];
						if (this.$this.filters.AllowToSpawn(text2))
						{
							this.$this.loadingText.text = string.Empty;
							PlayButton component = this.$this.AddRecord().GetComponent<PlayButton>();
							component.ip = this.$this.Base64Decode(this.<info>__3[0]);
							component.port = this.$this.Base64Decode(this.<info>__3[1]);
							component.motd.text = text2;
							component.infoType = this.$this.Base64Decode(this.<info>__3[2]).Split(new string[]
							{
								":[:BREAK:]:"
							}, StringSplitOptions.None)[1];
							component.players.text = this.<info>__3[3];
						}
					}
				}
				catch (Exception)
				{
					throw;
				}
				if (this.$this.loadingText.text == string.Empty)
				{
					this.$current = new WaitForEndOfFrame();
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				goto IL_331;
			}
			IL_352:
			IL_372:
			this.$PC = -1;
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

		internal List<GameObject>.Enumerator $locvar0;

		internal WWW <www>__0;

		internal string[] <results>__1;

		internal string[] $locvar1;

		internal int $locvar2;

		internal string <server>__2;

		internal string[] <info>__3;

		internal ServerListManager $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
