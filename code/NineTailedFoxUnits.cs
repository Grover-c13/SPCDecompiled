using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NineTailedFoxUnits : NetworkBehaviour
{
	public NineTailedFoxUnits()
	{
		this.list = new SyncListString();
	}

	private void SetList(SyncListString l)
	{
		this.list = l;
	}

	private void AddUnit(string unit)
	{
		this.list.Add(unit);
	}

	private string GenerateName()
	{
		return this.names[UnityEngine.Random.Range(0, this.names.Length)] + "-" + UnityEngine.Random.Range(1, 20);
	}

	private void Start()
	{
		this.ccm = base.GetComponent<CharacterClassManager>();
		this.txtlist = GameObject.Find("NTFlist").GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			if (this.host == null)
			{
				GameObject gameObject = GameObject.Find("Host");
				if (gameObject != null)
				{
					this.host = gameObject.GetComponent<NineTailedFoxUnits>();
				}
				return;
			}
			this.txtlist.text = string.Empty;
			if (this.ccm.curClass > 0 && this.ccm.klasy[this.ccm.curClass].team == Team.MTF)
			{
				for (int i = 0; i < this.host.list.Count; i++)
				{
					if (i == this.ccm.ntfUnit)
					{
						TextMeshProUGUI textMeshProUGUI = this.txtlist;
						textMeshProUGUI.text = textMeshProUGUI.text + "<u>" + this.host.GetNameById(i) + "</u>";
					}
					else
					{
						TextMeshProUGUI textMeshProUGUI2 = this.txtlist;
						textMeshProUGUI2.text += this.host.GetNameById(i);
					}
					TextMeshProUGUI textMeshProUGUI3 = this.txtlist;
					textMeshProUGUI3.text += "\n";
				}
			}
		}
	}

	public int NewName()
	{
		int num = 0;
		string text = this.GenerateName();
		while (this.list.Contains(text) && num < 100)
		{
			num++;
			text = this.GenerateName();
		}
		this.AddUnit(text);
		return this.list.Count - 1;
	}

	public string GetNameById(int id)
	{
		return this.list[id];
	}

	private void UNetVersion()
	{
	}

	protected static void InvokeSyncListlist(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("SyncList list called on server.");
			return;
		}
		((NineTailedFoxUnits)obj).list.HandleMsg(reader);
	}

	static NineTailedFoxUnits()
	{
		NetworkBehaviour.RegisterSyncListDelegate(typeof(NineTailedFoxUnits), NineTailedFoxUnits.kListlist, new NetworkBehaviour.CmdDelegate(NineTailedFoxUnits.InvokeSyncListlist));
		NetworkCRC.RegisterBehaviour("NineTailedFoxUnits", 0);
	}

	private void Awake()
	{
		this.list.InitializeBehaviour(this, NineTailedFoxUnits.kListlist);
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			SyncListString.WriteInstance(writer, this.list);
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
			SyncListString.WriteInstance(writer, this.list);
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
			SyncListString.ReadReference(reader, this.list);
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			SyncListString.ReadReference(reader, this.list);
		}
	}

	public string[] names;

	[SyncVar(hook = "SetList")]
	public SyncListString list;

	private CharacterClassManager ccm;

	private TextMeshProUGUI txtlist;

	private NineTailedFoxUnits host;

	private static int kListlist = -376129279;
}
