using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ServerListManager : MonoBehaviour
{
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
		this.loadingText.text = ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "DOWNLOADING DATA" : "POBIERANIE DANYCH");
		foreach (GameObject obj in this.spawns)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.spawns.Clear();
		WWW www = new WWW("https://hubertmoszka.pl/lobbylist.php");
		yield return www;
		this.loadingText.text = ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "NO SERVERS AVAILABLE" : "BRAK AKTYWNYCH SERWERÓW");
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
									string text = this.Base64Decode(info[2]).Split(new string[]
									{
										":[:BREAK:]:"
									}, StringSplitOptions.None)[0];
									if (this.filters.AllowToSpawn(text))
									{
										this.loadingText.text = string.Empty;
										PlayButton component = this.AddRecord().GetComponent<PlayButton>();
										component.ip = this.Base64Decode(info[0]);
										component.port = this.Base64Decode(info[1]);
										component.motd.text = text;
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
}
