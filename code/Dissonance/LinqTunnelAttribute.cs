﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class LinqTunnelAttribute : Attribute
	{
		public LinqTunnelAttribute()
		{
		}
	}
}
