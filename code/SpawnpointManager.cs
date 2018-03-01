using System;
using UnityEngine;

public class SpawnpointManager : MonoBehaviour
{
	public SpawnpointManager()
	{
	}

	public GameObject GetRandomPosition(int classID)
	{
		GameObject result = null;
		Class @class = GameObject.Find("Host").GetComponent<CharacterClassManager>().klasy[classID];
		if (@class.team == Team.CDP || @class.team == Team.TUT)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("SP_CDP");
			int num = UnityEngine.Random.Range(0, array.Length);
			result = array[num];
		}
		if (classID == 10)
		{
			return null;
		}
		if (@class.team == Team.SCP)
		{
			if (classID == 3)
			{
				GameObject[] array2 = GameObject.FindGameObjectsWithTag("SP_106");
				int num2 = UnityEngine.Random.Range(0, array2.Length);
				result = array2[num2];
			}
			else if (classID == 5)
			{
				GameObject[] array3 = GameObject.FindGameObjectsWithTag("SP_049");
				int num3 = UnityEngine.Random.Range(0, array3.Length);
				result = array3[num3];
			}
			else if (classID == 7)
			{
				GameObject[] array4 = GameObject.FindGameObjectsWithTag("SP_079");
				int num4 = UnityEngine.Random.Range(0, array4.Length);
				result = array4[num4];
			}
			else if (classID == 9)
			{
				GameObject[] array5 = GameObject.FindGameObjectsWithTag("SCP_096");
				int num5 = UnityEngine.Random.Range(0, array5.Length);
				result = array5[num5];
			}
			else
			{
				GameObject[] array6 = GameObject.FindGameObjectsWithTag("SP_173");
				int num6 = UnityEngine.Random.Range(0, array6.Length);
				result = array6[num6];
			}
		}
		if (@class.team == Team.MTF)
		{
			GameObject[] array7 = GameObject.FindGameObjectsWithTag("SP_MTF");
			int num7 = UnityEngine.Random.Range(0, array7.Length);
			result = array7[num7];
		}
		if (@class.team == Team.RSC)
		{
			GameObject[] array8 = GameObject.FindGameObjectsWithTag("SP_RSC");
			int num8 = UnityEngine.Random.Range(0, array8.Length);
			result = array8[num8];
		}
		if (@class.team == Team.CHI)
		{
			GameObject[] array9 = GameObject.FindGameObjectsWithTag("SP_CI");
			int num9 = UnityEngine.Random.Range(0, array9.Length);
			result = array9[num9];
		}
		return result;
	}
}
