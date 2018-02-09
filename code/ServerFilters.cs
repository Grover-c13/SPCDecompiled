using System;
using UnityEngine;

public class ServerFilters : MonoBehaviour
{
	public bool AllowToSpawn(string server_name)
	{
		if (this.nameFilter.Length == 0)
		{
			return true;
		}
		int num = 0;
		int num2 = 0;
		foreach (char c in this.nameFilter)
		{
			for (int j = num2; j < server_name.Length; j++)
			{
				if (server_name[j] == c)
				{
					num2 = j;
					num++;
					break;
				}
			}
		}
		return num == this.nameFilter.Length;
	}

	private void Start()
	{
		this.list = base.GetComponent<ServerListManager>();
	}

	private ServerListManager list;

	public string nameFilter;
}
