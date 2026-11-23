using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class DurabilityStatusController : BaseStatusController {
		public override void Add(StatusType addType, float addValue) {
			base.Add(addType, addValue);
			CurrentValue = Mathf.Clamp(CurrentValue, 0, 100);
		}

		protected override void OnStart() {
			CurrentValue = Mathf.Clamp(CurrentValue, 0, 100);
			_type = StatusType.Durability;

			base.OnStart();
		}
	}

	public class PhysicalStatusController : SingleThresholdStatusController {
		private DurabilityStatusController _durabilityControl;

		public override void Add(StatusType addType, float addValue) {
			if (addType != StatusType.Physical)
				return;

			addValue *= _durabilityControl.CurrentValue / 100f;

			base.Add(addType, addValue);
		}

		protected override void OnStart() {
			_durabilityControl = gameObject.AddComponent<DurabilityStatusController>();

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
