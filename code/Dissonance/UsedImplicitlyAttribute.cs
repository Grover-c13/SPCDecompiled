﻿using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class UsedImplicitlyAttribute : Attribute
	{
		public ImplicitUseKindFlags UseKindFlags { get; private set; }

		public ImplicitUseTargetFlags TargetFlags { get; private set; }

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
	}
}
