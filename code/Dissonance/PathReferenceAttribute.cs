using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class PathReferenceAttribute : Attribute
	{
		public PathReferenceAttribute()
		{
		}

		public PathReferenceAttribute([PathReference, NotNull] string basePath)
		{
			this.BasePath = basePath;
		}

		[CanBeNull]
		public string BasePath
		{
			[CompilerGenerated]
			get
			{
				return this.<BasePath>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<BasePath>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <BasePath>k__BackingField;
	}
}
