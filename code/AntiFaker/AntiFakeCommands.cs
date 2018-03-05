using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AntiFaker
{
	public class AntiFakeCommands : NetworkBehaviour
	{
		public AntiFakeCommands()
		{
		}

		private void Start()
		{
			this.scp173 = base.GetComponent<Scp173PlayerScript>();
			this.scp096 = base.GetComponent<Scp096PlayerScript>();
			if (TutorialManager.status)
			{
				return;
			}
			if (base.isLocalPlayer && base.isServer)
			{
				AntiFakeCommands.allowedTeleportPositions.Clear();
				this.AddTypeToList("Spawnpoint");
				AntiFakeCommands.host = this;
			}
			this.ccm = base.GetComponent<CharacterClassManager>();
			this.pms = base.GetComponent<PlyMovementSync>();
			base.StartCoroutine(this.AntiSpeedhack());
		}

		public bool CheckMovement(Vector3 pos)
		{
			if (TutorialManager.status)
			{
				return true;
			}
			this.distanceTraveled += Vector2.Distance(new Vector2(this.prevPos.x, this.prevPos.z), new Vector2(pos.x, pos.z));
			float num = 0f;
			if (this.ccm.curClass == 0)
			{
				num = this.scp173.boost_teleportDistance.Evaluate(base.GetComponent<PlayerStats>().GetHealthPercent()) * 2f;
			}
			else if (this.ccm.curClass > 0)
			{
				num = this.ccm.klasy[this.ccm.curClass].runSpeed;
			}
			if (this.ccm.curClass == 9 && this.scp096.enraged == Scp096PlayerScript.RageState.Enraged)
			{
				num *= 4.9f;
			}
			if (this.distanceTraveled < num * 1.3f || this.SpeedhackJustification(pos) || (base.isLocalPlayer && base.isServer))
			{
				this.prevPos = pos;
				return true;
			}
			return false;
		}

		private IEnumerator AntiSpeedhack()
		{
			for (;;)
			{
				this.distanceTraveled = 0f;
				yield return new WaitForSeconds(1f);
			}
			yield break;
		}

		public bool SpeedhackJustification(Vector3 pos)
		{
			int curClass = this.ccm.curClass;
			if (Vector3.Distance(pos, this.ccm.deathPosition) < 10f || pos.y > 2000f || pos.y < -1500f)
			{
				return true;
			}
			foreach (Transform transform in AntiFakeCommands.allowedTeleportPositions)
			{
				if (Vector3.Distance(pos, transform.position) < 10f)
				{
					if (transform.tag == "SP_CDP" && curClass != 1)
					{
						return false;
					}
					if (transform.tag == "SP_173" && curClass != 0)
					{
						return false;
					}
					if (transform.tag == "SP_106" && curClass != 3)
					{
						return false;
					}
					if (transform.tag == "SP_049" && curClass != 5)
					{
						return false;
					}
					if (transform.tag == "SP_MTF" && this.ccm.klasy[curClass].team != Team.MTF)
					{
						return false;
					}
					if (transform.tag == "SP_RSC" && curClass != 6)
					{
						return false;
					}
					if (transform.tag == "SP_CI" && curClass != 8)
					{
						return false;
					}
					return true;
				}
			}
			return curClass == 3 && Vector3.Distance(pos, GameObject.Find("SCP106_PORTAL").transform.position) < 10f;
		}

		public void FindAllowedTeleportPositions()
		{
			if (TutorialManager.status)
			{
				return;
			}
			this.AddTypeToList("SP_CDP");
			this.AddTypeToList("SP_173");
			this.AddTypeToList("SP_106");
			this.AddTypeToList("SP_049");
			this.AddTypeToList("SP_MTF");
			this.AddTypeToList("SP_RSC");
			this.AddTypeToList("SP_079");
			this.AddTypeToList("SCP_096");
			this.AddTypeToList("PD_EXIT");
			this.AddTypeToList("SP_CI");
			this.AddTypeToList("LiftTarget");
		}

		private void AddTypeToList(string type)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(type);
			foreach (GameObject gameObject in array)
			{
				AntiFakeCommands.allowedTeleportPositions.Add(gameObject.transform);
			}
		}

		static AntiFakeCommands()
		{
			// Note: this type is marked as 'beforefieldinit'.
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

		private static List<Transform> allowedTeleportPositions = new List<Transform>();

		private static AntiFakeCommands host;

		private Scp173PlayerScript scp173;

		private Scp096PlayerScript scp096;

		private PlyMovementSync pms;

		private CharacterClassManager ccm;

		private float distanceTraveled;

		private Vector3 prevPos = Vector3.zero;
	}
}
