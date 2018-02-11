using System;

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
		public string BasePath { get; private set; }
	}
}
