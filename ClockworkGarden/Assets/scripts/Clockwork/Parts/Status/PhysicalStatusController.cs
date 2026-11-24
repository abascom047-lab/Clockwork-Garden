using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class DurabilityStatusController : BaseStatusController {
		public const float MAX_VALUE = 100f;

		public override void Add(StatusType addType, float addValue) {
			base.Add(addType, addValue);
			CurrentValue = Mathf.Clamp(CurrentValue, StartValue, MAX_VALUE);
		}

		public override void Set(StatusType setType, float newValue) {
			base.Set(setType, newValue);
			CurrentValue = Mathf.Clamp(CurrentValue, StartValue, MAX_VALUE);
		}

		protected override void OnStart() {
			CurrentValue = MAX_VALUE;
			_type = StatusType.Durability;

			base.OnStart();
		}
	}

	public class PhysicalStatusController : SingleThresholdStatusController {
		public float MaxValue = 100f;

		private DurabilityStatusController _durabilityControl;

		public override void Add(StatusType addType, float addValue) {
			if (addType != StatusType.Physical)
				return;

			addValue *= _durabilityControl.CurrentValue / DurabilityStatusController.MAX_VALUE;

			float percentage = addValue / MaxValue;
			float lostDurability = DurabilityStatusController.MAX_VALUE * percentage;
			_durabilityControl.Add(StatusType.Durability, lostDurability);

			base.Add(addType, addValue);
			CurrentValue = Mathf.Clamp(CurrentValue, StartValue, MaxValue);
		}

		public override void Set(StatusType setType, float newValue) {
			base.Set(setType, newValue);
			CurrentValue = Mathf.Clamp(CurrentValue, StartValue, MaxValue);
		}

		protected override void OnStart() {
			_durabilityControl = gameObject.AddComponent<DurabilityStatusController>();

			StartValue = 0f;
			CurrentValue = Mathf.Clamp(CurrentValue, StartValue, MaxValue);

			_type = StatusType.Physical;
			_threshold = 0;

			base.OnStart();
		}

		protected override void BelowThreshold() {
			// add status condition
			base.BelowThreshold();
		}
	}
}
