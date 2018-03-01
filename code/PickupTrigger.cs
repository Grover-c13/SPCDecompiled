using System;
using UnityEngine;

public class PickupTrigger : MonoBehaviour
{
	public PickupTrigger()
	{
	}

	public bool Trigger(int item)
	{
		if (this.triggerID == -1)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return true;
		}
		if (this.filter == -1 || item == this.filter)
		{
			if (this.alias != string.Empty)
			{
				UnityEngine.Object.FindObjectOfType<TutorialManager>().Trigger(this.alias);
			}
			else
			{
				UnityEngine.Object.FindObjectOfType<TutorialManager>().Trigger(this.triggerID);
			}
			if (this.disableOnEnd)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			return true;
		}
		return false;
	}

	public int filter = -1;

	public int triggerID;

	public string alias;

	public bool disableOnEnd = true;

	public int prioirty;
}
