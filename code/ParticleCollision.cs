using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
	public ParticleCollision()
	{
	}

	private void Start()
	{
		this.m_ParticleSystem = base.GetComponent<ParticleSystem>();
	}

	private void OnParticleCollision(GameObject other)
	{
		int collisionEvents = this.m_ParticleSystem.GetCollisionEvents(other, this.m_CollisionEvents);
		for (int i = 0; i < collisionEvents; i++)
		{
			Component colliderComponent = this.m_CollisionEvents[i].colliderComponent;
			ExtinguishableFire component = colliderComponent.GetComponent<ExtinguishableFire>();
			if (component != null)
			{
				component.Extinguish();
			}
		}
	}

	private List<ParticleCollisionEvent> m_CollisionEvents = new List<ParticleCollisionEvent>();

	private ParticleSystem m_ParticleSystem;
}
