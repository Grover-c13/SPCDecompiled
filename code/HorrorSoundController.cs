using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HorrorSoundController : NetworkBehaviour
{
	private void Start()
	{
		this.pmng = PlayerManager.singleton;
		this.cmng = base.GetComponent<CharacterClassManager>();
	}

	public void BlindSFX()
	{
		this.horrorSoundSource.PlayOneShot(this.blindedSoundClip);
	}

	private void Update()
	{
		if (base.isLocalPlayer)
		{
			if (this.cmng.curClass < 0 || this.cmng.curClass == 2)
			{
				return;
			}
			if (this.cmng.klasy[this.cmng.curClass].team == Team.SCP)
			{
				return;
			}
			List<GameObject> list = new List<GameObject>();
			GameObject[] players = this.pmng.players;
			foreach (GameObject gameObject in players)
			{
				CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
				if (component.curClass >= 0 && component.klasy[component.curClass].team == Team.SCP)
				{
					list.Add(gameObject);
				}
			}
			List<GameObject> list2 = new List<GameObject>();
			foreach (GameObject gameObject2 in list)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(this.plyCamera.position, (gameObject2.transform.position - this.plyCamera.position).normalized, out raycastHit, 50f, this.mask) && Mathf.Abs(Vector3.Angle(base.transform.forward, raycastHit.normal) + base.transform.rotation.y - 180f) < 50f && raycastHit.transform.GetComponentInParent<CharacterClassManager>() != null && base.GetComponent<CharacterClassManager>().klasy[raycastHit.transform.GetComponentInParent<CharacterClassManager>().curClass].team == Team.SCP)
				{
					list2.Add(gameObject2);
				}
			}
			if (list2.Count == 0)
			{
				this.cooldown -= Time.deltaTime;
				if (this.cooldown < 0f && !TutorialManager.status)
				{
					SoundtrackManager.singleton.StopOverlay(0);
				}
				return;
			}
			if (this.cooldown < 0f)
			{
				float num = float.PositiveInfinity;
				foreach (GameObject gameObject3 in list2)
				{
					float num2 = Vector3.Distance(base.transform.position, gameObject3.transform.position);
					if (num2 < num)
					{
						num = num2;
					}
				}
				for (int j = 0; j < this.sounds.Length; j++)
				{
					if (this.sounds[j].distance > num)
					{
						this.horrorSoundSource.PlayOneShot(this.sounds[j].clip);
						this.cooldown = 20f;
						SoundtrackManager.singleton.PlayOverlay(0);
						return;
					}
				}
			}
			this.cooldown = 20f;
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

	private CharacterClassManager cmng;

	public LayerMask mask;

	[SerializeField]
	private Transform plyCamera;

	public AudioSource horrorSoundSource;

	[SerializeField]
	private HorrorSoundController.DistanceSound[] sounds;

	private float cooldown = 20f;

	private PlayerManager pmng;

	public AudioClip blindedSoundClip;

	[Serializable]
	public struct DistanceSound
	{
		public float distance;

		public AudioClip clip;
	}
}
