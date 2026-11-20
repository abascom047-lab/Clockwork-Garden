using Assets.scripts.Clockwork.Parts.Status;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Hinge {
	public class ClockworkPartHingeStatusController : MonoBehaviour {
		public ClockworkPartHingeController HingeControl;
		public ManyThresholdStatusController StickyControl;

		void OnEnable() {
			StickyControl.ThresholdPassed += OnStickyThresholdPassed;
		}

		void OnDisable() {
			StickyControl.ThresholdPassed -= OnStickyThresholdPassed;
		}

		private void OnStickyThresholdPassed(int threshold) {
			float speedIncrement = HingeControl.Speed / (StickyControl.Thresholds.Length - 1);
			float newSpeed = HingeControl.Speed - speedIncrement * threshold;

			HingeControl.SetOpenSpeed(newSpeed);
		}
	}
}
