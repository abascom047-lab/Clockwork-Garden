using Assets.scripts.Clockwork.Parts.Status;
using Assets.scripts.Clockwork.Parts.Status.Conditions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.scripts.Clockwork.Parts.Helpers {
	public static class DamageableHelpers {
		public static void AddDamage(BaseStatusController[] statusControls, StatusType statusType, float damage) {
			foreach (BaseStatusController control in statusControls)
				control.Add(statusType, damage);
		}

		public static bool TryGetHighestPriorityCondition(HashSet<IStatusCondition> conditions, out IStatusCondition priorityCondition) {
			priorityCondition = null;
			if (conditions.Count == 0)
				return false;

			priorityCondition = conditions.First();
			foreach (IStatusCondition condition in conditions) {
				if (condition.Priority < priorityCondition.Priority)
					priorityCondition = condition;
			}

			return true;
		}
	}
}
