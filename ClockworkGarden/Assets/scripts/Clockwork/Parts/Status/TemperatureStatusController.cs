using Assets.scripts.Clockwork.Parts.Interfaces;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class TemperatureStatusController : MinMaxThresholdStatusController {
		IPartSpeed _speedInterface;
		IPartDamageable _damageableInterface;

		public bool Hot => _state == ThresholdState.Over;
		public bool Cold => _state == ThresholdState.Under;

		protected override void OnStart() {
			_type = StatusType.Temperature;
			_minThreshold = -100;
			_maxThreshold = 100;

			_speedInterface = GetComponent<IPartSpeed>();
			_damageableInterface = GetComponent<IPartDamageable>();

			base.OnStart();
		}

		protected override void BetweenThresholds() {
			_speedInterface.DeleteSpeedModifier(_type);
			_damageableInterface.DeleteDurabilityModifier(_type);

			base.BetweenThresholds();
		}

		protected override void OverMaxThreshold() {
			_speedInterface.UpsertSpeedModifier(_type, 1.5f);
			_damageableInterface.UpsertDurabilityModifier(_type, .5f);

			base.OverMaxThreshold();
		}

		protected override void UnderMinThreshold() {
			_speedInterface.UpsertSpeedModifier(_type, .5f);
			_damageableInterface.UpsertDurabilityModifier(_type, .5f);

			base.UnderMinThreshold();
		}
	}
}
