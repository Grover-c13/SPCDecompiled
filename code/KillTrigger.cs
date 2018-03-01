using System;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
	public KillTrigger()
	{
	}

	public void Trigger(int amount)
	{
		if (amount != this.killsToTrigger)
		{
			return;
		}
		if (this.triggerID == -1)
		{
			UnityEngine.Object.FindObjectOfType<TutorialManager>().Tutorial2_Result();
		}
		else if (this.alias != string.Empty)
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
	}

	public int killsToTrigger;

	public int triggerID;

	public string alias;

	public bool disableOnEnd = true;

	public int prioirty;
}
