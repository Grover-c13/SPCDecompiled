﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class InstantHandleAttribute : Attribute
	{
		public InstantHandleAttribute()
		{
		}
	}
}
