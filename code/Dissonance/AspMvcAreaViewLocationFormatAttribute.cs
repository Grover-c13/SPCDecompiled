﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	internal sealed class AspMvcAreaViewLocationFormatAttribute : Attribute
	{
		[NotNull]
		public string Format { get; private set; }

		public AspMvcAreaViewLocationFormatAttribute([NotNull] string format)
		{
			this.Format = format;
		}
	}
}
