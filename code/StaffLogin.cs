using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class StaffLogin : MonoBehaviour
{
	public StaffLogin()
	{
	}

	private void Awake()
	{
		StaffLogin.singleton = this;
	}

	public void UpdateServer()
	{
		base.StartCoroutine(this.IUpdateServer());
	}

	private IEnumerator IUpdateServer()
	{
		yield return new WaitForEndOfFrame();
		CustomNetworkManager cnm = base.GetComponent<CustomNetworkManager>();
		WWWForm form = new WWWForm();
		string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/staff_login.txt";
		string nickname = string.Empty;
		string password = string.Empty;
		if (File.Exists(path))
		{
			StreamReader streamReader = new StreamReader(path);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			nickname = text.Remove(0, text.IndexOf("[") + 1);
			nickname = nickname.Remove(nickname.IndexOf("]"));
			password = text.Remove(0, text.LastIndexOf("[") + 1);
			password = password.Remove(password.IndexOf("]"));
		}
		if (!string.IsNullOrEmpty(nickname) && !string.IsNullOrEmpty(password))
		{
			string code = this.GetRandomString();
			string server_connected = string.Concat(new object[]
			{
				cnm.networkAddress,
				":",
				cnm.networkPort,
				":",
				code
			});
			form.AddField("nickname", nickname);
			form.AddField("password", password);
			form.AddField("server_connected", server_connected);
			WWW www = new WWW("https://hubertmoszka.pl/staffsite.php", form);
			yield return www;
			while (PlayerManager.localPlayer == null)
			{
				yield return new WaitForEndOfFrame();
			}
			PlayerManager.localPlayer.GetComponent<ServerRoles>().CallCmdRequestRole(server_connected);
		}
		yield break;
	}

	public string GetRandomString()
	{
		string text = string.Empty;
		for (int i = 0; i < 16; i++)
		{
			text += "qweSDFrty&*_uiopasdfghjkl#$%^zxcvbnmQWE[{]}RTYUI|~`OPAGHJ()KLZXCV'></BNM;1234@-=567890!+"[UnityEngine.Random.Range(0, 88)];
		}
		return text;
	}

	public static StaffLogin singleton;
}
