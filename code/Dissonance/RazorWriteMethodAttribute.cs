﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class RazorWriteMethodAttribute : Attribute
	{
		public RazorWriteMethodAttribute()
		{
		}
	}
}
