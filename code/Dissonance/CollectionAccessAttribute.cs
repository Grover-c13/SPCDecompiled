using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	internal sealed class CollectionAccessAttribute : Attribute
	{
		public CollectionAccessAttribute(CollectionAccessType collectionAccessType)
		{
			this.CollectionAccessType = collectionAccessType;
		}

		public CollectionAccessType CollectionAccessType { get; private set; }
	}
}
