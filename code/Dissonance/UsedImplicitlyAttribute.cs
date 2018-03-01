using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class UsedImplicitlyAttribute : Attribute
	{
		public UsedImplicitlyAttribute() : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags) : this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags) : this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			this.UseKindFlags = useKindFlags;
			this.TargetFlags = targetFlags;
		}

		public ImplicitUseKindFlags UseKindFlags
		{
			[CompilerGenerated]
			get
			{
				return this.<UseKindFlags>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<UseKindFlags>k__BackingField = value;
			}
		}

		public ImplicitUseTargetFlags TargetFlags
		{
			[CompilerGenerated]
			get
			{
				return this.<TargetFlags>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<TargetFlags>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private ImplicitUseKindFlags <UseKindFlags>k__BackingField;

		[CompilerGenerated]
		private ImplicitUseTargetFlags <TargetFlags>k__BackingField;
	}
}
