using System;
using System.Collections.Generic;
using Dissonance;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RadioInitializator : NetworkBehaviour
{
	private void Start()
	{
		this.pm = PlayerManager.singleton;
		try
		{
			this.parent = GameObject.Find("VoicechatPopups").transform;
		}
		catch
		{
		}
	}

	private void LateUpdate()
	{
		if (base.isLocalPlayer)
		{
			foreach (GameObject gameObject in this.pm.players)
			{
				if (gameObject != base.gameObject)
				{
					Radio component = gameObject.GetComponent<Radio>();
					component.SetRelationship();
					string playerId = gameObject.GetComponent<HlapiPlayer>().PlayerId;
					VoicePlayback component2 = component.mySource.GetComponent<VoicePlayback>();
					bool flag = component.mySource.spatialBlend == 0f && component2.Priority != ChannelPriority.None && component.ShouldBeVisible(base.gameObject);
					if (RadioInitializator.names.Contains(playerId))
					{
						int index = RadioInitializator.names.IndexOf(playerId);
						if (!flag)
						{
							UnityEngine.Object.Destroy(RadioInitializator.spawns[index]);
							RadioInitializator.spawns.RemoveAt(index);
							RadioInitializator.names.RemoveAt(index);
							return;
						}
						RadioInitializator.spawns[index].GetComponent<Image>().color = this.color_in.Evaluate(component2.Amplitude * 3f);
						RadioInitializator.spawns[index].GetComponent<Outline>().effectColor = this.color_out.Evaluate(component2.Amplitude * 3f);
					}
					else if (flag)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.prefab, this.parent);
						gameObject2.transform.localScale = Vector3.one;
						gameObject2.GetComponentInChildren<Text>().text = component.GetComponent<NicknameSync>().myNick;
						RadioInitializator.spawns.Add(gameObject2);
						RadioInitializator.names.Add(playerId);
						return;
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		try
		{
			int index = RadioInitializator.names.IndexOf(base.GetComponent<HlapiPlayer>().PlayerId);
			UnityEngine.Object.Destroy(RadioInitializator.spawns[index]);
			RadioInitializator.spawns.RemoveAt(index);
			RadioInitializator.names.RemoveAt(index);
		}
		catch
		{
		}
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private PlayerManager pm;

	private Transform parent;

	public GameObject prefab;

	private static List<GameObject> spawns = new List<GameObject>();

	private static List<string> names = new List<string>();

	public Gradient color_out;

	public Gradient color_in;
}
