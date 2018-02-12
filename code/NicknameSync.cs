using System;
using GameConsole;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NicknameSync : NetworkBehaviour
{
	private void Start()
	{
		if (base.isLocalPlayer)
		{
			string n = string.Empty;
			if (!ServerStatic.isDedicated && SteamManager.Initialized)
			{
				n = ((SteamFriends.GetPersonaName() != null) ? SteamFriends.GetPersonaName() : "Player");
			}
			else
			{
				GameConsole.Console.singleton.AddLog("Steam has been not initialized!", new Color32(byte.MaxValue, 0, 0, byte.MaxValue), false);
				if (PlayerPrefs.HasKey("nickname"))
				{
					n = PlayerPrefs.GetString("nickname");
				}
				else
				{
					string text = "Player " + SystemInfo.processorType + SystemInfo.operatingSystem;
					PlayerPrefs.SetString("nickname", text);
					n = text;
				}
			}
			this.CallCmdSetNick(n);
			this.spectCam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>().cam.transform;
			this.n_text = GameObject.Find("Nickname Text").GetComponent<Text>();
		}
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			bool flag = false;
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(this.spectCam.position, this.spectCam.forward), out raycastHit, this.viewRange, this.raycastMask))
			{
				NicknameSync component = raycastHit.transform.GetComponent<NicknameSync>();
				if (component != null && !component.isLocalPlayer)
				{
					CharacterClassManager component2 = component.GetComponent<CharacterClassManager>();
					CharacterClassManager component3 = base.GetComponent<CharacterClassManager>();
					flag = true;
					this.n_text.color = component2.klasy[component2.curClass].classColor;
					this.n_text.text = string.Empty;
					Text text = this.n_text;
					text.text += component.myNick;
					Text text2 = this.n_text;
					text2.text = text2.text + "\n" + ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? component2.klasy[component2.curClass].fullName : component2.klasy[component2.curClass].fullName_pl);
					try
					{
						if (component2.klasy[component2.curClass].team == Team.MTF && component3.klasy[component3.curClass].team == Team.MTF)
						{
							int num = 0;
							int num2 = 0;
							if (component2.curClass == 4 || component2.curClass == 11)
							{
								num2 = 200;
							}
							else if (component2.curClass == 13)
							{
								num2 = 100;
							}
							else if (component2.curClass == 12)
							{
								num2 = 300;
							}
							if (component3.curClass == 4 || component3.curClass == 11)
							{
								num = 200;
							}
							else if (component3.curClass == 13)
							{
								num = 100;
							}
							else if (component3.curClass == 12)
							{
								num = 300;
							}
							Text text3 = this.n_text;
							text3.text = text3.text + " (" + GameObject.Find("Host").GetComponent<NineTailedFoxUnits>().GetNameById(component2.ntfUnit) + ")\n\n<b>";
							num -= component3.ntfUnit;
							num2 -= component2.ntfUnit;
							if (num > num2)
							{
								Text text4 = this.n_text;
								text4.text += ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "YOU CAN GIVE ORDERS" : "MOŻESZ DAWAĆ ROZKAZY");
							}
							else if (num2 > num)
							{
								Text text5 = this.n_text;
								text5.text += ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "FOLLOW ORDERS" : "WYKONUJ ROZKAZY");
							}
							else if (num2 == num)
							{
								Text text6 = this.n_text;
								text6.text += ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "THE SAME PERMISSION LEVEL" : "RÓWNY POZIOM UPRAWNIEŃ");
							}
							Text text7 = this.n_text;
							text7.text += "</b>";
						}
					}
					catch
					{
						MonoBehaviour.print("Error");
					}
				}
			}
			this.transparency += Time.deltaTime * (float)((!flag) ? -3 : 3);
			if (flag)
			{
				float max = (this.viewRange - Vector3.Distance(base.transform.position, raycastHit.point)) / this.viewRange;
				this.transparency = Mathf.Clamp(this.transparency, 0f, max);
			}
			this.transparency = Mathf.Clamp01(this.transparency);
			CanvasRenderer component4 = this.n_text.GetComponent<CanvasRenderer>();
			component4.SetAlpha(this.transparency);
		}
	}

	[Command(channel = 2)]
	private void CmdSetNick(string n)
	{
		while (n.Contains("<"))
		{
			n = n.Replace("<", "＜");
		}
		while (n.Contains(">"))
		{
			n = n.Replace(">", "＞");
		}
		this.SetNick(n);
	}

	public void SetNick(string nick)
	{
		this.NetworkmyNick = nick;
	}

	private void UNetVersion()
	{
	}

	public string NetworkmyNick
	{
		get
		{
			return this.myNick;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetNick(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<string>(value, ref this.myNick, dirtyBit);
		}
	}

	protected static void InvokeCmdCmdSetNick(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetNick called on client.");
			return;
		}
		((NicknameSync)obj).CmdSetNick(reader.ReadString());
	}

	public void CallCmdSetNick(string n)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSetNick called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSetNick(n);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)NicknameSync.kCmdCmdSetNick);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(n);
		base.SendCommandInternal(networkWriter, 2, "CmdSetNick");
	}

	static NicknameSync()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(NicknameSync), NicknameSync.kCmdCmdSetNick, new NetworkBehaviour.CmdDelegate(NicknameSync.InvokeCmdCmdSetNick));
		NetworkCRC.RegisterBehaviour("NicknameSync", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.myNick);
			return true;
		}
		bool flag = false;
		if ((base.syncVarDirtyBits & 1u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.Write(this.myNick);
		}
		if (!flag)
		{
			writer.WritePackedUInt32(base.syncVarDirtyBits);
		}
		return flag;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (initialState)
		{
			this.myNick = reader.ReadString();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetNick(reader.ReadString());
		}
	}

	[SyncVar(hook = "SetNick")]
	public string myNick;

	public float viewRange;

	private Text n_text;

	private float transparency;

	public LayerMask raycastMask;

	private Transform spectCam;

	private static int kCmdCmdSetNick = 55613980;
}
