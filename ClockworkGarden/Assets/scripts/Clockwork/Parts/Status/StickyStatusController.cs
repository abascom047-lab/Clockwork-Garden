using Assets.scripts.Clockwork.Parts.Interfaces;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class StickyStatusController : ManyThresholdStatusController {
		private IPartControllableSpeed partSpeed;

		protected override void OnStart() {
			Type = StatusType.Sticky;
			Reverse = false;
			partSpeed = GetComponent<IPartControllableSpeed>();

			base.OnStart();
		}

		protected override void ThresholdPassed(int index) {
			if (partSpeed != null) {
				float baseSpeed = partSpeed.GetBaseSpeedByIndex(1);
				float speedIncrement = baseSpeed / (Thresholds.Length - 1);
				float newSpeed = baseSpeed - speedIncrement * index;

				partSpeed.SetSpeedByIndex(0, newSpeed);
			}

			base.ThresholdPassed(index);
		}
	}
}
