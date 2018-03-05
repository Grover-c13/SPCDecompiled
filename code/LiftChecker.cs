using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LiftChecker : NetworkBehaviour
{
	public LiftChecker()
	{
	}

	private IEnumerator PlayerCoroutine()
	{
		for (;;)
		{
			foreach (Lift lift in this.lifts)
			{
				if (lift.status != Lift.Status.Moving)
				{
					foreach (Lift.Elevator item in lift.elevators)
					{
						if (!item.door.GetBool("isOpen") && lift.operative)
						{
							bool found = true;
							if (Mathf.Abs(item.target.position.x - base.transform.position.x) > lift.maxDistance)
							{
								found = false;
							}
							yield return new WaitForEndOfFrame();
							if (found && Mathf.Abs(item.target.position.y - base.transform.position.y) > lift.maxDistance)
							{
								found = false;
							}
							yield return new WaitForEndOfFrame();
							if (found && Mathf.Abs(item.target.position.z - base.transform.position.z) > lift.maxDistance)
							{
								found = false;
							}
							yield return new WaitForEndOfFrame();
							if (found)
							{
								Transform transform = null;
								GameObject gameObject = item.target.gameObject;
								foreach (Lift.Elevator elevator in lift.elevators)
								{
									if (elevator.target.gameObject != gameObject)
									{
										transform = elevator.target;
									}
								}
								base.transform.parent = gameObject.transform;
								Vector3 localPosition = base.transform.transform.localPosition;
								base.transform.parent = transform.transform;
								base.transform.localPosition = localPosition;
								base.transform.parent = null;
							}
						}
					}
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void Start()
	{
		this.glitch = UnityEngine.Object.FindObjectOfType<ExplosionCameraShake>();
		this.lifts = UnityEngine.Object.FindObjectsOfType<Lift>();
		if (base.isLocalPlayer)
		{
			base.StartCoroutine(this.PlayerCoroutine());
			base.StartCoroutine(this.AmInElevator());
		}
	}

	private IEnumerator AmInElevator()
	{
		for (;;)
		{
			foreach (Lift item in this.lifts)
			{
				if (item.statusID == 2)
				{
					GameObject my = null;
					if (item.InRange(base.transform.position, out my))
					{
						yield return new WaitForSeconds(1.7f);
						int movingtime = 0;
						while (item.InRange(base.transform.position, out my) && movingtime < 20)
						{
							this.glitch.Shake(UnityEngine.Random.Range(0f, 0.08f));
							yield return new WaitForSeconds(0.05f);
							if (item.statusID != 2)
							{
								movingtime++;
							}
						}
					}
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
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

	private Lift[] lifts;

	private ExplosionCameraShake glitch;
}
