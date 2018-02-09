using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
	private void OnLevelWasLoaded(int level)
	{
		CursorManager.UnsetAll();
	}

	private void LateUpdate()
	{
		bool flag = CursorManager.eqOpen | CursorManager.pauseOpen | CursorManager.isServerOnly | CursorManager.consoleOpen | CursorManager.is079 | CursorManager.scp106 | CursorManager.roundStarted | CursorManager.raOp;
		Cursor.lockState = ((!flag) ? CursorLockMode.Locked : CursorLockMode.None);
		Cursor.visible = flag;
	}

	public static void UnsetAll()
	{
		CursorManager.eqOpen = false;
		CursorManager.pauseOpen = false;
		CursorManager.isServerOnly = false;
		CursorManager.consoleOpen = false;
		CursorManager.is079 = false;
		CursorManager.scp106 = false;
		CursorManager.roundStarted = false;
		CursorManager.raOp = false;
	}

	public static bool eqOpen;

	public static bool pauseOpen;

	public static bool isServerOnly;

	public static bool consoleOpen;

	public static bool is079;

	public static bool scp106;

	public static bool roundStarted;

	public static bool raOp;
}
