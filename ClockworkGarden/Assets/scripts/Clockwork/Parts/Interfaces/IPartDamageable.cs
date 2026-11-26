using Assets.scripts.Clockwork.Parts.Status;
using Assets.scripts.Clockwork.Parts.Status.Conditions;
using System.Collections.Generic;

namespace Assets.scripts.Clockwork.Parts.Interfaces {
	public interface IPartDamageable {
		HashSet<IStatusCondition> StatusConditions { get; }

		void AddDamage(StatusType statusType, float damage);
		void AddStatusCondition(IStatusCondition newCondition);
		void RemoveStatusCondition(IStatusCondition oldCondition);
	}
}
