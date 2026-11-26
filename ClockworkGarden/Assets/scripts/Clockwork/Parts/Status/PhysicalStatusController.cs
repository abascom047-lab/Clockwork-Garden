using Assets.scripts.Clockwork.Parts.Interfaces;
using Assets.scripts.Clockwork.Parts.Status.Conditions;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class PhysicalStatusController : SingleThresholdStatusController {
		public DurabilityStatusController DurabilityControl;
		public float MaxValue = 100f;

		private IPartDamageable _damageableInterface;
		private DisabledStatusCondition _disabledCondition;

		public override void Add(StatusType addType, float addValue) {
			if (addType != StatusType.Physical)
				return;

			float percentage = Mathf.Abs(addValue / MaxValue);
			float durabilityChange = DurabilityStatusController.MAX_VALUE * percentage;

			if (addValue > 0) {
				if (CurrentValue >= MaxValue) {
					DurabilityControl.Add(StatusType.Durability, durabilityChange);
				}

				base.Add(addType, addValue);
				CurrentValue = Mathf.Min(CurrentValue, MaxValue);
				
				return;
			}

			addValue *= 1 - DurabilityControl.CurrentValue / DurabilityStatusController.MAX_VALUE;

			base.Add(addType, addValue);
			DurabilityControl.Add(StatusType.Durability, -durabilityChange);
			CurrentValue = Mathf.Max(CurrentValue, StartValue);
		}

		public override void Set(StatusType setType, float newValue) {
			base.Set(setType, newValue);
			CurrentValue = Mathf.Clamp(CurrentValue, StartValue, MaxValue);
		}

		protected override void OnStart() {
			StartValue = 0f;
			CurrentValue = Mathf.Clamp(CurrentValue, StartValue, MaxValue);

			_type = StatusType.Physical;
			_threshold = 0;

			_damageableInterface = gameObject.GetComponent<IPartDamageable>();
			_disabledCondition = new DisabledStatusCondition();

			base.OnStart();
		}

		protected override void AtThreshold() {
			_damageableInterface.AddStatusCondition(_disabledCondition);
			base.AtThreshold();
		}

		protected override void AboveThreshold() {
			_damageableInterface.RemoveStatusCondition(_disabledCondition);
			base.AboveThreshold();
		}

		protected override void BelowThreshold() {
			_damageableInterface.AddStatusCondition(_disabledCondition);
			base.BelowThreshold();
		}
	}
}
