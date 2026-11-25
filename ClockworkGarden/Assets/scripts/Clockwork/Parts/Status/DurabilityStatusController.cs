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
}
