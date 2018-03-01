using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
	internal sealed class MacroAttribute : Attribute
	{
		public MacroAttribute()
		{
		}

		[CanBeNull]
		public string Expression
		{
			[CompilerGenerated]
			get
			{
				return this.<Expression>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Expression>k__BackingField = value;
			}
		}

		public int Editable
		{
			[CompilerGenerated]
			get
			{
				return this.<Editable>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Editable>k__BackingField = value;
			}
		}

		[CanBeNull]
		public string Target
		{
			[CompilerGenerated]
			get
			{
				return this.<Target>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<Target>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Expression>k__BackingField;

		[CompilerGenerated]
		private int <Editable>k__BackingField;

		[CompilerGenerated]
		private string <Target>k__BackingField;
	}
}
