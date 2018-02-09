using System;
using System.Collections;
using UnityEngine;

public class MenuAnimator : MonoBehaviour
{
	private void Update()
	{
		bool flag = (this.con1.activeSelf | this.con2.activeSelf | base.GetComponent<MainMenuScript>().submenus[6].activeSelf) || this.dsc.activeSelf;
		this.kamera.transform.position = Vector3.Lerp(this.kamera.transform.position, (!flag) ? this.unfoc.transform.position : this.foc.transform.position, Time.deltaTime * 2f);
		this.kamera.transform.rotation = Quaternion.Lerp(this.kamera.transform.rotation, (!flag) ? this.unfoc.transform.rotation : this.foc.transform.rotation, Time.deltaTime);
	}

	private void Start()
	{
		base.StartCoroutine(this.Animate());
	}

	private IEnumerator Animate()
	{
		for (;;)
		{
			int t = UnityEngine.Random.Range(2, 5);
			foreach (SignBlink signBlink in UnityEngine.Object.FindObjectsOfType<SignBlink>())
			{
				signBlink.Play(t);
			}
			yield return new WaitForSeconds((float)UnityEngine.Random.Range(3, 10));
		}
		yield break;
	}

	public GameObject kamera;

	public GameObject con1;

	public GameObject con2;

	public GameObject foc;

	public GameObject unfoc;

	public GameObject dsc;
}
