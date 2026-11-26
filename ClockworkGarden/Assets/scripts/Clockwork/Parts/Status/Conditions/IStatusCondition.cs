namespace Assets.scripts.Clockwork.Parts.Status.Conditions {
	public enum StatusCondtionType {
		None,
		Disabled,
		Frozen
	}

	public interface IStatusCondition {
		StatusCondtionType ConditionType { get; }
		int Priority { get; }

		bool TryGetSpeedModifier(out float modifier);
	}
}
