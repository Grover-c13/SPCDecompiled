﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class AspMvcViewComponentAttribute : Attribute
	{
		public AspMvcViewComponentAttribute()
		{
		}
	}
}
