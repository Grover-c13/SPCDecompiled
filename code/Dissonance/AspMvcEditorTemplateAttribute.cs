﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class AspMvcEditorTemplateAttribute : Attribute
	{
		public AspMvcEditorTemplateAttribute()
		{
		}
	}
}
