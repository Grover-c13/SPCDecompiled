using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Spectator : NetworkBehaviour
{
	private void Start()
	{
		this.spectInfo = UserMainInterface.singleton.specatorInfo;
		this.pm = PlayerManager.singleton;
		this.ccm = base.GetComponent<CharacterClassManager>();
	}

	public void Init()
	{
		if (TutorialManager.status)
		{
			return;
		}
		this.isSpect = (this.ccm.curClass == 2);
		if (base.isLocalPlayer)
		{
			this.spectCam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>();
			if (this.ccm.curClass != -1 && this.ccm.curClass != 2)
			{
				this.deadCam = true;
			}
			this.RefreshSpectList();
			this.ActivateCorrectCameras();
			if (this.isSpect)
			{
				base.transform.position = Vector3.up * 200f;
			}
		}
	}

	private void ActivateCorrectCameras()
	{
		if (this.spectCam == null)
		{
			this.spectCam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>();
			return;
		}
		this.spectCam.freeCam.enabled = (this.alivePlayers.Count == 0 && this.isSpect);
		this.spectCam.cam.enabled = (this.alivePlayers.Count != 0 && this.isSpect);
		this.spectCam.cam079.enabled = (this.ccm.curClass == 7);
		this.mainCam.enabled = !this.isSpect;
		this.weaponCams.SetActive(!this.isSpect && this.ccm.curClass != 7);
		this.mainCam.gameObject.SetActive(this.ccm.curClass != 7);
	}

	private void Update()
	{
		if (base.isLocalPlayer && this.isSpect)
		{
			this.WaitForInput();
		}
	}

	private void LateUpdate()
	{
		if (base.isLocalPlayer && !this.isSpect)
		{
			if (this.spectCam == null)
			{
				this.spectCam = UnityEngine.Object.FindObjectOfType<SpectatorCamera>();
				return;
			}
			if (this.ccm.curClass != 7)
			{
				this.spectCam.cam.transform.position = this.mainCam.transform.position;
				this.spectCam.cam.transform.rotation = this.mainCam.transform.rotation;
			}
		}
		if (base.isLocalPlayer)
		{
			if (this.isTracking)
			{
				this.isTracking = false;
				GameObject gameObject = this.alivePlayers[this.curFocus];
				this.spectInfo.color = this.ccm.klasy[gameObject.GetComponent<CharacterClassManager>().curClass].classColor;
				this.spectInfo.text = string.Concat(new object[]
				{
					gameObject.GetComponent<NicknameSync>().myNick,
					" ",
					gameObject.GetComponent<PlayerStats>().health,
					"HP"
				});
			}
			else
			{
				this.spectInfo.text = string.Empty;
			}
		}
	}

	private void WaitForInput()
	{
		if (Input.anyKeyDown)
		{
			this.RefreshSpectList();
			this.ActivateCorrectCameras();
		}
		if (Input.GetButtonDown("Spectator: Next Player"))
		{
			this.curFocus++;
		}
		if (Input.GetButtonDown("Spectator: Previous Player"))
		{
			this.curFocus++;
		}
		if (this.alivePlayers.Count > 0)
		{
			if (this.deadCam && Input.anyKeyDown)
			{
				this.deadCam = false;
			}
			else if (!this.deadCam)
			{
				if (this.curFocus >= this.alivePlayers.Count)
				{
					this.curFocus = 0;
				}
				if (this.curFocus < 0)
				{
					this.curFocus = this.alivePlayers.Count - 1;
				}
				if (this.curFocus >= 0 && this.curFocus < this.alivePlayers.Count && this.alivePlayers[this.curFocus].GetComponent<CharacterClassManager>().curClass != 2)
				{
					this.TrackMovement(this.alivePlayers[this.curFocus]);
				}
			}
		}
	}

	private void TrackMovement(GameObject ply)
	{
		this.isTracking = true;
		PlyMovementSync component = ply.GetComponent<PlyMovementSync>();
		this.spectCam.transform.position = ply.GetComponent<Spectator>().spectCamPos.position;
		this.spectCam.transform.rotation = Quaternion.Lerp(this.spectCam.transform.rotation, Quaternion.Euler(new Vector3(component.rotX, component.rotation, 0f)), Time.deltaTime * 15f);
	}

	private void RefreshSpectList()
	{
		this.alivePlayers.Clear();
		GameObject[] players = this.pm.players;
		foreach (GameObject gameObject in players)
		{
			CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
			if (component.curClass != 2 && component.curClass != -1)
			{
				this.alivePlayers.Add(gameObject);
			}
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

	private CharacterClassManager ccm;

	public bool isSpect;

	private PlayerManager pm;

	public Camera mainCam;

	private SpectatorCamera spectCam;

	public GameObject weaponCams;

	public Transform spectCamPos;

	private List<GameObject> alivePlayers = new List<GameObject>();

	private int curFocus;

	private bool deadCam;

	private Text spectInfo;

	private bool isTracking;
}
