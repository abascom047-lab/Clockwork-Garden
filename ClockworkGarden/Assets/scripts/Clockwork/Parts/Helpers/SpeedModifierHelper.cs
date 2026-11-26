using Assets.scripts.Clockwork.Parts.Interfaces;

namespace Assets.scripts.Clockwork.Parts.Helpers {
	public static class SpeedModifierHelper {
		public static float getPrimarySpeed(IPartModifiableSpeed modSpeed) {
			float newSpeed = modSpeed.BasePrimarySpeed;
			foreach (float modifier in modSpeed.PrimarySpeedModifiers.Values)
				newSpeed *= modifier;

			return newSpeed;
		}

		public static float getSecondarySpeed(IPartModifiableSpeed modSpeed) {
			float newSpeed = modSpeed.BaseSecondarySpeed;
			foreach (float modifier in modSpeed.SecondarySpeedModifiers.Values)
				newSpeed *= modifier;

			return newSpeed;
		}
	}
}
