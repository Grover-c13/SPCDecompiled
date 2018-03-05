﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class RazorImportNamespaceAttribute : Attribute
	{
		[NotNull]
		public string Name { get; private set; }

		public RazorImportNamespaceAttribute([NotNull] string name)
		{
			this.Name = name;
		}
	}
}
