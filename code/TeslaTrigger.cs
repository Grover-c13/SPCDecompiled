using System;
using UnityEngine;

public class TeslaTrigger : MonoBehaviour
{
	private void Start()
	{
		this.pmng = PlayerManager.singleton;
	}

	private void Update()
	{
		GameObject[] players = this.pmng.players;
		foreach (GameObject gameObject in players)
		{
			if (Vector3.Distance(base.transform.position, gameObject.transform.position) <= this.activeDistance && gameObject.GetComponent<CharacterClassManager>().curClass != 2)
			{
				base.GetComponentInParent<TeslaGate>().Trigger(false, gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		base.GetComponentInParent<TeslaGate>().Trigger(true, other.gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
		Gizmos.DrawSphere(base.transform.position, this.activeDistance);
	}

	public float activeDistance = 5f;

	private PlayerManager pmng;
}
