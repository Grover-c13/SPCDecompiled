using System;
using System.Collections;
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
}
