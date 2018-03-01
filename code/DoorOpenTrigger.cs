using System;
using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
	public DoorOpenTrigger()
	{
	}

	private void Update()
	{
		if (this.door.isOpen == this.stageToTrigger)
		{
			if (this.alias != string.Empty)
			{
				UnityEngine.Object.FindObjectOfType<TutorialManager>().Trigger(this.alias);
			}
			else
			{
				UnityEngine.Object.FindObjectOfType<TutorialManager>().Trigger(this.id);
			}
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
	}

	public Door door;

	public bool stageToTrigger = true;

	public int id;

	public string alias;
}
