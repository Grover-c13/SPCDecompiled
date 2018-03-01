using System;
using UnityEngine;

public class NoammoTrigger : MonoBehaviour
{
	public NoammoTrigger()
	{
	}

	public bool Trigger(int item)
	{
		bool flag = false;
		foreach (int num in this.optionalForcedID)
		{
			if (TutorialManager.curlog == num)
			{
				flag = true;
			}
		}
		if (!flag && this.optionalForcedID.Length != 0)
		{
			return false;
		}
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

	public int[] optionalForcedID;
}
