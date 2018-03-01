using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RagdollManager : NetworkBehaviour
{
	public RagdollManager()
	{
	}

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
				text = TranslationReader.Get("Death_Causes", 12);
				text = text.Replace("[user]", componentInParent.owner.steamClientName);
				text = text.Replace("[cause]", RagdollManager.GetCause(componentInParent.owner.deathCause, false));
				text = text.Replace("[class]", string.Concat(new string[]
				{
					"<color=",
					this.GetColor(this.ccm.klasy[componentInParent.owner.charclass].classColor),
					">",
					this.ccm.klasy[componentInParent.owner.charclass].fullName,
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
		string result = TranslationReader.Get("Death_Causes", 11);
		int num = -1;
		if (info.tool == "NUKE")
		{
			result = TranslationReader.Get("Death_Causes", 0);
		}
		else if (info.tool == "FALLDOWN")
		{
			result = TranslationReader.Get("Death_Causes", 1);
		}
		else if (info.tool == "LURE")
		{
			result = TranslationReader.Get("Death_Causes", 2);
		}
		else if (info.tool == "POCKET")
		{
			result = TranslationReader.Get("Death_Causes", 3);
		}
		else if (info.tool == "CONTAIN")
		{
			result = TranslationReader.Get("Death_Causes", 4);
		}
		else if (info.tool == "TESLA")
		{
			result = TranslationReader.Get("Death_Causes", 5);
		}
		else if (info.tool == "WALL")
		{
			result = TranslationReader.Get("Death_Causes", 6);
		}
		else if (info.tool.Length > 7 && info.tool.Substring(0, 7) == "Weapon:" && int.TryParse(info.tool.Remove(0, 7), out num) && num != -1)
		{
			GameObject gameObject = GameObject.Find("Host");
			WeaponManager.Weapon weapon = gameObject.GetComponent<WeaponManager>().weapons[num];
			AmmoBox component = gameObject.GetComponent<AmmoBox>();
			result = TranslationReader.Get("Death_Causes", 7).Replace("[ammotype]", component.types[weapon.ammoType].label);
		}
		else if (info.tool.Length > 4 && info.tool.Substring(0, 4) == "SCP:" && int.TryParse(info.tool.Remove(0, 4), out num) && num != -1)
		{
			if (num == 173)
			{
				result = TranslationReader.Get("Death_Causes", 8);
			}
			else if (num == 106)
			{
				result = TranslationReader.Get("Death_Causes", 9);
			}
			else if (num == 49 || num == 492)
			{
				result = TranslationReader.Get("Death_Causes", 10);
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

	private static int kCmdCmdRegisterScpFrag = 646872709;
}
