using System;
using UnityEngine;
using UnityEngine.Networking;

public class ShockGrenade : GrenadeInstance
{
	public ShockGrenade()
	{
	}

	public override void Explode(int thrower)
	{
		base.Explode(thrower);
		UnityEngine.Object.Destroy(UnityEngine.Object.Instantiate<GameObject>(this.gfx, base.transform.position, Quaternion.Euler(Vector3.zero)), 8f);
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
			{
				ExplosionCameraShake explosionCameraShake = UnityEngine.Object.FindObjectOfType<ExplosionCameraShake>();
				if (explosionCameraShake != null)
				{
					explosionCameraShake.Shake(this.shakeOverRange.Evaluate(Vector3.Distance(base.transform.position, gameObject.transform.position)));
				}
				if (gameObject.name == "Host")
				{
					this.HurtPlayers(thrower);
				}
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void HurtPlayers(int thrower)
	{
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			float num = this.damageOverRange.Evaluate(Vector3.Distance(base.transform.position, gameObject.transform.position) / this.range);
			if (num > 1f && gameObject.GetComponent<WeaponManager>().GetShootPermission(gameObject.GetComponent<CharacterClassManager>().klasy[thrower].team))
			{
				gameObject.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(num, "WORLD", "TESLA"), gameObject);
			}
		}
	}

	public GameObject gfx;

	public float range;

	public AnimationCurve damageOverRange;

	public AnimationCurve shakeOverRange;

	public LayerMask dmgmask;
}
