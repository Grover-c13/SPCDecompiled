using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AntiFaker
{
	public class AntiFakeCommands : NetworkBehaviour
	{
		private void Start()
		{
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
			float num = (this.ccm.curClass >= 0) ? ((this.ccm.curClass != 0) ? this.ccm.klasy[this.ccm.curClass].runSpeed : (base.GetComponent<Scp173PlayerScript>().boost_teleportDistance.Evaluate(base.GetComponent<PlayerStats>().GetHealthPercent()) * 2f)) : 0f;
			if (this.distanceTraveled < num * 1.3f || this.SpeedhackJustification(pos))
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
			int curClass = base.GetComponent<CharacterClassManager>().curClass;
			if (Vector3.Distance(pos, base.GetComponent<CharacterClassManager>().deathPosition) < 10f || pos.y > 2000f || pos.y < -1500f)
			{
				return true;
			}
			foreach (Transform transform in AntiFakeCommands.allowedTeleportPositions)
			{
				if (Vector3.Distance(pos, transform.position) < 10f)
				{
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

		private PlyMovementSync pms;

		private CharacterClassManager ccm;

		private float distanceTraveled;

		private Vector3 prevPos = Vector3.zero;
	}
}
