using Assets.scripts.Clockwork.Parts.Interfaces;
using Assets.scripts.Clockwork.Parts.Status;

namespace Assets.scripts.Clockwork.Parts {
	public class TemperatureStatusController : MinMaxThresholdStatusController {
		IPartSpeed _speedInterface;
		IPartDamageable _damageableInterface;

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
