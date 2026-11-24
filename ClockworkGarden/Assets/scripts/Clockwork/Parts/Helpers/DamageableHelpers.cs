using Assets.scripts.Clockwork.Parts.Status;

namespace Assets.scripts.Clockwork.Parts.Helpers {
	public static class DamageableHelpers {
		public static void AddDamage(BaseStatusController[] statusControls, StatusType statusType, float damage) {
			foreach (BaseStatusController control in statusControls)
				control.Add(statusType, damage);
		}
	}
}
