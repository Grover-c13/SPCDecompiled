using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal sealed class AspChildControlTypeAttribute : Attribute
	{
		public AspChildControlTypeAttribute([NotNull] string tagName, [NotNull] Type controlType)
		{
			this.TagName = tagName;
			this.ControlType = controlType;
		}

		[NotNull]
		public string TagName
		{
			[CompilerGenerated]
			get
			{
				return this.<TagName>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<TagName>k__BackingField = value;
			}
		}

		[NotNull]
		public Type ControlType
		{
			[CompilerGenerated]
			get
			{
				return this.<ControlType>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<ControlType>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <TagName>k__BackingField;

		[CompilerGenerated]
		private Type <ControlType>k__BackingField;
	}
}
