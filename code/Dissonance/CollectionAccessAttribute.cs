using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	internal sealed class CollectionAccessAttribute : Attribute
	{
		public CollectionAccessAttribute(CollectionAccessType collectionAccessType)
		{
			this.CollectionAccessType = collectionAccessType;
		}

		public CollectionAccessType CollectionAccessType
		{
			[CompilerGenerated]
			get
			{
				return this.<CollectionAccessType>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<CollectionAccessType>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private CollectionAccessType <CollectionAccessType>k__BackingField;
	}
}
