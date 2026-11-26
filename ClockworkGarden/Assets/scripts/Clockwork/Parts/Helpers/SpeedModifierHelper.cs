using Assets.scripts.Clockwork.Parts.Interfaces;

namespace Assets.scripts.Clockwork.Parts.Helpers {
	public static class SpeedModifierHelper {
		public static float GetPrimarySpeed(IPartSpeed modSpeed) {
			float newSpeed = modSpeed.CurrentPrimarySpeed;
			foreach (float modifier in modSpeed.PrimarySpeedModifiers.Values)
				newSpeed *= modifier;

			return newSpeed;
		}

		public static float GetSecondarySpeed(IPartSpeed modSpeed) {
			float newSpeed = modSpeed.CurrentSecondarySpeed;
			foreach (float modifier in modSpeed.SecondarySpeedModifiers.Values)
				newSpeed *= modifier;

			return newSpeed;
		}
	}
}
