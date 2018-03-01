using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class LiftChecker : NetworkBehaviour
{
	public LiftChecker()
	{
	}

	private IEnumerator PlayerCoroutine()
	{
		for (;;)
		{
			foreach (Lift lift in this.lifts)
			{
				if (lift.status != Lift.Status.Moving)
				{
					foreach (Lift.Elevator item in lift.elevators)
					{
						if (!item.door.GetBool("isOpen") && lift.operative)
						{
							bool found = true;
							if (Mathf.Abs(item.target.position.x - base.transform.position.x) > lift.maxDistance)
							{
								found = false;
							}
							yield return new WaitForEndOfFrame();
							if (found && Mathf.Abs(item.target.position.y - base.transform.position.y) > lift.maxDistance)
							{
								found = false;
							}
							yield return new WaitForEndOfFrame();
							if (found && Mathf.Abs(item.target.position.z - base.transform.position.z) > lift.maxDistance)
							{
								found = false;
							}
							yield return new WaitForEndOfFrame();
							if (found)
							{
								Transform transform = null;
								GameObject gameObject = item.target.gameObject;
								foreach (Lift.Elevator elevator in lift.elevators)
								{
									if (elevator.target.gameObject != gameObject)
									{
										transform = elevator.target;
									}
								}
								base.transform.parent = gameObject.transform;
								Vector3 localPosition = base.transform.transform.localPosition;
								base.transform.parent = transform.transform;
								base.transform.localPosition = localPosition;
								base.transform.parent = null;
							}
						}
					}
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void Start()
	{
		this.glitch = UnityEngine.Object.FindObjectOfType<ExplosionCameraShake>();
		this.lifts = UnityEngine.Object.FindObjectsOfType<Lift>();
		if (base.isLocalPlayer)
		{
			base.StartCoroutine(this.PlayerCoroutine());
			base.StartCoroutine(this.AmInElevator());
		}
	}

	private IEnumerator AmInElevator()
	{
		for (;;)
		{
			foreach (Lift item in this.lifts)
			{
				if (item.statusID == 2)
				{
					GameObject my = null;
					if (item.InRange(base.transform.position, out my))
					{
						yield return new WaitForSeconds(1.7f);
						int movingtime = 0;
						while (item.InRange(base.transform.position, out my) && movingtime < 20)
						{
							this.glitch.Shake(UnityEngine.Random.Range(0f, 0.08f));
							yield return new WaitForSeconds(0.05f);
							if (item.statusID != 2)
							{
								movingtime++;
							}
						}
					}
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	private Lift[] lifts;

	private ExplosionCameraShake glitch;

	[CompilerGenerated]
	private sealed class <PlayerCoroutine>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <PlayerCoroutine>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				if (this.<found>__3 && Mathf.Abs(this.<item>__2.target.position.y - this.$this.transform.position.y) > this.<lift>__1.maxDistance)
				{
					this.<found>__3 = false;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			case 2u:
				if (this.<found>__3 && Mathf.Abs(this.<item>__2.target.position.z - this.$this.transform.position.z) > this.<lift>__1.maxDistance)
				{
					this.<found>__3 = false;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			case 3u:
				if (this.<found>__3)
				{
					Transform transform = null;
					GameObject gameObject = this.<item>__2.target.gameObject;
					foreach (Lift.Elevator elevator in this.<lift>__1.elevators)
					{
						if (elevator.target.gameObject != gameObject)
						{
							transform = elevator.target;
						}
					}
					this.$this.transform.parent = gameObject.transform;
					Vector3 localPosition = this.$this.transform.transform.localPosition;
					this.$this.transform.parent = transform.transform;
					this.$this.transform.localPosition = localPosition;
					this.$this.transform.parent = null;
					goto IL_31C;
				}
				goto IL_31C;
			case 4u:
				IL_35C:
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 5;
				}
				return true;
			case 5u:
				this.$locvar1++;
				goto IL_389;
			case 6u:
				break;
			default:
				return false;
			}
			this.$locvar0 = this.$this.lifts;
			this.$locvar1 = 0;
			goto IL_389;
			IL_31C:
			this.$locvar3++;
			IL_32A:
			if (this.$locvar3 >= this.$locvar2.Length)
			{
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			}
			this.<item>__2 = this.$locvar2[this.$locvar3];
			if (!this.<item>__2.door.GetBool("isOpen") && this.<lift>__1.operative)
			{
				this.<found>__3 = true;
				if (Mathf.Abs(this.<item>__2.target.position.x - this.$this.transform.position.x) > this.<lift>__1.maxDistance)
				{
					this.<found>__3 = false;
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			goto IL_31C;
			IL_389:
			if (this.$locvar1 >= this.$locvar0.Length)
			{
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 6;
				}
			}
			else
			{
				this.<lift>__1 = this.$locvar0[this.$locvar1];
				if (this.<lift>__1.status != Lift.Status.Moving)
				{
					this.$locvar2 = this.<lift>__1.elevators;
					this.$locvar3 = 0;
					goto IL_32A;
				}
				goto IL_35C;
			}
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		public void Dispose()
		{
			this.$disposing = true;
			this.$PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal Lift[] $locvar0;

		internal int $locvar1;

		internal Lift <lift>__1;

		internal Lift.Elevator[] $locvar2;

		internal int $locvar3;

		internal Lift.Elevator <item>__2;

		internal bool <found>__3;

		internal LiftChecker $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}

	[CompilerGenerated]
	private sealed class <AmInElevator>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <AmInElevator>c__Iterator1()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				this.<movingtime>__3 = 0;
				goto IL_12D;
			case 2u:
				if (this.<item>__1.statusID != 2)
				{
					this.<movingtime>__3++;
					goto IL_12D;
				}
				goto IL_12D;
			case 3u:
				this.$locvar1++;
				goto IL_18D;
			case 4u:
				break;
			default:
				return false;
			}
			this.$locvar0 = this.$this.lifts;
			this.$locvar1 = 0;
			goto IL_18D;
			IL_12D:
			if (this.<item>__1.InRange(this.$this.transform.position, out this.<my>__2) && this.<movingtime>__3 < 20)
			{
				this.$this.glitch.Shake(UnityEngine.Random.Range(0f, 0.08f));
				this.$current = new WaitForSeconds(0.05f);
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			IL_160:
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 3;
			}
			return true;
			IL_18D:
			if (this.$locvar1 >= this.$locvar0.Length)
			{
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
			}
			else
			{
				this.<item>__1 = this.$locvar0[this.$locvar1];
				if (this.<item>__1.statusID != 2)
				{
					goto IL_160;
				}
				this.<my>__2 = null;
				if (!this.<item>__1.InRange(this.$this.transform.position, out this.<my>__2))
				{
					goto IL_160;
				}
				this.$current = new WaitForSeconds(1.7f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
			}
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		public void Dispose()
		{
			this.$disposing = true;
			this.$PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal Lift[] $locvar0;

		internal int $locvar1;

		internal Lift <item>__1;

		internal GameObject <my>__2;

		internal int <movingtime>__3;

		internal LiftChecker $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
