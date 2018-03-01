using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TextChat : NetworkBehaviour
{
	public TextChat()
	{
	}

	private void Start()
	{
		if (base.isLocalPlayer)
		{
			TextChat.lply = base.transform;
		}
	}

	private void Update()
	{
		if (base.isLocalPlayer && this.enabledChat)
		{
			for (int i = 0; i < this.msgs.Count; i++)
			{
				if (this.msgs[i] == null)
				{
					this.msgs.RemoveAt(i);
					break;
				}
				this.msgs[i].GetComponent<TextMessage>().position = (float)(this.msgs.Count - i - 1);
			}
			if (Input.GetKeyDown(KeyCode.Return))
			{
				this.SendChat("(づ｡◕‿‿◕｡)づ" + UnityEngine.Random.Range(0, 4654), base.GetComponent<NicknameSync>().myNick, base.transform.position);
			}
		}
	}

	private void SendChat(string msg, string nick, Vector3 position)
	{
		this.CallCmdSendChat(msg, nick, position);
	}

	[Command(channel = 2)]
	private void CmdSendChat(string msg, string nick, Vector3 pos)
	{
		this.CallRpcSendChat(msg, nick, pos);
	}

	[ClientRpc(channel = 2)]
	private void RpcSendChat(string msg, string nick, Vector3 pos)
	{
		if (Vector3.Distance(TextChat.lply.position, pos) < 15f)
		{
			this.AddMsg(msg, nick);
		}
	}

	private void AddMsg(string msg, string nick)
	{
		while (msg.Contains("<"))
		{
			msg = msg.Replace("<", "＜");
		}
		while (msg.Contains(">"))
		{
			msg = msg.Replace(">", "＞");
		}
		string text = "<b>" + nick + "</b>: " + msg;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.textMessagePrefab);
		gameObject.transform.SetParent(this.attachParent);
		this.msgs.Add(gameObject);
		gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<Text>().text = text;
		gameObject.GetComponent<TextMessage>().remainingLife = (float)this.messageDuration;
		UnityEngine.Object.Destroy(gameObject, (float)this.messageDuration);
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeCmdCmdSendChat(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSendChat called on client.");
			return;
		}
		((TextChat)obj).CmdSendChat(reader.ReadString(), reader.ReadString(), reader.ReadVector3());
	}

	public void CallCmdSendChat(string msg, string nick, Vector3 pos)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("Command function CmdSendChat called on server.");
			return;
		}
		if (base.isServer)
		{
			this.CmdSendChat(msg, nick, pos);
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)5));
		networkWriter.WritePackedUInt32((uint)TextChat.kCmdCmdSendChat);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(msg);
		networkWriter.Write(nick);
		networkWriter.Write(pos);
		base.SendCommandInternal(networkWriter, 2, "CmdSendChat");
	}

	protected static void InvokeRpcRpcSendChat(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcSendChat called on server.");
			return;
		}
		((TextChat)obj).RpcSendChat(reader.ReadString(), reader.ReadString(), reader.ReadVector3());
	}

	public void CallRpcSendChat(string msg, string nick, Vector3 pos)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("RPC Function RpcSendChat called on client.");
			return;
		}
		NetworkWriter networkWriter = new NetworkWriter();
		networkWriter.Write(0);
		networkWriter.Write((short)((ushort)2));
		networkWriter.WritePackedUInt32((uint)TextChat.kRpcRpcSendChat);
		networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
		networkWriter.Write(msg);
		networkWriter.Write(nick);
		networkWriter.Write(pos);
		this.SendRPCInternal(networkWriter, 2, "RpcSendChat");
	}

	static TextChat()
	{
		NetworkBehaviour.RegisterCommandDelegate(typeof(TextChat), TextChat.kCmdCmdSendChat, new NetworkBehaviour.CmdDelegate(TextChat.InvokeCmdCmdSendChat));
		TextChat.kRpcRpcSendChat = -734819717;
		NetworkBehaviour.RegisterRpcDelegate(typeof(TextChat), TextChat.kRpcRpcSendChat, new NetworkBehaviour.CmdDelegate(TextChat.InvokeRpcRpcSendChat));
		NetworkCRC.RegisterBehaviour("TextChat", 0);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public int messageDuration;

	private static Transform lply;

	public GameObject textMessagePrefab;

	private Transform attachParent;

	public bool enabledChat;

	private List<GameObject> msgs = new List<GameObject>();

	private static int kCmdCmdSendChat = -683434843;

	private static int kRpcRpcSendChat;
}
