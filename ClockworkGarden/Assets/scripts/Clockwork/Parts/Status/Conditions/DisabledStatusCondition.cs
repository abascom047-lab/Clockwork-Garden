namespace Assets.scripts.Clockwork.Parts.Status.Conditions {
	public class DisabledStatusCondition : IStatusCondition {
		public StatusCondtionType ConditionType { get; private set; } = StatusCondtionType.Disabled;
		public int Priority { get; private set; } = 0;

		public bool TryGetSpeedModifier(out float modifier) {
			modifier = 0;
			return true;
		}
	}
}
