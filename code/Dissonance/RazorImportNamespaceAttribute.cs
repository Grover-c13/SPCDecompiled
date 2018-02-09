using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class RazorImportNamespaceAttribute : Attribute
	{
		public RazorImportNamespaceAttribute([NotNull] string name)
		{
			this.Name = name;
		}

		[NotNull]
		public string Name { get; private set; }
	}
}
