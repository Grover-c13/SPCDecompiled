using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	public TutorialManager()
	{
	}

	private void Awake()
	{
		string name = SceneManager.GetActiveScene().name;
		TutorialManager.status = name.Contains("Tutorial");
		if (TutorialManager.status)
		{
			TutorialManager.levelID = int.Parse(name.Remove(0, name.IndexOf("0")));
			this.logs = this.tutorials[TutorialManager.levelID - 1].logs;
		}
	}

	private void Start()
	{
		if (TutorialManager.status)
		{
			TutorialManager.curlog = -1;
			this.fpc = base.GetComponent<FirstPersonController>();
			this.src = GameObject.Find("Lector").GetComponent<AudioSource>();
			this.txt = GameObject.FindGameObjectWithTag("Respawn").GetComponent<TextMeshProUGUI>();
		}
	}

	private void LateUpdate()
	{
		if (!TutorialManager.status)
		{
			return;
		}
		this.fpc.tutstop = false;
		if (TutorialManager.curlog >= 0 && this.logs[TutorialManager.curlog].stopPlayer)
		{
			this.fpc.tutstop = true;
		}
		if (this.timeToNext > 0f)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				this.timeToNext = 0f;
				this.src.Stop();
			}
			this.timeToNext -= Time.deltaTime;
			if (this.timeToNext <= 0f && TutorialManager.curlog != -1)
			{
				this.txt.text = string.Empty;
				if (this.logs[TutorialManager.curlog].jumpforward)
				{
					this.Trigger(TutorialManager.curlog + 1);
				}
				else
				{
					TutorialManager.curlog = -1;
				}
			}
		}
		if (TutorialManager.curlog != -1 && this.timeToNext <= 0f)
		{
			this.timeToNext = this.logs[TutorialManager.curlog].duration_en;
			if (this.logs[TutorialManager.curlog].clip_en != null)
			{
				this.src.PlayOneShot(this.logs[TutorialManager.curlog].clip_en);
			}
			if (this.logs[TutorialManager.curlog].duration_en > 0f)
			{
				this.txt.text = TranslationReader.Get("Tutorial_" + TutorialManager.levelID.ToString("00"), TutorialManager.curlog);
			}
		}
	}

	public void Trigger(int id)
	{
		TutorialManager.curlog = id;
		if (this.logs[id].duration_en == -100f)
		{
			PlayerPrefs.SetInt("TutorialProgress", TutorialManager.levelID + 1);
			NetworkManager.singleton.StopHost();
		}
		if (this.logs[id].duration_en == -200f)
		{
			base.SendMessage(this.logs[id].content_en);
			if (this.logs[id].jumpforward)
			{
				this.Trigger(id + 1);
			}
		}
		else
		{
			this.src.Stop();
			this.txt.text = string.Empty;
			this.timeToNext = 0f;
		}
	}

	public void Trigger(string alias)
	{
		for (int i = 0; i < this.logs.Count; i++)
		{
			if (this.logs[i].alias == alias)
			{
				this.Trigger(i);
				break;
			}
		}
	}

	public void KillNPC()
	{
		this.npcKills++;
		KillTrigger[] array = UnityEngine.Object.FindObjectsOfType<KillTrigger>();
		KillTrigger killTrigger = null;
		foreach (KillTrigger killTrigger2 in array)
		{
			if (killTrigger == null || killTrigger2.prioirty < killTrigger.prioirty)
			{
				killTrigger = killTrigger2;
			}
		}
		if (killTrigger != null)
		{
			killTrigger.Trigger(this.npcKills);
		}
	}

	public void Reload()
	{
		this.reloads++;
	}

	private void Tutorial2_GiveNTFRifle()
	{
		UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<NoammoTrigger>().gameObject);
		GameObject.Find("Host").GetComponent<Inventory>().CallCmdSetPickup(20, 0f, GameObject.Find("ItemPos").transform.position, Quaternion.Euler(-90f, 0f, 0f), default(Quaternion));
		base.Invoke("Tutorial2_GiveSFA", 1f);
	}

	private void Tutorial2_GiveAmmo()
	{
		foreach (Pickup pickup in UnityEngine.Object.FindObjectsOfType<Pickup>())
		{
			if (pickup.id == 29)
			{
				return;
			}
		}
		GameObject.Find("Host").GetComponent<Inventory>().CallCmdSetPickup(29, 12f, GameObject.Find("ItemPos").transform.position, default(Quaternion), default(Quaternion));
	}

	private void Tutorial2_MoreAmmo()
	{
		foreach (Pickup pickup in UnityEngine.Object.FindObjectsOfType<Pickup>())
		{
			if (pickup.id == 29)
			{
				return;
			}
		}
		GameObject.Find("Host").GetComponent<Inventory>().CallCmdSetPickup(29, 12f, GameObject.Find("ItemPos").transform.position, default(Quaternion), default(Quaternion));
		this.Trigger(5);
	}

	private void Tutorial2_Jumpin()
	{
		this.Trigger("epsilon");
	}

	private void Tutorial2_Curtain()
	{
		GameObject.Find("Curtain").GetComponent<AudioSource>().Play();
		GameObject.Find("Curtain").GetComponent<Animator>().SetBool("Open", !GameObject.Find("Curtain").GetComponent<Animator>().GetBool("Open"));
	}

	private void Tutorial2_GiveSFA()
	{
		GameObject.Find("Host").GetComponent<Inventory>().CallCmdSetPickup(22, 1E+08f, GameObject.Find("ItemPos").transform.position, default(Quaternion), default(Quaternion));
	}

	private void Tutorial2_ResultText()
	{
		GameObject.Find("ResultText").GetComponent<Text>().text = (this.npcKills - 9).ToString("00");
	}

	public void Tutorial2_Preset()
	{
		UnityEngine.Object.FindObjectOfType<MainMenuScript>().ChangeMenu(UnityEngine.Object.FindObjectOfType<MainMenuScript>().curMenu + 1);
	}

	public void Tutorial2_Result()
	{
		this.Tutorial2_Curtain();
		if (this.reloads == 1)
		{
			this.Trigger("result_good");
		}
		else if (this.reloads == 2)
		{
			this.Trigger("result_ok");
		}
		else
		{
			this.Trigger("result_bad");
		}
	}

	private void Tutorial3_GiveKeycard()
	{
		GameObject.Find("Host").GetComponent<Inventory>().CallCmdSetPickup(0, 0f, GameObject.Find("ItemPos").transform.position, default(Quaternion), default(Quaternion));
	}

	public void Tutorial3_KeycardBurnt()
	{
		this.burns++;
		if (this.burns == 1)
		{
			this.Trigger("bc1");
			base.Invoke("Tutorial3_GiveKeycard", 3f);
		}
		if (this.burns == 2)
		{
			this.Trigger("bc2");
			base.Invoke("Tutorial3_GiveKeycard", 5f);
		}
		if (this.burns == 3)
		{
			this.Trigger("bc3");
			for (int i = 1; i <= 5; i++)
			{
				base.Invoke("Tutorial3_GiveKeycard", (float)(1 + i / 5));
			}
		}
		if (this.burns == 8)
		{
			this.Trigger("bc4");
		}
	}

	private void Tutorial3_Quit()
	{
		Application.Quit();
	}

	static TutorialManager()
	{
		// Note: this type is marked as 'beforefieldinit'.
	}

	public static bool status;

	public static int levelID;

	private FirstPersonController fpc;

	private TextMeshProUGUI txt;

	public TutorialManager.TutorialScene[] tutorials;

	private List<TutorialManager.Log> logs = new List<TutorialManager.Log>();

	private AudioSource src;

	public static int curlog = -1;

	private float timeToNext;

	private int npcKills;

	private int reloads;

	private int burns;

	[Serializable]
	public class TutorialScene
	{
		public TutorialScene()
		{
		}

		public List<TutorialManager.Log> logs = new List<TutorialManager.Log>();
	}

	[Serializable]
	public class Log
	{
		public Log()
		{
		}

		[Multiline]
		public string content_en;

		public AudioClip clip_en;

		public float duration_en;

		public bool jumpforward;

		public bool stopPlayer;

		public string alias;
	}
}
