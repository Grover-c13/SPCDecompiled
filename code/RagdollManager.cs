using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RagdollManager : NetworkBehaviour
{
	public void SpawnRagdoll(Vector3 pos, Quaternion rot, int classID, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick)
	{
		Class @class = base.GetComponent<CharacterClassManager>().klasy[1];
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(@class.model_ragdoll, pos + @class.ragdoll_offset.position, Quaternion.Euler(rot.eulerAngles + @class.ragdoll_offset.rotation));
		NetworkServer.Spawn(gameObject);
		gameObject.GetComponent<Ragdoll>().SetOwner(new Ragdoll.Info(ownerID, ownerNick, ragdollInfo, classID));
		gameObject.GetComponent<Ragdoll>().SetRecall(allowRecall);
		if (ragdollInfo.tool.Contains("SCP") || ragdollInfo.tool == "POCKET")
		{
			this.CallCmdRegisterScpFrag();
		}
	}

	private void Start()
	{
		this.isEN = (PlayerPrefs.GetString("langver", "en") != "pl");
		this.txt = GameObject.Find("BodyInspection").GetComponentInChildren<TextMeshProUGUI>();
		this.cam = base.GetComponent<Scp049PlayerScript>().plyCam.transform;
		this.ccm = base.GetComponent<CharacterClassManager>();
	}

	public void Update()
	{
		if (!base.isLocalPlayer)
		{
			return;
		}
		string text = string.Empty;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(this.cam.position, this.cam.forward), out raycastHit, 3f, this.inspectionMask))
		{
			Ragdoll componentInParent = raycastHit.transform.GetComponentInParent<Ragdoll>();
			if (componentInParent != null)
			{
				text = ((!this.isEN) ? "To ciało <b>[user]</b>, to [class]!\n\nPrzyczyna śmierci: [cause]" : "It's <b>[user]</b>'s body, he was [class]!\n\nCause of death: [cause]");
				text = text.Replace("[user]", componentInParent.owner.steamClientName);
				text = text.Replace("[cause]", RagdollManager.GetCause(componentInParent.owner.deathCause, false));
				text = text.Replace("[class]", string.Concat(new string[]
				{
					"<color=",
					this.GetColor(this.ccm.klasy[componentInParent.owner.charclass].classColor),
					">",
					(!this.isEN) ? this.ccm.klasy[componentInParent.owner.charclass].fullName_pl : this.ccm.klasy[componentInParent.owner.charclass].fullName,
					"</color>"
				}));
			}
		}
		this.txt.text = text;
	}

	public string GetColor(Color c)
	{
		Color32 color = new Color32((byte)(c.r * 255f), (byte)(c.g * 255f), (byte)(c.b * 255f), byte.MaxValue);
		return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	[Command(channel = 2)]
	public void CmdRegisterScpFrag()
	{
		if (RoundSummary.host != null)
		{
			RoundSummary.host.summary.scp_frags++;
		}
	}

	public static string GetCause(PlayerStats.HitInfo info, bool ragdoll)
	{
		bool flag = PlayerPrefs.GetString("langver", "en") != "pl";
		string result = (!flag) ? "Nieznana przyczyna śmierci." : "Unknown cause of death.";
		int num = -1;
		if (info.tool == "NUKE")
		{
			result = ((!flag) ? "Zgon od eksplozji." : "Died of an explosion.");
		}
		else if (info.tool == "FALLDOWN")
		{
			result = ((!flag) ? "Upadek zakończył jego żywot." : "The fall has ended his life.");
		}
		else if (info.tool == "LURE")
		{
			result = ((!flag) ? "Poświęcenie w celu zamknięcia SCP-106." : "Died to re-contain SCP-106.");
		}
		else if (info.tool == "POCKET")
		{
			result = ((!flag) ? "Zaginął w Wymiarze Łuzowym." : "Lost in the Pocket Dimension.");
		}
		else if (info.tool == "CONTAIN")
		{
			result = ((!flag) ? "To jest pozostałość po SCP-106." : "It's a remnant of SCP-106.");
		}
		else if (info.tool == "TESLA")
		{
			result = ((!flag) ? "Porażenie prądem o wysokim napięciu." : "High-voltage electric shock.");
		}
		else if (info.tool == "WALL")
		{
			result = ((!flag) ? "Zmiażdżony przez ciężki obiekt." : "Crushed by a heavy structure.");
		}
		else if (info.tool.Length > 7 && info.tool.Substring(0, 7) == "Weapon:" && int.TryParse(info.tool.Remove(0, 7), out num) && num != -1)
		{
			GameObject gameObject = GameObject.Find("Host");
			WeaponManager.Weapon weapon = gameObject.GetComponent<WeaponManager>().weapons[num];
			AmmoBox component = gameObject.GetComponent<AmmoBox>();
			result = ((!flag) ? "Na ciele widać wiele ran postrzałowych. Broń z której zostały oddane strzały najprawdopodobniej korzysta z amunicji " : "There are many gunshot wounds on the body. Most likely, a ") + component.types[weapon.ammoType].label + ((!flag) ? "." : " ammo type was used.");
		}
		else if (info.tool.Length > 4 && info.tool.Substring(0, 4) == "SCP:" && int.TryParse(info.tool.Remove(0, 4), out num) && num != -1)
		{
			if (num == 173)
			{
				result = ((!flag) ? "Skręcenie karku u podstawy czaszki." : "Snapped neck at the base of the skull.");
			}
			else if (num == 106)
			{
				result = ((!flag) ? "Nie przetrwał spotkania z SCP-106." : "Did not survive the meeting with SCP-106.");
			}
			else if (num == 49 || num == 492)
			{
				result = ((!flag) ? "Został wyleczony z pomoru." : "He was cured of a plague.");
			}
		}
		return result;
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdRegisterScpFrag(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRegisterScpFrag called on client.");
			return;
		}
		((RagdollManager)obj).CmdRegisterScpFrag();
	}

	public void CallCmdRegisterScpFrag()
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdRegisterScpFrag called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdRegisterScpFrag();
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)RagdollManager.kCmdCmdRegisterScpFrag);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		base.SendCommandInternal(networkWriter, 2, "CmdRegisterScpFrag");
	}

	static RagdollManager()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(RagdollManager), RagdollManager.kCmdCmdRegisterScpFrag, new NetworkBehaviour.CmdDelegate(RagdollManager.InvokeCmdCmdRegisterScpFrag));
		NetworkCRC.RegisterBehaviour("RagdollManager", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public LayerMask inspectionMask;

	private Transform cam;

	private CharacterClassManager ccm;

	private TextMeshProUGUI txt;

	private bool isEN = true;

	private static int kCmdCmdRegisterScpFrag = 646872709;
}
