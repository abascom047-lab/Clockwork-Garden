using Assets.scripts.Clockwork.Parts.Interfaces;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class TemperatureStatusController : MinMaxThresholdStatusController {
		public const float TEMPERATURE_THRESHOLD = 100f;

		private IPartSpeed _speedInterface;
		private IPartDamageable _damageableInterface;

		protected override void OnStart() {
			_type = StatusType.Temperature;
			_minThreshold = -TEMPERATURE_THRESHOLD;
			_maxThreshold = TEMPERATURE_THRESHOLD;

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
