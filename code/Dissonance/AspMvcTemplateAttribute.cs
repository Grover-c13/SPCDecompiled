﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class AspMvcTemplateAttribute : Attribute
	{
		public AspMvcTemplateAttribute()
		{
		}
	}
}
