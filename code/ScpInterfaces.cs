using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScpInterfaces : MonoBehaviour
{
	public ScpInterfaces()
	{
	}

	private GameObject FindLocalPlayer()
	{
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
				return gameObject;
			}
		}
		return null;
	}

	public void CreatePortal()
	{
		this.FindLocalPlayer().GetComponent<Scp106PlayerScript>().CreatePortalInCurrentPosition();
	}

	public void Update106Highlight(int id)
	{
		this.FindLocalPlayer().GetComponent<Scp106PlayerScript>().highlightID = id;
	}

	public void Use106Portal()
	{
		this.FindLocalPlayer().GetComponent<Scp106PlayerScript>().UseTeleport();
	}

	public GameObject Scp106_eq;

	public TextMeshProUGUI Scp106_ability_highlight;

	public Text Scp106_ability_points;

	public GameObject Scp049_eq;

	public Image Scp049_loading;
}
