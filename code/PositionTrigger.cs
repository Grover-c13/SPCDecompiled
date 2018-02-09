using System;
using UnityEngine;

public class PositionTrigger : MonoBehaviour
{
	private void Update()
	{
		if (this.ply == null)
		{
			this.ply = GameObject.FindGameObjectWithTag("Player");
			return;
		}
		if (Vector3.Distance(this.ply.transform.position, base.transform.position) <= this.range)
		{
			UnityEngine.Object.FindObjectOfType<TutorialManager>().Trigger(this.id);
			if (this.disableOnEnd)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0.1f, 0.2f, 0.2f);
		Gizmos.DrawSphere(base.transform.position, this.range);
	}

	public bool disableOnEnd = true;

	public int id;

	public float range;

	private GameObject ply;
}
