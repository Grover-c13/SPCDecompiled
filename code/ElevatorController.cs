using System;
using UnityEngine;
using UnityEngine.Networking;

public class ElevatorController : NetworkBehaviour
{
	public bool Teleport(string id, bool onlyCheck = false)
	{
		LiftIdentity liftIdentity = null;
		LiftIdentity liftIdentity2 = null;
		LiftIdentity[] array = UnityEngine.Object.FindObjectsOfType<LiftIdentity>();
		foreach (LiftIdentity liftIdentity3 in array)
		{
			if (liftIdentity3.InArea(base.transform.position) && liftIdentity3.identity == id)
			{
				liftIdentity = liftIdentity3;
			}
		}
		if (liftIdentity != null)
		{
			if (!onlyCheck)
			{
				foreach (LiftIdentity liftIdentity4 in array)
				{
					if (liftIdentity4.identity == liftIdentity.identity && liftIdentity4.isSecond != liftIdentity.isSecond)
					{
						liftIdentity2 = liftIdentity4;
					}
				}
				base.transform.SetParent(liftIdentity.GetComponentInParent<MeshCollider>().transform);
				Vector3 localPosition = base.transform.localPosition;
				Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
				base.transform.SetParent(liftIdentity2.GetComponentInParent<MeshCollider>().transform);
				base.transform.localPosition = localPosition;
				base.transform.GetComponentInParent<FirstPersonController>().m_MouseLook.SetRotation(Quaternion.Euler(new Vector3(0f, liftIdentity2.GetComponentInParent<MeshCollider>().transform.rotation.eulerAngles.y - liftIdentity.GetComponentInParent<MeshCollider>().transform.rotation.eulerAngles.y, 0f)));
				base.transform.parent = null;
			}
			else
			{
				foreach (LiftIdentity liftIdentity5 in array)
				{
					if (!(liftIdentity5.identity == liftIdentity.identity) || liftIdentity5.isSecond)
					{
					}
				}
				if (liftIdentity.isSecond != liftIdentity.isUp)
				{
					return false;
				}
			}
		}
		return true;
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
}
